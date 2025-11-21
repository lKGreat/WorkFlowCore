using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : Entity<Guid>, ITenantEntity, ISoftDelete
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
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 直属上级ID
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

