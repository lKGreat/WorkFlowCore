using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 通知公告服务接口
/// </summary>
public interface INoticeService : IApplicationService
{
    /// <summary>
    /// 获取通知公告分页列表
    /// </summary>
    Task<PagedResponse<NoticeDto>> GetPagedListAsync(NoticeQueryDto query);

    /// <summary>
    /// 根据ID获取通知公告
    /// </summary>
    Task<NoticeDto?> GetByIdAsync(long id);

    /// <summary>
    /// 创建通知公告
    /// </summary>
    Task<NoticeDto> CreateAsync(NoticeDto dto);

    /// <summary>
    /// 更新通知公告
    /// </summary>
    Task UpdateAsync(NoticeDto dto);

    /// <summary>
    /// 删除通知公告
    /// </summary>
    Task DeleteAsync(long[] ids);

    /// <summary>
    /// 获取当前有效的弹出公告
    /// </summary>
    Task<List<NoticeDto>> GetPopupNoticesAsync();
}

