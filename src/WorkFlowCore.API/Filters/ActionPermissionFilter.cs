using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp.DependencyInjection;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Filters;

/// <summary>
/// 接口权限过滤器
/// 用于验证用户是否有权限访问特定接口
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ActionPermissionFilter : ActionFilterAttribute
{
    /// <summary>
    /// 权限字符串，例如 system:user:list
    /// </summary>
    public string Permission { get; set; } = string.Empty;

    /// <summary>
    /// 角色字符串，例如 admin,common
    /// </summary>
    public string RolePermission { get; set; } = string.Empty;

    /// <summary>
    /// 是否跳过权限验证（用于公开接口）
    /// </summary>
    public bool SkipAuthorization { get; set; } = false;

    private ILogger<ActionPermissionFilter>? _logger;
    private IMemoryCache? _cache;
    private IMenuService? _menuService;

    public ActionPermissionFilter()
    {
    }

    public ActionPermissionFilter(string permission)
    {
        Permission = permission;
    }

    /// <summary>
    /// 执行Action前校验是否有权限访问
    /// </summary>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 如果跳过权限验证，直接执行
        if (SkipAuthorization)
        {
            await next();
            return;
        }

        // 获取服务
        _logger = context.HttpContext.RequestServices.GetService<ILogger<ActionPermissionFilter>>();
        _cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
        _menuService = context.HttpContext.RequestServices.GetService<IMenuService>();

        // 获取当前用户信息
        var currentUserService = context.HttpContext.RequestServices.GetService<ICurrentUserService>();
        if (currentUserService == null)
        {
            context.Result = UnauthorizedResult("无法获取用户信息");
            return;
        }

        var currentUser = currentUserService.GetCurrentUser();
        
        // 检查用户是否已登录
        if (!currentUser.IsAuthenticated)
        {
            context.Result = UnauthorizedResult("用户未登录");
            return;
        }

        var userId = currentUser.UserId!.Value;
        var userName = currentUser.UserName ?? "Unknown";

        // 检查权限
        var hasPermission = await CheckPermissionAsync(userId, currentUser);
        
        if (!hasPermission)
        {
            var url = context.HttpContext.Request.Path;
            _logger?.LogWarning("用户 {UserName}({UserId}) 没有权限访问 {Url}，需要权限: {Permission}", 
                userName, userId, url, Permission);

            context.Result = ForbiddenResult($"您没有权限访问此资源，需要权限: {Permission}");
            return;
        }

        // 权限验证通过，继续执行
        await next();
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    private async Task<bool> CheckPermissionAsync(Guid userId, Application.Common.CurrentUser currentUser)
    {
        // 1. 检查是否为超级管理员（拥有所有权限）
        if (currentUser.HasRole("admin"))
        {
            return true;
        }

        // 2. 如果没有指定权限要求，允许访问
        if (string.IsNullOrEmpty(Permission) && string.IsNullOrEmpty(RolePermission))
        {
            return true;
        }

        // 3. 检查角色权限
        if (!string.IsNullOrEmpty(RolePermission))
        {
            var requiredRoles = RolePermission.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (currentUser.HasAnyRole(requiredRoles))
            {
                return true;
            }
        }

        // 4. 检查菜单权限
        if (!string.IsNullOrEmpty(Permission) && _menuService != null)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            
            // 检查是否有完全匹配的权限
            if (userPermissions.Contains(Permission, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            // 检查通配符权限 *:*:*
            if (userPermissions.Contains("*:*:*"))
            {
                return true;
            }

            // 检查模块级通配符，例如 system:*:*
            var parts = Permission.Split(':');
            if (parts.Length >= 2)
            {
                var moduleWildcard = $"{parts[0]}:*:*";
                if (userPermissions.Contains(moduleWildcard, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 获取用户权限列表（带缓存）
    /// </summary>
    private async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        if (_cache == null || _menuService == null)
        {
            return new List<string>();
        }

        var cacheKey = $"user_permissions_{userId}";
        
        if (_cache.TryGetValue(cacheKey, out List<string>? permissions) && permissions != null)
        {
            return permissions;
        }

        // 从数据库获取权限
        permissions = await _menuService.GetUserPermissionsAsync(userId);
        
        // 缓存10分钟
        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(10));
        
        return permissions;
    }

    /// <summary>
    /// 返回401未授权结果
    /// </summary>
    private JsonResult UnauthorizedResult(string message)
    {
        return new JsonResult(ApiResponse<object>.Fail(message, "401"))
        {
            StatusCode = 401
        };
    }

    /// <summary>
    /// 返回403禁止访问结果
    /// </summary>
    private JsonResult ForbiddenResult(string message)
    {
        return new JsonResult(ApiResponse<object>.Fail(message, "403"))
        {
            StatusCode = 403
        };
    }
}

