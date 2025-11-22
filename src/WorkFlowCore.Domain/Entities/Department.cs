using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 部门实体
/// </summary>
public class Department : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 父级部门ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 部门主管ID
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected Department() { }

    public Department(Guid id, Guid? tenantId, string name) : base(id)
    {
        TenantId = tenantId;
        Name = name;
    }
}
