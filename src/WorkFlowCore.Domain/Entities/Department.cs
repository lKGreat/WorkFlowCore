using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 部门实体
/// </summary>
public class Department : Entity<Guid>, ITenantEntity, ISoftDelete
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

