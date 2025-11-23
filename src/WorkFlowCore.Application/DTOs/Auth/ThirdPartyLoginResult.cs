using WorkFlowCore.Domain.Identity;

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
    /// 用户信息(已绑定)
    /// </summary>
    public AppUser? User { get; set; }

    /// <summary>
    /// 第三方用户信息(未绑定)
    /// </summary>
    public ThirdPartyUserInfo? ThirdPartyUser { get; set; }

    /// <summary>
    /// 临时令牌(用于绑定)
    /// </summary>
    public string? TempToken { get; set; }

    /// <summary>
    /// 访问令牌(已绑定)
    /// </summary>
    public string? AccessToken { get; set; }
}

