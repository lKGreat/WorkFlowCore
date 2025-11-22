using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 租户服务接口
/// </summary>
public interface ITenantService : ICrudAppService<TenantDto, Guid, PagedAndSortedResultRequestDto, TenantDto, TenantDto>
{
    Task<List<TenantDto>> GetAllAsync();
    Task<PagedResponse<TenantDto>> GetPagedAsync(PagedRequest request);
}
