namespace WorkFlowCore.Application.DTOs.User;

/// <summary>
/// 重置密码输入
/// </summary>
public class ResetPasswordInput
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}

