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

    /// <summary>
    /// 获取个人资料
    /// </summary>
    [HttpGet("system/user/profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetProfile()
    {
        var userId = CurrentUser.Id!.Value;
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return ApiResponse<UserInfoDto>.Fail("用户不存在").ToActionResult();
        }

        return ApiResponse<UserInfoDto>.Ok(new UserInfoDto
        {
            UserId = user.Id.ToString(),
            UserName = user.UserName ?? string.Empty,
            NickName = user.NickName ?? string.Empty,
            Avatar = user.Avatar,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DepartmentId = user.DepartmentId,
            Sex = "0",
            Status = user.Status
        }).ToActionResult();
    }

    /// <summary>
    /// 更新个人资料
    /// </summary>
    [HttpPut("system/user/profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> UpdateProfile([FromBody] UpdateProfileInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return ApiResponse<object?>.Fail("用户不存在").ToActionResult();
        }

        user.NickName = input.NickName;
        await _userManager.SetEmailAsync(user, input.Email);
        await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);

        await _userManager.UpdateAsync(user);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    [HttpPut("system/user/profile/updatePwd")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> UpdatePassword([FromBody] UpdatePasswordInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return ApiResponse<object?>.Fail("用户不存在").ToActionResult();
        }

        var result = await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
        if (!result.Succeeded)
        {
            return ApiResponse<object?>.Fail("密码修改失败: " + string.Join(", ", result.Errors.Select(e => e.Description))).ToActionResult();
        }

        return ApiResponse<object?>.Ok(null, "密码修改成功").ToActionResult();
    }

    /// <summary>
    /// 上传头像
    /// </summary>
    [HttpPost("system/user/profile/avatar")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AvatarUploadResult>>> UploadAvatar([FromForm(Name = "avatar")] IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
        {
            return ApiResponse<AvatarUploadResult>.Fail("请选择要上传的图片").ToActionResult();
        }

        // 验证文件类型
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return ApiResponse<AvatarUploadResult>.Fail("仅支持 jpg, jpeg, png, gif, webp 格式的图片").ToActionResult();
        }

        // 验证文件大小（2MB）
        if (avatar.Length > 2 * 1024 * 1024)
        {
            return ApiResponse<AvatarUploadResult>.Fail("图片大小不能超过 2MB").ToActionResult();
        }

        // 保存文件
        var userId = CurrentUser.Id!.Value;
        var fileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
        var uploadPath = Path.Combine("wwwroot", "uploads", "avatars");
        
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.CopyToAsync(stream);
        }

        // 更新用户头像URL
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            var avatarUrl = $"/uploads/avatars/{fileName}";
            user.Avatar = avatarUrl;
            await _userManager.UpdateAsync(user);

            return ApiResponse<AvatarUploadResult>.Ok(new AvatarUploadResult
            {
                ImgUrl = avatarUrl
            }, "头像上传成功").ToActionResult();
        }

        return ApiResponse<AvatarUploadResult>.Fail("用户不存在").ToActionResult();
    }
}

public class UpdateProfileInput
{
    public string? NickName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UpdatePasswordInput
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class AvatarUploadResult
{
    public string ImgUrl { get; set; } = string.Empty;
}

