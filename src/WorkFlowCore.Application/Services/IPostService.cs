using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 岗位服务接口
/// </summary>
public interface IPostService : IApplicationService
{
    /// <summary>
    /// 获取岗位分页列表
    /// </summary>
    Task<PagedResponse<PostDto>> GetPagedListAsync(PostQueryDto query);

    /// <summary>
    /// 获取所有岗位
    /// </summary>
    Task<List<PostDto>> GetAllAsync();

    /// <summary>
    /// 根据ID获取岗位
    /// </summary>
    Task<PostDto?> GetByIdAsync(long id);

    /// <summary>
    /// 创建岗位
    /// </summary>
    Task<PostDto> CreateAsync(PostDto dto);

    /// <summary>
    /// 更新岗位
    /// </summary>
    Task UpdateAsync(PostDto dto);

    /// <summary>
    /// 删除岗位
    /// </summary>
    Task DeleteAsync(long[] ids);

    /// <summary>
    /// 检查岗位名称是否唯一
    /// </summary>
    Task<bool> CheckPostNameUniqueAsync(string postName, long? excludeId = null);

    /// <summary>
    /// 检查岗位编码是否唯一
    /// </summary>
    Task<bool> CheckPostCodeUniqueAsync(string postCode, long? excludeId = null);
}

