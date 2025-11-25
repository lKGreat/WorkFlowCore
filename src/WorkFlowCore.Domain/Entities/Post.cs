using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 岗位实体
/// </summary>
public class Post : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 岗位编码
    /// </summary>
    public string PostCode { get; set; } = string.Empty;

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string PostName { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int PostSort { get; set; }

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected Post() { }

    public Post(long id, Guid? tenantId, string postCode, string postName) : base(id)
    {
        TenantId = tenantId;
        PostCode = postCode;
        PostName = postName;
    }
}

