using global::WorkflowCore.Interface;
using global::WorkflowCore.Models;

namespace WorkFlowCore.Engine;

/// <summary>
/// 工作流引擎服务实现
/// </summary>
public class WorkflowEngineService : IWorkflowEngine
{
    private readonly IWorkflowHost _workflowHost;
    private readonly IWorkflowController _workflowController;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="workflowHost">WorkflowCore 主机</param>
    /// <param name="workflowController">WorkflowCore 控制器</param>
    public WorkflowEngineService(
        IWorkflowHost workflowHost,
        IWorkflowController workflowController)
    {
        _workflowHost = workflowHost;
        _workflowController = workflowController;
    }

    /// <summary>
    /// 启动流程实例
    /// </summary>
    /// <param name="definitionId">流程定义ID</param>
    /// <param name="variables">流程变量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>流程实例ID</returns>
    public async Task<string> StartProcessAsync(
        string definitionId,
        Dictionary<string, object> variables,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(definitionId))
            throw new ArgumentException("流程定义ID不能为空", nameof(definitionId));

        // 将变量字典转换为动态对象
        var data = ConvertToDynamic(variables);

        // 启动工作流实例
        var instanceId = await _workflowHost.StartWorkflow<object>(
            definitionId,
            null,
            data);

        if (string.IsNullOrEmpty(instanceId))
            throw new InvalidOperationException($"启动流程失败: 流程定义ID '{definitionId}' 未找到");

        return instanceId;
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="variables">任务变量</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task CompleteTaskAsync(
        string taskId,
        Dictionary<string, object> variables,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(taskId))
            throw new ArgumentException("任务ID不能为空", nameof(taskId));

        // 通过发布事件来完成任务
        // WorkflowCore 使用事件机制来唤醒等待中的步骤
        await _workflowController.PublishEvent(
            eventName: $"TaskCompleted_{taskId}",
            eventKey: taskId,
            eventData: variables,
            effectiveDate: DateTime.UtcNow);
    }

    /// <summary>
    /// 终止流程实例
    /// </summary>
    /// <param name="instanceId">流程实例ID</param>
    /// <param name="reason">终止原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task TerminateProcessAsync(
        string instanceId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(instanceId))
            throw new ArgumentException("流程实例ID不能为空", nameof(instanceId));

        // 终止工作流实例
        var terminated = await _workflowHost.TerminateWorkflow(instanceId);

        if (!terminated)
            throw new InvalidOperationException($"终止流程失败: 流程实例ID '{instanceId}' 未找到或已终止");
    }

    /// <summary>
    /// 将字典转换为动态对象
    /// </summary>
    /// <param name="variables">变量字典</param>
    /// <returns>动态对象</returns>
    private static dynamic ConvertToDynamic(Dictionary<string, object>? variables)
    {
        if (variables == null || variables.Count == 0)
            return new { };

        // 使用 ExpandoObject 或直接返回字典
        return variables;
    }
}

