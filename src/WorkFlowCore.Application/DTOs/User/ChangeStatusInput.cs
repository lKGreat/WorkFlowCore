namespace WorkFlowCore.Application.DTOs.User;

/// <summary>
/// 更改状态输入
/// </summary>
public class ChangeStatusInput
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

