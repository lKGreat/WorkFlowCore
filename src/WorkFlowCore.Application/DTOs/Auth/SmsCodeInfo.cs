namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 短信验证码类型
/// </summary>
public enum SmsCodeType
{
    /// <summary>
    /// 登录
    /// </summary>
    Login = 0,

    /// <summary>
    /// 注册
    /// </summary>
    Register = 1,

    /// <summary>
    /// 重置密码
    /// </summary>
    ResetPassword = 2,

    /// <summary>
    /// 绑定手机号
    /// </summary>
    BindPhone = 3
}

/// <summary>
/// 短信验证码缓存项
/// </summary>
public class SmsCodeCacheItem
{
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 验证码类型
    /// </summary>
    public SmsCodeType Type { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
}

/// <summary>
/// 发送短信验证码请求
/// </summary>
public class SendSmsCodeRequest
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 验证码类型
    /// </summary>
    public SmsCodeType Type { get; set; }
}

