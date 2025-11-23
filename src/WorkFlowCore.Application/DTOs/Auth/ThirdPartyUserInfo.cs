namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 第三方用户信息
/// </summary>
public class ThirdPartyUserInfo
{
    /// <summary>
    /// 提供商
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// OpenId
    /// </summary>
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// UnionId
    /// </summary>
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 令牌过期时间
    /// </summary>
    public DateTime? TokenExpireTime { get; set; }
}

