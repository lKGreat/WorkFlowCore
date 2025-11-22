namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 验证码信息
/// </summary>
public class CaptchaInfo
{
    /// <summary>
    /// 验证码UUID
    /// </summary>
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// Base64编码的验证码图片
    /// </summary>
    public string ImageBase64 { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime ExpireTime { get; set; }
}

/// <summary>
/// 验证码缓存项
/// </summary>
public class CaptchaCacheItem
{
    /// <summary>
    /// 验证码内容
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

