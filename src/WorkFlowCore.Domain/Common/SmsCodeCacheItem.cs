namespace WorkFlowCore.Domain.Common;

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
    /// 短信类型
    /// </summary>
    public SmsCodeType Type { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 短信验证码类型
/// </summary>
public enum SmsCodeType
{
    /// <summary>
    /// 登录验证码
    /// </summary>
    Login,

    /// <summary>
    /// 注册验证码
    /// </summary>
    Register,

    /// <summary>
    /// 重置密码
    /// </summary>
    ResetPassword,

    /// <summary>
    /// 绑定手机号
    /// </summary>
    BindPhone
}

