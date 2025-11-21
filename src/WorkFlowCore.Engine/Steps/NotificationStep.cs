using global::WorkflowCore.Interface;
using global::WorkflowCore.Models;

namespace WorkFlowCore.Engine.Steps;

/// <summary>
/// 通知步骤
/// </summary>
public class NotificationStep : StepBodyAsync
{
    /// <summary>
    /// 通知标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 通知内容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 接收人ID列表
    /// </summary>
    public List<Guid>? RecipientIds { get; set; }

    /// <summary>
    /// 通知类型（邮件、短信、站内信等）
    /// </summary>
    public string NotificationType { get; set; } = "InApp";

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        // TODO: 实现实际的通知发送逻辑
        // 这里可以调用邮件服务、短信服务、钉钉、企业微信等
        
        // 模拟发送通知
        Console.WriteLine($"发送通知: {Title} - {Message}");
        Console.WriteLine($"接收人数: {RecipientIds?.Count ?? 0}");
        
        await Task.CompletedTask;
        return ExecutionResult.Next();
    }
}

