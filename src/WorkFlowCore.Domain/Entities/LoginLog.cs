using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 登录日志实体
/// </summary>
public class LoginLog : CreationAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 登录状态 (0=成功, 1=失败)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 登录IP地址
    /// </summary>
    public string Ipaddr { get; set; } = string.Empty;

    /// <summary>
    /// 登录地点
    /// </summary>
    public string LoginLocation { get; set; } = string.Empty;

    /// <summary>
    /// 浏览器类型
    /// </summary>
    public string Browser { get; set; } = string.Empty;

    /// <summary>
    /// 操作系统
    /// </summary>
    public string Os { get; set; } = string.Empty;

    /// <summary>
    /// 提示消息
    /// </summary>
    public string Msg { get; set; } = string.Empty;

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 客户端ID（设备标识）
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected LoginLog() { }

    public LoginLog(long id, Guid? tenantId, string userName) : base(id)
    {
        TenantId = tenantId;
        UserName = userName;
        LoginTime = DateTime.Now;
    }
}

