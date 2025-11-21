using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Engine;

/// <summary>
/// 流程实例管理器接口
/// </summary>
public interface IProcessInstanceManager
{
    /// <summary>
    /// 获取流程实例
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<ProcessInstance?> GetInstanceAsync(Guid instanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建流程实例
    /// </summary>
    /// <param name="definitionId">流程定义ID</param>
    /// <param name="initiatorId">发起人ID</param>
    /// <param name="variables">流程变量</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<ProcessInstance> CreateInstanceAsync(Guid definitionId, Guid initiatorId, Dictionary<string, object> variables, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新流程实例状态
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <param name="status">新状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task UpdateInstanceStatusAsync(Guid instanceId, string status, CancellationToken cancellationToken = default);
}

