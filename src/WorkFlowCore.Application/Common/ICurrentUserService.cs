namespace WorkFlowCore.Application.Common;

/// <summary>
/// 当前用户服务接口
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    CurrentUser GetCurrentUser();

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    Guid? GetUserId();

    /// <summary>
    /// 获取当前租户ID
    /// </summary>
    Guid? GetTenantId();

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    string? GetUserName();

    /// <summary>
    /// 检查是否已登录
    /// </summary>
    bool IsAuthenticated();

    /// <summary>
    /// 检查登录状态
    /// </summary>
    LoginStatus GetLoginStatus();
}

