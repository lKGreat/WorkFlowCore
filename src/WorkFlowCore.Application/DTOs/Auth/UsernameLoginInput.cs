namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 用户名密码登录输入
/// </summary>
public class UsernameLoginInput
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 图形验证码UUID
    /// </summary>
    public string CaptchaUuid { get; set; } = string.Empty;

    /// <summary>
    /// 图形验证码
    /// </summary>
    public string CaptchaCode { get; set; } = string.Empty;

    /// <summary>
    /// 记住我
    /// </summary>
    public bool RememberMe { get; set; }
}

