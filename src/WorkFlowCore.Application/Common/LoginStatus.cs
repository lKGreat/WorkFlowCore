namespace WorkFlowCore.Application.Common;

/// <summary>
/// 登录状态枚举
/// </summary>
public enum LoginStatus
{
    /// <summary>
    /// 未登录
    /// </summary>
    NotLoggedIn = 0,

    /// <summary>
    /// 已登录
    /// </summary>
    LoggedIn = 1,

    /// <summary>
    /// Token已过期
    /// </summary>
    TokenExpired = 2,

    /// <summary>
    /// Token无效
    /// </summary>
    TokenInvalid = 3
}

