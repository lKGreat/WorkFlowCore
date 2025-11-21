using global::WorkflowCore.Interface;
using global::WorkflowCore.Models;

namespace WorkFlowCore.Engine.Steps;

/// <summary>
/// 审批任务步骤
/// </summary>
public class ApprovalStep : StepBodyAsync
{
    /// <summary>
    /// 任务标题
    /// </summary>
    public string TaskTitle { get; set; } = string.Empty;

    /// <summary>
    /// 审批人ID（可以是表达式）
    /// </summary>
    public Guid? ApproverId { get; set; }

    /// <summary>
    /// 候选审批人ID列表
    /// </summary>
    public List<Guid>? CandidateApprovers { get; set; }

    /// <summary>
    /// 审批结果输出
    /// </summary>
    public bool? Approved { get; set; }

    /// <summary>
    /// 审批意见输出
    /// </summary>
    public string? Comment { get; set; }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var persistenceData = context.PersistenceData as IDictionary<string, object>;
        if (persistenceData == null)
        {
            return ExecutionResult.Next();
        }

        // 创建审批任务（暂挂，等待外部完成）
        if (!persistenceData.ContainsKey("TaskCreated"))
        {
            // 第一次执行：创建任务
            persistenceData["TaskCreated"] = true;
            persistenceData["TaskId"] = Guid.NewGuid().ToString();
            
            // TODO: 在这里应该创建 TaskInstance 记录到数据库
            
            // 返回持久化等待，等待外部完成任务
            return ExecutionResult.Persist(null);
        }

        // 任务已被外部完成，读取结果
        if (persistenceData.ContainsKey("Approved"))
        {
            Approved = bool.Parse(persistenceData["Approved"].ToString()!);
            Comment = persistenceData.ContainsKey("Comment") 
                ? persistenceData["Comment"].ToString() 
                : null;
            
            return ExecutionResult.Next();
        }

        // 继续等待
        await Task.CompletedTask;
        return ExecutionResult.Persist(null);
    }
}

