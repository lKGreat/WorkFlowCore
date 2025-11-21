namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 启动流程请求
/// </summary>
public class StartWorkflowRequest
{
    /// <summary>
    /// 流程定义Key
    /// </summary>
    public string WorkflowId { get; set; } = string.Empty;

    /// <summary>
    /// 流程版本（可选，默认最新版本）
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
    /// 流程数据
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// 引用键（业务键）
    /// </summary>
    public string? Reference { get; set; }
}

/// <summary>
/// 完成任务请求
/// </summary>
public class CompleteTaskRequest
{
    /// <summary>
    /// 工作流实例ID
    /// </summary>
    public string WorkflowId { get; set; } = string.Empty;

    /// <summary>
    /// 步骤ID
    /// </summary>
    public string StepId { get; set; } = string.Empty;

    /// <summary>
    /// 审批意见
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 审批结果
    /// </summary>
    public bool Approved { get; set; }
}

/// <summary>
/// 工作流实例DTO
/// </summary>
public class WorkflowInstanceDto
{
    public string Id { get; set; } = string.Empty;
    public string WorkflowDefinitionId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string? Reference { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreateTime { get; set; }
    public DateTime? CompleteTime { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

