using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class Role : FullAuditedAggregateRoot<long>, IMultiTenant
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
    /// 数据权限范围（1=全部数据,2=自定义,3=本部门,4=本部门及以下,5=项目,6=仅本人）
    /// </summary>
    public int DataScope { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int RoleSort { get; set; }

    /// <summary>
    /// 状态（0=正常,1=停用）
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }
    
    protected Role() { }

    public Role(long id, Guid? tenantId, string name, string code) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Code = code;
    }
}
