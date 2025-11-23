using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using WorkFlowCore.Application.DTOs.Role;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 角色服务实现
/// </summary>
public class RoleService : ApplicationService, IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IRepository<RoleMenu, long> _roleMenuRepository;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        IRepository<RoleMenu, long> roleMenuRepository)
    {
        _roleManager = roleManager;
        _roleMenuRepository = roleMenuRepository;
    }

    public async Task<PagedResponse<RoleDto>> GetPagedAsync(PagedRequest request)
    {
        var query = _roleManager.Roles.AsQueryable();

        var total = await query.CountAsync();
        var roles = await query
            .OrderBy(r => r.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // 批量获取角色菜单关系（性能优化）
        var roleIds = roles.Select(r => r.Id).ToList();
        var roleMenus = await _roleMenuRepository
            .GetListAsync(rm => roleIds.Contains(rm.RoleId));
        var roleMenuDict = roleMenus
            .GroupBy(rm => rm.RoleId)
            .ToDictionary(g => g.Key, g => g.Select(rm => rm.MenuId).ToList());

        var items = roles.Select(r => new RoleDto
        {
            RoleId = r.Id.ToString(),
            RoleName = r.Name!,
            RoleKey = r.Name,
            RoleSort = 0,
            Status = "0",
            CreationTime = DateTime.Now,
            MenuIds = roleMenuDict.ContainsKey(r.Id) ? roleMenuDict[r.Id] : new List<long>()
        }).ToList();

        return new PagedResponse<RoleDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return null;

        var menuIds = await GetRoleMenuIdsAsync(id);

        return new RoleDto
        {
            RoleId = role.Id.ToString(),
            RoleName = role.Name!,
            RoleKey = role.Name,
            RoleSort = 0,
            Status = "0",
            CreationTime = DateTime.Now,
            MenuIds = menuIds
        };
    }

    public async Task<RoleDto> CreateAsync(CreateRoleInput input)
    {
        var role = new IdentityRole(Guid.NewGuid(), input.RoleName, CurrentTenant.Id);
        var result = await _roleManager.CreateAsync(role);
        
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException($"创建角色失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // 分配菜单
        if (input.MenuIds.Any())
        {
            await AssignMenusAsync(role.Id, input.MenuIds);
        }

        return (await GetByIdAsync(role.Id))!;
    }

    public async Task UpdateAsync(UpdateRoleInput input)
    {
        var roleId = Guid.Parse(input.RoleId);
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        
        if (role == null)
        {
            throw new Volo.Abp.UserFriendlyException("角色不存在");
        }

        await _roleManager.SetRoleNameAsync(role, input.RoleName);
        
        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException($"更新角色失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task DeleteAsync(List<Guid> ids)
    {
        // 批量查询角色（性能优化）
        var roles = await _roleManager.Roles
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();

        // 批量删除
        foreach (var role in roles)
        {
            await _roleManager.DeleteAsync(role);
        }

        // 删除角色菜单关系
        await _roleMenuRepository.DeleteAsync(rm => ids.Contains(rm.RoleId));
    }

    public async Task<List<RoleOption>> GetOptionsAsync()
    {
        var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        return roles.Select(r => new RoleOption
        {
            RoleId = r.Id.ToString(),
            RoleName = r.Name!
        }).ToList();
    }

    public async Task AssignMenusAsync(Guid roleId, List<long> menuIds)
    {
        // 删除旧的菜单关系
        await _roleMenuRepository.DeleteAsync(rm => rm.RoleId == roleId);

        // 批量插入新的菜单关系（性能优化）
        var roleMenus = menuIds.Select(menuId => new RoleMenu(
            SnowflakeIdGenerator.NextId(),
            roleId,
            menuId
        )).ToList();

        await _roleMenuRepository.InsertManyAsync(roleMenus);
    }

    public async Task<List<long>> GetRoleMenuIdsAsync(Guid roleId)
    {
        var roleMenus = await _roleMenuRepository.GetListAsync(rm => rm.RoleId == roleId);
        return roleMenus.Select(rm => rm.MenuId).ToList();
    }
}

