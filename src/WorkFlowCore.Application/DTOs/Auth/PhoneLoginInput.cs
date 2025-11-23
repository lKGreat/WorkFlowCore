namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 手机号登录输入
/// </summary>
public class PhoneLoginInput
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 短信验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

