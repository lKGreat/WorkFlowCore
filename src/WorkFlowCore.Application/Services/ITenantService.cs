using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 租户服务接口
/// </summary>
public interface ITenantService
{
    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<List<TenantDto>> GetAllAsync();
    Task<PagedResponse<TenantDto>> GetPagedAsync(PagedRequest request);
    Task<TenantDto> CreateAsync(TenantDto dto);
    Task UpdateAsync(TenantDto dto);
    Task DeleteAsync(Guid id);
}
