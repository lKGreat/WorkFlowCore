using WorkFlowCore.Domain.Common;

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
    /// 短信类型
    /// </summary>
    public SmsCodeType Type { get; set; }
}

