using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 任务实例实体
/// </summary>
public class TaskInstance : AuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 流程实例ID
    /// </summary>
    public Guid ProcessInstanceId { get; set; }

    /// <summary>
    /// 任务节点ID
    /// </summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// 任务名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 任务类型（用户任务、系统任务等）
    /// </summary>
    public string TaskType { get; set; } = "UserTask";

    /// <summary>
    /// 受理人ID
    /// </summary>
    public Guid? AssigneeId { get; set; }

    /// <summary>
    /// 候选人ID列表（JSON数组）
    /// </summary>
    public string? CandidateUsers { get; set; }

    /// <summary>
    /// 候选组ID列表（JSON数组）
    /// </summary>
    public string? CandidateGroups { get; set; }

    /// <summary>
    /// 任务状态（待处理、已完成、已跳过）
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// 到期时间
    /// </summary>
    public DateTime? DueDate { get; set; }

    // CreationTime 从基类继承，移除原 CreateTime

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 任务变量（JSON）
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// 审批意见
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }
    
    protected TaskInstance() { }
    
    public TaskInstance(Guid id, Guid? tenantId, Guid processInstanceId, string name, string nodeId) : base(id)
    {
        TenantId = tenantId;
        ProcessInstanceId = processInstanceId;
        Name = name;
        NodeId = nodeId;
    }
}
