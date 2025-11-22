using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 租户实体
/// </summary>
public class Tenant : FullAuditedAggregateRoot<long>
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
    
    protected Tenant() { }

    public Tenant(long id, string name, string code) : base(id)
    {
        Name = name;
        Code = code;
    }
}
