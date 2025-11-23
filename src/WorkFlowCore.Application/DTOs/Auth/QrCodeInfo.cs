namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 二维码登录信息
/// </summary>
public class QrCodeInfo
{
    /// <summary>
    /// 二维码UUID
    /// </summary>
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// 状态标识
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// 二维码内容
    /// </summary>
    public string QrContent { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime ExpireTime { get; set; }
}

