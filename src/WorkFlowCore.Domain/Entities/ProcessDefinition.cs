using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 流程定义实体
/// </summary>
public class ProcessDefinition : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码（唯一标识）
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 流程定义内容（BPMN XML 或 JSON）
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 内容格式（BPMN、JSON）
    /// </summary>
    public string ContentFormat { get; set; } = "JSON";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }
    
    protected ProcessDefinition() { }

    public ProcessDefinition(long id, Guid? tenantId, string name, string key, string content) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Key = key;
        Content = content;
    }
}
