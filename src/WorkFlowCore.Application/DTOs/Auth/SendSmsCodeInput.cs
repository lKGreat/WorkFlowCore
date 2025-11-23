namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 发送短信验证码输入
/// </summary>
public class SendSmsCodeInput
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 短信类型 (0=登录, 1=注册, 2=重置密码, 3=绑定手机)
    /// </summary>
    public int Type { get; set; }
}

