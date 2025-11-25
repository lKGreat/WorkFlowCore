using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 菜单服务实现
/// </summary>
public class MenuService : ApplicationService, IMenuService
{
    private readonly IRepository<Menu, long> _menuRepository;
    private readonly IRepository<RoleMenu, long> _roleMenuRepository;
    private readonly IRepository<IdentityUserRole> _userRoleRepository;

    public MenuService(
        IRepository<Menu, long> menuRepository,
        IRepository<RoleMenu, long> roleMenuRepository,
        IRepository<IdentityUserRole> userRoleRepository)
    {
        _menuRepository = menuRepository;
        _roleMenuRepository = roleMenuRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<List<MenuDto>> GetListAsync()
    {
        var menus = await _menuRepository.GetListAsync();
        return menus.Select(MapToDto).ToList();
    }

    public async Task<List<MenuDto>> GetTreeAsync()
    {
        var menus = await _menuRepository.GetListAsync();
        var menuDtos = menus.Select(MapToDto).ToList();
        return BuildTree(menuDtos, null);
    }

    public async Task<List<RouterDto>> GetRoutersByRoleAsync(Guid userId)
    {
        // 1. 获取用户角色
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        if (!roleIds.Any())
        {
            return new List<RouterDto>();
        }

        // 2. 获取角色对应的菜单ID
        var roleMenus = await _roleMenuRepository.GetListAsync(rm => roleIds.Contains(rm.RoleId));
        var menuIds = roleMenus.Select(rm => rm.MenuId).Distinct().ToList();

        // 3. 获取菜单详情
        var menus = await _menuRepository.GetListAsync(m => 
            menuIds.Contains(m.Id) && 
            m.Status == "0" && 
            m.Visible);

        // 4. 构建路由树
        var menuDtos = menus.OrderBy(m => m.OrderNum).Select(MapToDto).ToList();
        var tree = BuildTree(menuDtos, null);

        // 5. 转换为路由格式
        return BuildRouters(tree);
    }

    public async Task<MenuDto> CreateAsync(MenuDto dto)
    {
        var menu = new Menu(SnowflakeIdGenerator.NextId(), dto.MenuName, dto.MenuType)
        {
            ParentId = dto.ParentId,
            Path = dto.Path,
            Component = dto.Component,
            PermissionCode = dto.PermissionCode,
            Icon = dto.Icon,
            Query = dto.Query,
            IsCache = dto.IsCache,
            MenuNameKey = dto.MenuNameKey,
            OrderNum = dto.OrderNum,
            Visible = dto.Visible,
            IsFrame = dto.IsFrame,
            Status = dto.Status,
            TenantId = CurrentTenant.Id
        };

        await _menuRepository.InsertAsync(menu);
        return MapToDto(menu);
    }

    public async Task UpdateAsync(MenuDto dto)
    {
        var menu = await _menuRepository.GetAsync(dto.MenuId);
        
        menu.MenuName = dto.MenuName;
        menu.ParentId = dto.ParentId;
        menu.MenuType = dto.MenuType;
        menu.Path = dto.Path;
        menu.Component = dto.Component;
        menu.PermissionCode = dto.PermissionCode;
        menu.Icon = dto.Icon;
        menu.Query = dto.Query;
        menu.IsCache = dto.IsCache;
        menu.MenuNameKey = dto.MenuNameKey;
        menu.OrderNum = dto.OrderNum;
        menu.Visible = dto.Visible;
        menu.IsFrame = dto.IsFrame;
        menu.Status = dto.Status;

        await _menuRepository.UpdateAsync(menu);
    }

    public async Task DeleteAsync(long id)
    {
        // 检查是否有子菜单
        var hasChildren = await _menuRepository.AnyAsync(m => m.ParentId == id);
        if (hasChildren)
        {
            throw new Volo.Abp.UserFriendlyException("存在子菜单,不允许删除");
        }

        // 删除角色菜单关系
        await _roleMenuRepository.DeleteAsync(rm => rm.MenuId == id);

        // 删除菜单
        await _menuRepository.DeleteAsync(id);
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        // 1. 获取用户角色
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        if (!roleIds.Any())
        {
            return new List<string>();
        }

        // 2. 获取角色对应的菜单ID
        var roleMenus = await _roleMenuRepository.GetListAsync(rm => roleIds.Contains(rm.RoleId));
        var menuIds = roleMenus.Select(rm => rm.MenuId).Distinct().ToList();

        // 3. 获取菜单详情（仅获取有权限代码的菜单）
        var menus = await _menuRepository.GetListAsync(m => 
            menuIds.Contains(m.Id) && 
            m.Status == "0" &&
            !string.IsNullOrEmpty(m.PermissionCode));

        // 4. 提取权限代码
        var permissions = menus
            .Where(m => !string.IsNullOrEmpty(m.PermissionCode))
            .Select(m => m.PermissionCode!)
            .Distinct()
            .ToList();

        return permissions;
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    private List<MenuDto> BuildTree(List<MenuDto> menus, long? parentId)
    {
        var children = menus.Where(m => m.ParentId == parentId).ToList();
        
        foreach (var child in children)
        {
            child.Children = BuildTree(menus, child.MenuId);
            if (child.Children.Count == 0)
            {
                child.Children = null;
            }
        }

        return children;
    }

    /// <summary>
    /// 构建路由
    /// </summary>
    private List<RouterDto> BuildRouters(List<MenuDto> menus)
    {
        var routers = new List<RouterDto>();

        foreach (var menu in menus)
        {
            var router = new RouterDto
            {
                Name = GetRouteName(menu),
                Path = GetRouterPath(menu),
                Hidden = !menu.Visible,
                Component = GetComponent(menu),
                Meta = new RouterMetaDto
                {
                    Title = menu.MenuName,
                    Icon = menu.Icon,
                    NoCache = false,
                    Link = menu.IsFrame ? menu.Path : null
                }
            };

            // 处理子菜单
            var children = menu.Children;
            if (children != null && children.Any() && menu.MenuType == "M")
            {
                router.AlwaysShow = true;
                router.Redirect = "noRedirect";
                router.Children = BuildRouters(children);
            }
            else if (IsMenuFrame(menu))
            {
                router.Meta = null!;
                var childrenList = new List<RouterDto>();
                var childRouter = new RouterDto
                {
                    Path = menu.Path!,
                    Component = menu.Component!,
                    Name = StringFirstToUpper(menu.Path!),
                    Meta = new RouterMetaDto
                    {
                        Title = menu.MenuName,
                        Icon = menu.Icon,
                        NoCache = false,
                        Link = menu.IsFrame ? menu.Path : null
                    }
                };
                childrenList.Add(childRouter);
                router.Children = childrenList;
            }

            routers.Add(router);
        }

        return routers;
    }

    /// <summary>
    /// 获取路由名称
    /// </summary>
    private string GetRouteName(MenuDto menu)
    {
        var routerName = StringFirstToUpper(menu.Path ?? "");
        // 非外链并且是一级目录（类型为目录）
        if (IsMenuFrame(menu))
        {
            routerName = string.Empty;
        }
        return routerName;
    }

    /// <summary>
    /// 获取路由地址
    /// </summary>
    private string GetRouterPath(MenuDto menu)
    {
        var routerPath = menu.Path ?? "";
        // 非外链并且是一级目录（类型为目录）
        if (menu.ParentId == null && menu.MenuType == "M" && !menu.IsFrame)
        {
            routerPath = "/" + menu.Path;
        }
        // 非外链并且是一级目录（类型为菜单）
        else if (IsMenuFrame(menu))
        {
            routerPath = "/";
        }
        return routerPath;
    }

    /// <summary>
    /// 获取组件信息
    /// </summary>
    private string GetComponent(MenuDto menu)
    {
        var component = "Layout";
        if (!string.IsNullOrEmpty(menu.Component) && !IsMenuFrame(menu))
        {
            component = menu.Component;
        }
        else if (string.IsNullOrEmpty(menu.Component) && menu.ParentId != null && IsInnerLink(menu))
        {
            component = "InnerLink";
        }
        else if (string.IsNullOrEmpty(menu.Component) && IsParentView(menu))
        {
            component = "ParentView";
        }
        return component;
    }

    /// <summary>
    /// 是否为菜单内部跳转
    /// </summary>
    private bool IsMenuFrame(MenuDto menu)
    {
        return menu.ParentId == null && menu.MenuType == "C" && !menu.IsFrame;
    }

    /// <summary>
    /// 是否为内链组件
    /// </summary>
    private bool IsInnerLink(MenuDto menu)
    {
        return !menu.IsFrame && !string.IsNullOrEmpty(menu.Path) && menu.Path.StartsWith("http");
    }

    /// <summary>
    /// 是否为parent_view组件
    /// </summary>
    private bool IsParentView(MenuDto menu)
    {
        return menu.ParentId != null && menu.MenuType == "M";
    }

    /// <summary>
    /// 首字母大写
    /// </summary>
    private string StringFirstToUpper(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    /// <summary>
    /// 映射到DTO
    /// </summary>
    private MenuDto MapToDto(Menu menu)
    {
        return new MenuDto
        {
            MenuId = menu.Id,
            MenuName = menu.MenuName,
            ParentId = menu.ParentId,
            MenuType = menu.MenuType,
            Path = menu.Path,
            Component = menu.Component,
            PermissionCode = menu.PermissionCode,
            Icon = menu.Icon,
            Query = menu.Query,
            IsCache = menu.IsCache,
            MenuNameKey = menu.MenuNameKey,
            OrderNum = menu.OrderNum,
            Visible = menu.Visible,
            IsFrame = menu.IsFrame,
            Status = menu.Status,
            CreationTime = menu.CreationTime
        };
    }
}

