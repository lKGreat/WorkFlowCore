using global::WorkflowCore.Interface;
using global::WorkflowCore.Models;
using WorkFlowCore.Engine.Steps;

namespace WorkFlowCore.Engine.Workflows;

/// <summary>
/// 简单审批工作流示例
/// </summary>
public class SimpleApprovalWorkflow : IWorkflow<ApprovalWorkflowData>
{
    public string Id => "SimpleApproval";
    public int Version => 1;

    public void Build(IWorkflowBuilder<ApprovalWorkflowData> builder)
    {
        builder
            .StartWith<NotificationStep>()
                .Input(step => step.Title, data => "审批通知")
                .Input(step => step.Message, data => $"新的审批申请: {data.Title}")
            .Then<ApprovalStep>()
                .Input(step => step.TaskTitle, data => data.Title)
                .Input(step => step.ApproverId, data => data.ApproverId)
                .Output(data => data.Approved, step => step.Approved ?? false)
                .Output(data => data.ApprovalComment, step => step.Comment ?? "")
            .Then<NotificationStep>()
                .Input(step => step.Title, data => "审批完成")
                .Input(step => step.Message, data => data.Approved ? "审批已通过" : "审批已拒绝");
    }
}

/// <summary>
/// 审批工作流数据
/// </summary>
public class ApprovalWorkflowData
{
    /// <summary>
    /// 申请标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 审批人ID
    /// </summary>
    public Guid ApproverId { get; set; }

    /// <summary>
    /// 审批结果
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// 审批意见
    /// </summary>
    public string ApprovalComment { get; set; } = string.Empty;
}

