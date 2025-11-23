using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Identity;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 系统接口控制器 (兼容ZrAdmin接口)
/// </summary>
[Route("api")]
public class SystemController : BaseController
{
    private readonly IMenuService _menuService;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<Volo.Abp.Identity.IdentityRole> _roleManager;

    public SystemController(
        IMenuService menuService,
        UserManager<AppUser> userManager,
        RoleManager<Volo.Abp.Identity.IdentityRole> roleManager)
    {
        _menuService = menuService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    [HttpGet("getInfo")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<GetInfoResult>>> GetInfo()
    {
        var userId = CurrentUser.Id!.Value;
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return ApiResponse<GetInfoResult>.Fail("用户不存在").ToActionResult();
        }

        // 获取角色
        var roles = await _userManager.GetRolesAsync(user);

        // 获取权限 (从角色权限中获取)
        var permissions = new List<string>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                // TODO: 从PermissionManagement获取角色权限
                // 这里暂时返回基础权限
                permissions.Add($"{roleName}:*");
            }
        }

        var result = new GetInfoResult
        {
            User = new UserInfoDto
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName ?? string.Empty,
                NickName = user.NickName ?? user.UserName ?? string.Empty,
                Avatar = user.Avatar,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = user.DepartmentId,
                Sex = "0",
                Status = user.Status
            },
            Roles = roles.ToList(),
            Permissions = permissions.Distinct().ToList()
        };

        return ApiResponse<GetInfoResult>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    [HttpGet("getRouters")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<RouterDto>>>> GetRouters()
    {
        var userId = CurrentUser.Id!.Value;
        var routers = await _menuService.GetRoutersByRoleAsync(userId);
        return ApiResponse<List<RouterDto>>.Ok(routers).ToActionResult();
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpPost("LogOut")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> Logout()
    {
        // TODO: 清除Token缓存
        await Task.CompletedTask;
        return ApiResponse<object?>.Ok(null, "退出成功").ToActionResult();
    }
}

