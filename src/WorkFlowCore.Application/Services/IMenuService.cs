using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 菜单服务接口
/// </summary>
public interface IMenuService : IApplicationService
{
    /// <summary>
    /// 获取所有菜单
    /// </summary>
    Task<List<MenuDto>> GetListAsync();

    /// <summary>
    /// 获取菜单树
    /// </summary>
    Task<List<MenuDto>> GetTreeAsync();

    /// <summary>
    /// 根据角色获取路由
    /// </summary>
    Task<List<RouterDto>> GetRoutersByRoleAsync(Guid userId);

    /// <summary>
    /// 创建菜单
    /// </summary>
    Task<MenuDto> CreateAsync(MenuDto dto);

    /// <summary>
    /// 更新菜单
    /// </summary>
    Task UpdateAsync(MenuDto dto);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 获取用户权限列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>权限代码列表</returns>
    Task<List<string>> GetUserPermissionsAsync(Guid userId);
}

