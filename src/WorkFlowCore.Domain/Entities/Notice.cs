using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 通知公告实体
/// </summary>
public class Notice : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 公告标题
    /// </summary>
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型 (1=通知, 2=公告)
    /// </summary>
    public int NoticeType { get; set; } = 1;

    /// <summary>
    /// 公告内容
    /// </summary>
    public string? NoticeContent { get; set; }

    /// <summary>
    /// 公告状态 (0=正常, 1=关闭)
    /// </summary>
    public int Status { get; set; } = 0;

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
    /// 是否弹出提示
    /// </summary>
    public int Popup { get; set; } = 0;

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected Notice() { }

    public Notice(long id, Guid? tenantId, string noticeTitle) : base(id)
    {
        TenantId = tenantId;
        NoticeTitle = noticeTitle;
    }
}

