using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 在线用户服务接口（基于JWT Token的简化实现）
/// </summary>
public interface IOnlineUserService : IApplicationService
{
    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    Task<List<OnlineUserDto>> GetOnlineUsersAsync();

    /// <summary>
    /// 记录用户登录（添加到在线列表）
    /// </summary>
    Task RecordUserLoginAsync(Guid userId, string userName, string nickName, string ipaddr, string? browser = null, string? os = null);

    /// <summary>
    /// 强制用户下线
    /// </summary>
    Task ForceLogoutAsync(Guid userId);

    /// <summary>
    /// 用户退出登录（从在线列表移除）
    /// </summary>
    Task RecordUserLogoutAsync(Guid userId);

    /// <summary>
    /// 清理过期的在线用户记录
    /// </summary>
    Task CleanExpiredUsersAsync();
}

