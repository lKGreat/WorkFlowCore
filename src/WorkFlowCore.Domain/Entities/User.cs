using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 用户名（登录用）
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 所属部门ID
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 直属上级ID
    /// </summary>
    public long? ManagerId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 关联的 ABP Identity 用户ID
    /// </summary>
    public Guid? AbpUserId { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected User() { }

    public User(long id, Guid? tenantId, string userName, string realName) 
        : base(id)
    {
        TenantId = tenantId;
        UserName = userName;
        RealName = realName;
    }
}
