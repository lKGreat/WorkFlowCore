namespace WorkFlowCore.Engine;

/// <summary>
/// 工作流引擎接口
/// </summary>
public interface IWorkflowEngine
{
    /// <summary>
    /// 启动流程实例
    /// </summary>
    /// <param name="definitionId">流程定义ID</param>
    /// <param name="variables">流程变量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>流程实例ID</returns>
    Task<string> StartProcessAsync(string definitionId, Dictionary<string, object> variables, CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="variables">任务变量</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task CompleteTaskAsync(string taskId, Dictionary<string, object> variables, CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止流程实例
    /// </summary>
    /// <param name="instanceId">流程实例ID</param>
    /// <param name="reason">终止原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task TerminateProcessAsync(string instanceId, string reason, CancellationToken cancellationToken = default);
}

