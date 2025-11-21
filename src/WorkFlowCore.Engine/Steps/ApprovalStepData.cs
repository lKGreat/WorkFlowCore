namespace WorkFlowCore.Engine.Steps;

/// <summary>
/// 审批步骤数据
/// </summary>
public class ApprovalStepData
{
    /// <summary>
    /// 审批人ID
    /// </summary>
    public Guid? ApproverId { get; set; }

    /// <summary>
    /// 审批意见
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 审批结果（批准/拒绝）
    /// </summary>
    public bool? Approved { get; set; }

    /// <summary>
    /// 审批时间
    /// </summary>
    public DateTime? ApprovalTime { get; set; }
}

