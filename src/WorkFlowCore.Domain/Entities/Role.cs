using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class Role : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }
    
    protected Role() { }

    public Role(Guid id, Guid? tenantId, string name, string code) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Code = code;
    }
}
