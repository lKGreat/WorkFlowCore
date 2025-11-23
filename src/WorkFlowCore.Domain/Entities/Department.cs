using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 部门实体
/// </summary>
public class Department : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 父级部门ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 祖级列表 (如: "0,1,2")
    /// </summary>
    public string Ancestors { get; set; } = "0";

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    /// <summary>
    /// 部门用户集合
    /// </summary>
    public virtual ICollection<AppUser>? Users { get; set; }

    protected Department() { }

    public Department(long id, Guid? tenantId, string deptName) : base(id)
    {
        TenantId = tenantId;
        DeptName = deptName;
    }
}
