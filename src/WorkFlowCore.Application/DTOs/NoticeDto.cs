using Volo.Abp.Application.Dtos;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 通知公告DTO
/// </summary>
public class NoticeDto : FullAuditedEntityDto<long>
{
    /// <summary>
    /// 公告ID
    /// </summary>
    public long NoticeId { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型
    /// </summary>
    public int NoticeType { get; set; }

    /// <summary>
    /// 公告内容
    /// </summary>
    public string? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 发布人
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 是否弹出
    /// </summary>
    public int Popup { get; set; }
}

/// <summary>
/// 通知公告查询DTO
/// </summary>
public class NoticeQueryDto : PagedRequest
{
    /// <summary>
    /// 公告标题
    /// </summary>
    public string? NoticeTitle { get; set; }

    /// <summary>
    /// 公告类型
    /// </summary>
    public int? NoticeType { get; set; }

    /// <summary>
    /// 发布人
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

