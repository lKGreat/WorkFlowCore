using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs.Role;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService : IApplicationService
{
    Task<PagedResponse<RoleDto>> GetPagedAsync(PagedRequest request);
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<RoleDto> CreateAsync(CreateRoleInput input);
    Task UpdateAsync(UpdateRoleInput input);
    Task DeleteAsync(List<Guid> ids);
    Task<List<RoleOption>> GetOptionsAsync();
    Task AssignMenusAsync(Guid roleId, List<long> menuIds);
    Task<List<long>> GetRoleMenuIdsAsync(Guid roleId);
}

