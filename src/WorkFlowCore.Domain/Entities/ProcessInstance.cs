using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 流程实例实体
/// </summary>
public class ProcessInstance : AuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 流程定义ID
    /// </summary>
    public long ProcessDefinitionId { get; set; }

    /// <summary>
    /// 业务键（关联业务数据）
    /// </summary>
    public string? BusinessKey { get; set; }

    /// <summary>
    /// 流程标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 发起人ID
    /// </summary>
    public long InitiatorId { get; set; }

    /// <summary>
    /// 当前状态（运行中、已完成、已终止）
    /// </summary>
    public string Status { get; set; } = "Running";

    /// <summary>
    /// 流程变量（JSON）
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// 开始时间 (可以使用 CreationTime，但为了兼容保留业务含义)
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }
    
    protected ProcessInstance() { }

    public ProcessInstance(long id, Guid? tenantId, long processDefinitionId) : base(id)
    {
        TenantId = tenantId;
        ProcessDefinitionId = processDefinitionId;
    }
}
