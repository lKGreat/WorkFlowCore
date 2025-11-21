namespace WorkFlowCore.Engine;

/// <summary>
/// 任务分配器接口
/// </summary>
public interface ITaskAllocator
{
    /// <summary>
    /// 分配任务给用户
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task AssignTaskAsync(string taskId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据候选规则计算候选人
    /// </summary>
    /// <param name="candidateRule">候选人规则</param>
    /// <param name="variables">流程变量</param>
    /// <returns>候选人ID列表</returns>
    Task<List<Guid>> CalculateCandidatesAsync(string candidateRule, Dictionary<string, object> variables);
}

