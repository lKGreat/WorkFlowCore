namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 多租户实体接口
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// 租户ID
    /// </summary>
    Guid TenantId { get; set; }
}

