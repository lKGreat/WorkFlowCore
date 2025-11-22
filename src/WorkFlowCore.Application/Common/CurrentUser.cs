using System;

namespace WorkFlowCore.Application.Common;

/// <summary>
/// 当前登录用户信息
/// </summary>
public class CurrentUser
{
    /// <summary>
    /// 登录状态
    /// </summary>
    public LoginStatus Status { get; set; } = LoginStatus.NotLoggedIn;

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 租户名称
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// 用户角色列表
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Token过期时间
    /// </summary>
    public DateTime? TokenExpireAt { get; set; }

    /// <summary>
    /// 是否已登录
    /// </summary>
    public bool IsAuthenticated => Status == LoginStatus.LoggedIn && UserId.HasValue;

    /// <summary>
    /// 是否Token已过期
    /// </summary>
    public bool IsTokenExpired => Status == LoginStatus.TokenExpired;

    /// <summary>
    /// 是否Token无效
    /// </summary>
    public bool IsTokenInvalid => Status == LoginStatus.TokenInvalid;

    /// <summary>
    /// 是否有指定角色
    /// </summary>
    public bool HasRole(string role) => Roles.Contains(role);

    /// <summary>
    /// 是否有任意指定角色
    /// </summary>
    public bool HasAnyRole(params string[] roles) => roles.Any(r => Roles.Contains(r));

    /// <summary>
    /// 是否有所有指定角色
    /// </summary>
    public bool HasAllRoles(params string[] roles) => roles.All(r => Roles.Contains(r));

    /// <summary>
    /// 创建未登录用户
    /// </summary>
    public static CurrentUser NotLoggedIn() => new() { Status = LoginStatus.NotLoggedIn };

    /// <summary>
    /// 创建Token过期用户
    /// </summary>
    public static CurrentUser TokenExpired() => new() { Status = LoginStatus.TokenExpired };

    /// <summary>
    /// 创建Token无效用户
    /// </summary>
    public static CurrentUser TokenInvalid() => new() { Status = LoginStatus.TokenInvalid };
}

