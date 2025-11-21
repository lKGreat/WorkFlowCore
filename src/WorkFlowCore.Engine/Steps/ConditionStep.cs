using global::WorkflowCore.Interface;
using global::WorkflowCore.Models;

namespace WorkFlowCore.Engine.Steps;

/// <summary>
/// 条件判断步骤
/// </summary>
public class ConditionStep : StepBodyAsync
{
    /// <summary>
    /// 条件表达式结果
    /// </summary>
    public bool ConditionResult { get; set; }

    /// <summary>
    /// 条件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        // 条件结果应该从工作流变量中计算得出
        // 这里只是简单返回
        await Task.CompletedTask;
        return ExecutionResult.Next();
    }
}

