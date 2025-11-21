using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 租户实体
/// </summary>
public class Tenant : Entity<Guid>, ISoftDelete
{
    /// <summary>
    /// 租户名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 租户编码（唯一）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

