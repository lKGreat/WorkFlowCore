namespace WorkFlowCore.Application.DTOs.Auth;

/// <summary>
/// 第三方登录结果
/// </summary>
public class ThirdPartyLoginResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 是否需要绑定账号
    /// </summary>
    public bool NeedBind { get; set; }

    /// <summary>
    /// 用户信息（已绑定时返回）
    /// </summary>
    public WorkFlowCore.Domain.Identity.AppUser? User { get; set; }

    /// <summary>
    /// 第三方用户信息（未绑定时返回）
    /// </summary>
    public ThirdPartyUserInfo? ThirdPartyUser { get; set; }

    /// <summary>
    /// 临时Token（用于绑定流程）
    /// </summary>
    public string? TempToken { get; set; }

    /// <summary>
    /// 访问令牌(已绑定)
    /// </summary>
    public string? AccessToken { get; set; }
}

/// <summary>
/// 第三方用户信息
/// </summary>
public class ThirdPartyUserInfo
{
    public string Provider { get; set; } = string.Empty;
    public string OpenId { get; set; } = string.Empty;
    public string? UnionId { get; set; }
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpireTime { get; set; }
}

/// <summary>
/// 绑定账号请求
/// </summary>
public class BindAccountRequest
{
    /// <summary>
    /// 临时Token
    /// </summary>
    public string TempToken { get; set; } = string.Empty;

    /// <summary>
    /// 用户名（可选，如果提供则绑定到指定账号）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 密码（如果提供UserName则必填）
    /// </summary>
    public string? Password { get; set; }
}

