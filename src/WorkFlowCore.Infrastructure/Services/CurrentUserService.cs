using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WorkFlowCore.Application.Common;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 当前用户服务实现
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    public CurrentUser GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return CurrentUser.NotLoggedIn();
        }

        // 检查Token是否过期
        var expClaim = user.FindFirst("exp")?.Value;
        if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out var exp))
        {
            var expireTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            if (expireTime < DateTime.UtcNow)
            {
                return CurrentUser.TokenExpired();
            }
        }

        var currentUser = new CurrentUser
        {
            Status = LoginStatus.LoggedIn,
            UserId = GetClaimValue<Guid?>(user, ClaimTypes.NameIdentifier, "sub"),
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value,
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value,
            PhoneNumber = user.FindFirst(ClaimTypes.MobilePhone)?.Value ?? user.FindFirst("phone_number")?.Value,
            TenantId = GetClaimValue<Guid?>(user, "tenant_id"),
            TenantName = user.FindFirst("tenant_name")?.Value,
            DepartmentId = GetClaimValue<Guid?>(user, "department_id"),
            DepartmentName = user.FindFirst("department_name")?.Value,
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };

        if (expClaim != null && long.TryParse(expClaim, out exp))
        {
            currentUser.TokenExpireAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
        }

        return currentUser;
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    public Guid? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return null;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                          user.FindFirst("sub")?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <summary>
    /// 获取当前租户ID
    /// </summary>
    public Guid? GetTenantId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return null;
        }

        var tenantIdClaim = user.FindFirst("tenant_id")?.Value;
        return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
    }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    public string? GetUserName()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return null;
        }

        return user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value;
    }

    /// <summary>
    /// 检查是否已登录
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// 检查登录状态
    /// </summary>
    public LoginStatus GetLoginStatus()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return LoginStatus.NotLoggedIn;
        }

        // 检查Token是否过期
        var expClaim = user.FindFirst("exp")?.Value;
        if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out var exp))
        {
            var expireTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            if (expireTime < DateTime.UtcNow)
            {
                return LoginStatus.TokenExpired;
            }
        }

        return LoginStatus.LoggedIn;
    }

    /// <summary>
    /// 获取Claim值并转换为指定类型
    /// </summary>
    private T? GetClaimValue<T>(ClaimsPrincipal user, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var claim = user.FindFirst(claimType);
            if (claim != null)
            {
                try
                {
                    if (typeof(T) == typeof(Guid?) || typeof(T) == typeof(Guid))
                    {
                        return Guid.TryParse(claim.Value, out var guid) ? (T)(object)guid : default;
                    }
                    return (T)Convert.ChangeType(claim.Value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
                }
                catch
                {
                    return default;
                }
            }
        }
        return default;
    }
}

