namespace WorkFlowCore.Engine.Models;

/// <summary>
/// 流程定义模型
/// </summary>
public class ProcessDefinitionModel
{
    /// <summary>
    /// 流程定义ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 流程名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 流程节点列表
    /// </summary>
    public List<ProcessNode> Nodes { get; set; } = new();

    /// <summary>
    /// 流程连线列表
    /// </summary>
    public List<ProcessEdge> Edges { get; set; } = new();
}

/// <summary>
/// 流程节点
/// </summary>
public class ProcessNode
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 节点类型（StartEvent, UserTask, EndEvent等）
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 节点配置（JSON）
    /// </summary>
    public Dictionary<string, object> Config { get; set; } = new();
}

/// <summary>
/// 流程连线
/// </summary>
public class ProcessEdge
{
    /// <summary>
    /// 连线ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 源节点ID
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// 目标节点ID
    /// </summary>
    public string TargetId { get; set; } = string.Empty;

    /// <summary>
    /// 条件表达式（可选）
    /// </summary>
    public string? Condition { get; set; }
}

