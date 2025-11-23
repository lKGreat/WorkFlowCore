using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 二维码登录结果
/// </summary>
public class QrCodeLoginResult
{
    /// <summary>
    /// 二维码状态
    /// </summary>
    public QrCodeStatus Status { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public int? ExpiresIn { get; set; }
}

