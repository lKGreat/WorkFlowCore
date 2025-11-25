using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 在线用户服务实现（基于内存缓存的简化版本）
/// 注意：这是一个简化实现，生产环境建议使用Redis等分布式缓存
/// </summary>
public class OnlineUserService : ApplicationService, IOnlineUserService
{
    private readonly IMemoryCache _cache;
    private const string ONLINE_USER_PREFIX = "online_user_";
    private const string ONLINE_USER_LIST_KEY = "online_user_list";
    private static readonly TimeSpan OnlineUserExpiration = TimeSpan.FromHours(2); // Token有效期

    public OnlineUserService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<List<OnlineUserDto>> GetOnlineUsersAsync()
    {
        var userIds = _cache.Get<List<Guid>>(ONLINE_USER_LIST_KEY) ?? new List<Guid>();
        var onlineUsers = new List<OnlineUserDto>();

        foreach (var userId in userIds)
        {
            var key = ONLINE_USER_PREFIX + userId;
            if (_cache.TryGetValue(key, out OnlineUserDto? user) && user != null)
            {
                // 清除Token（安全考虑）
                user.Token = null;
                onlineUsers.Add(user);
            }
        }

        return Task.FromResult(onlineUsers.OrderByDescending(u => u.LoginTime).ToList());
    }

    public Task RecordUserLoginAsync(Guid userId, string userName, string nickName, string ipaddr, string? browser = null, string? os = null)
    {
        var onlineUser = new OnlineUserDto
        {
            UserId = userId,
            UserName = userName,
            NickName = nickName,
            Ipaddr = ipaddr,
            Browser = browser,
            Os = os,
            LoginTime = DateTime.Now
        };

        // 添加到缓存
        var key = ONLINE_USER_PREFIX + userId;
        _cache.Set(key, onlineUser, OnlineUserExpiration);

        // 更新在线用户列表
        var userIds = _cache.Get<List<Guid>>(ONLINE_USER_LIST_KEY) ?? new List<Guid>();
        if (!userIds.Contains(userId))
        {
            userIds.Add(userId);
            _cache.Set(ONLINE_USER_LIST_KEY, userIds, TimeSpan.FromDays(1));
        }

        return Task.CompletedTask;
    }

    public Task ForceLogoutAsync(Guid userId)
    {
        // 从缓存中移除
        var key = ONLINE_USER_PREFIX + userId;
        _cache.Remove(key);

        // 从在线列表移除
        var userIds = _cache.Get<List<Guid>>(ONLINE_USER_LIST_KEY) ?? new List<Guid>();
        userIds.Remove(userId);
        _cache.Set(ONLINE_USER_LIST_KEY, userIds, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    public Task RecordUserLogoutAsync(Guid userId)
    {
        return ForceLogoutAsync(userId);
    }

    public Task CleanExpiredUsersAsync()
    {
        var userIds = _cache.Get<List<Guid>>(ONLINE_USER_LIST_KEY) ?? new List<Guid>();
        var activeUserIds = new List<Guid>();

        foreach (var userId in userIds)
        {
            var key = ONLINE_USER_PREFIX + userId;
            if (_cache.TryGetValue(key, out OnlineUserDto? user))
            {
                activeUserIds.Add(userId);
            }
        }

        // 更新活跃用户列表
        _cache.Set(ONLINE_USER_LIST_KEY, activeUserIds, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }
}

