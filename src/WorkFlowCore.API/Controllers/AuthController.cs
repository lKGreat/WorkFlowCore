using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.DTOs.Auth;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Application.Services.Auth;
using WorkFlowCore.Domain.Identity;
using WorkFlowCore.Infrastructure.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
public class AuthController : BaseController
{
    private readonly ICaptchaService _captchaService;
    private readonly ISmsCodeService _smsCodeService;
    private readonly IThirdPartyLoginService _thirdPartyLoginService;
    private readonly IQrCodeLoginService _qrCodeLoginService;
    private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
    private readonly IRepository<AppUser, Guid> _userRepository;
    private readonly JwtService _jwtService;

    public AuthController(
        ICaptchaService captchaService,
        ISmsCodeService smsCodeService,
        IThirdPartyLoginService thirdPartyLoginService,
        IQrCodeLoginService qrCodeLoginService,
        SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
        IRepository<AppUser, Guid> userRepository,
        JwtService jwtService)
    {
        _captchaService = captchaService;
        _smsCodeService = smsCodeService;
        _thirdPartyLoginService = thirdPartyLoginService;
        _qrCodeLoginService = qrCodeLoginService;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    /// <summary>
    /// 获取图形验证码
    /// </summary>
    [HttpGet("captcha")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<CaptchaInfo>>> GetCaptcha()
    {
        var captcha = await _captchaService.GenerateAsync();
        return ApiResponse<CaptchaInfo>.Ok(captcha).ToActionResult();
    }

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    [HttpPost("sms/send")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object?>>> SendSmsCode([FromBody] SendSmsCodeInput input)
    {
        var result = await _smsCodeService.SendAsync(input.PhoneNumber, (Domain.Common.SmsCodeType)input.Type);
        return result
            ? ApiResponse<object?>.Ok(null, "发送成功").ToActionResult()
            : ApiResponse<object?>.Fail("发送失败").ToActionResult();
    }

    /// <summary>
    /// 用户名密码登录
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] UsernameLoginInput input)
    {
        // 验证图形验证码
        if (!await _captchaService.ValidateAsync(input.CaptchaUuid, input.CaptchaCode))
        {
            return ApiResponse<LoginResponse>.Fail("验证码错误").ToActionResult();
        }

        // 登录
        var result = await _signInManager.PasswordSignInAsync(
            input.UserName,
            input.Password,
            input.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var userQueryable = await _userRepository.GetQueryableAsync();
            var user = await userQueryable.FirstOrDefaultAsync(u => u.UserName == input.UserName);
            if (user == null)
            {
                return ApiResponse<LoginResponse>.Fail("用户不存在").ToActionResult();
            }

            var token = _jwtService.GenerateToken(user.Id, user.UserName!, user.TenantId ?? Guid.Empty);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return ApiResponse<LoginResponse>.Ok(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = MapToUserDto(user)
            }).ToActionResult();
        }

        // 根据失败原因返回具体错误信息
        if (result.IsLockedOut)
        {
            return ApiResponse<LoginResponse>.Fail("账号已被锁定，请稍后再试").ToActionResult();
        }
        
        if (result.IsNotAllowed)
        {
            return ApiResponse<LoginResponse>.Fail("账号未激活或邮箱未确认").ToActionResult();
        }
        
        if (result.RequiresTwoFactor)
        {
            return ApiResponse<LoginResponse>.Fail("需要双因素认证").ToActionResult();
        }

        return ApiResponse<LoginResponse>.Fail("用户名或密码错误").ToActionResult();
    }

    /// <summary>
    /// 手机号验证码登录
    /// </summary>
    [HttpPost("phone-login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> PhoneLogin([FromBody] PhoneLoginInput input)
    {
        // 验证短信验证码
        if (!await _smsCodeService.ValidateAsync(input.PhoneNumber, input.Code))
        {
            return ApiResponse<LoginResponse>.Fail("验证码错误").ToActionResult();
        }

        // 直接查询AppUser表 (使用ABP仓储)
        var userQueryable = await _userRepository.GetQueryableAsync();
        var user = await userQueryable.FirstOrDefaultAsync(u => u.PhoneNumber == input.PhoneNumber);
        
        if (user == null)
        {
            return ApiResponse<LoginResponse>.Fail("该手机号未注册").ToActionResult();
        }

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponse>.Fail("账号已被禁用").ToActionResult();
        }

        var token = _jwtService.GenerateToken(user.Id, user.UserName!, user.TenantId ?? Guid.Empty);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user)
        }).ToActionResult();
    }

    /// <summary>
    /// 第三方登录授权URL
    /// </summary>
    [HttpGet("oauth/{provider}/authorize")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<string>>> GetOAuthUrl(string provider, [FromQuery] string redirectUrl)
    {
        var authUrl = await _thirdPartyLoginService.GetAuthorizationUrlAsync(provider, redirectUrl);
        return ApiResponse<string>.Ok(authUrl).ToActionResult();
    }

    /// <summary>
    /// 第三方登录回调
    /// </summary>
    [HttpGet("oauth/{provider}/callback")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> OAuthCallback(
        string provider,
        [FromQuery] string code,
        [FromQuery] string state)
    {
        var result = await _thirdPartyLoginService.CallbackAsync(provider, code, state);

        if (result.NeedBind)
        {
            // 需要绑定账号
            return ApiResponse<object>.Ok(new
            {
                needBind = true,
                tempToken = result.TempToken,
                thirdPartyUser = result.ThirdPartyUser
            }, "需要绑定系统账号").ToActionResult();
        }

        if (result.User != null)
        {
            var token = _jwtService.GenerateToken(result.User.Id, result.User.UserName!, result.User.TenantId ?? Guid.Empty);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return ApiResponse<object>.Ok(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = MapToUserDto(result.User)
            }).ToActionResult();
        }

        return ApiResponse<object>.Fail("登录失败").ToActionResult();
    }

    /// <summary>
    /// 绑定第三方账号
    /// </summary>
    [HttpPost("oauth/{provider}/bind")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> BindThirdPartyAccount(
        string provider,
        [FromBody] BindAccountInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var result = await _thirdPartyLoginService.BindAccountAsync(userId, provider, input.TempToken);
        return result
            ? ApiResponse<object?>.Ok(null, "绑定成功").ToActionResult()
            : ApiResponse<object?>.Fail("绑定失败").ToActionResult();
    }

    /// <summary>
    /// 解绑第三方账号
    /// </summary>
    [HttpPost("oauth/{provider}/unbind")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> UnbindThirdPartyAccount(string provider)
    {
        var userId = CurrentUser.Id!.Value;
        var result = await _thirdPartyLoginService.UnbindAccountAsync(userId, provider);
        return result
            ? ApiResponse<object?>.Ok(null, "解绑成功").ToActionResult()
            : ApiResponse<object?>.Fail("解绑失败").ToActionResult();
    }

    /// <summary>
    /// 生成扫码登录二维码
    /// </summary>
    [HttpGet("qrcode/generate")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<QrCodeInfo>>> GenerateQrCode([FromQuery] string deviceId)
    {
        var qrInfo = await _qrCodeLoginService.GenerateQrCodeAsync(deviceId);
        return ApiResponse<QrCodeInfo>.Ok(qrInfo).ToActionResult();
    }

    /// <summary>
    /// 移动端扫描二维码
    /// </summary>
    [HttpPost("qrcode/scan")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> ScanQrCode([FromBody] ScanQrCodeInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var result = await _qrCodeLoginService.ScanAsync(input.Uuid, userId);
        return result
            ? ApiResponse<object?>.Ok(null, "扫描成功").ToActionResult()
            : ApiResponse<object?>.Fail("二维码已失效").ToActionResult();
    }

    /// <summary>
    /// 移动端确认登录
    /// </summary>
    [HttpPost("qrcode/confirm")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> ConfirmQrCode([FromBody] ConfirmQrCodeInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var result = await _qrCodeLoginService.ConfirmAsync(input.Uuid, userId);
        return result
            ? ApiResponse<object?>.Ok(null, "确认成功").ToActionResult()
            : ApiResponse<object?>.Fail("确认失败").ToActionResult();
    }

    /// <summary>
    /// Web端轮询二维码状态
    /// </summary>
    [HttpGet("qrcode/poll")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<QrCodeLoginResult>>> PollQrCodeStatus([FromQuery] string uuid)
    {
        var result = await _qrCodeLoginService.PollStatusAsync(uuid);

        // 如果已确认,生成Token
        if (result.Status == Domain.Common.QrCodeStatus.Confirmed && result.UserId.HasValue)
        {
            var userQueryable = await _userRepository.GetQueryableAsync();
            var user = await userQueryable.FirstOrDefaultAsync(u => u.Id == result.UserId.Value);
            if (user != null)
            {
                result.AccessToken = _jwtService.GenerateToken(user.Id, user.UserName!, user.TenantId ?? Guid.Empty);
                result.RefreshToken = _jwtService.GenerateRefreshToken();
                result.ExpiresIn = 3600;
            }
        }

        return ApiResponse<QrCodeLoginResult>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> Logout()
    {
        await _signInManager.SignOutAsync();
        return ApiResponse<object?>.Ok(null, "退出成功").ToActionResult();
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    [HttpGet("user-info")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserInfo()
    {
        var userId = CurrentUser.Id!.Value;
        var userQueryable = await _userRepository.GetQueryableAsync();
        var user = await userQueryable.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return ApiResponse<UserDto>.Fail("用户不存在").ToActionResult();
        }

        return ApiResponse<UserDto>.Ok(MapToUserDto(user)).ToActionResult();
    }

    /// <summary>
    /// 获取用户信息和权限（兼容ZrAdmin前端）
    /// </summary>
    [HttpGet("getInfo")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<GetInfoResult>>> GetInfo()
    {
        var userId = CurrentUser.Id!.Value;
        var userQueryable = await _userRepository.GetQueryableAsync();
        var user = await userQueryable.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return ApiResponse<GetInfoResult>.Fail("用户不存在").ToActionResult();
        }

        var menuService = HttpContext.RequestServices.GetRequiredService<IMenuService>();

        // 获取用户权限列表
        var permissions = await menuService.GetUserPermissionsAsync(userId);

        // 获取用户角色列表（从ABP CurrentUser获取）
        var roles = CurrentUser.Roles.ToList();
        if (!roles.Any())
        {
            roles.Add("default");
        }

        var result = new GetInfoResult
        {
            User = MapToUserInfoDto(user),
            Roles = roles,
            Permissions = permissions
        };

        return ApiResponse<GetInfoResult>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取路由信息（兼容ZrAdmin前端）
    /// </summary>
    [HttpGet("getRouters")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<RouterDto>>>> GetRouters()
    {
        var userId = CurrentUser.Id!.Value;
        var menuService = HttpContext.RequestServices.GetRequiredService<IMenuService>();

        var routers = await menuService.GetRoutersByRoleAsync(userId);

        return ApiResponse<List<RouterDto>>.Ok(routers).ToActionResult();
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // TODO: 实现刷新Token逻辑（需要存储RefreshToken到数据库）
        await Task.CompletedTask;
        return ApiResponse<LoginResponse>.Fail("刷新Token功能待实现").ToActionResult();
    }

    /// <summary>
    /// 映射用户到UserDto
    /// </summary>
    private static UserDto MapToUserDto(AppUser user)
    {
        return new UserDto
        {
            Id = 0, // AppUser使用Guid主键,暂时返回0
            UserName = user.UserName ?? string.Empty,
            RealName = user.NickName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Phone = user.PhoneNumber ?? string.Empty,
            IsEnabled = user.Status == "0"
        };
    }

    /// <summary>
    /// 映射用户到UserInfoDto
    /// </summary>
    private static UserInfoDto MapToUserInfoDto(AppUser user)
    {
        return new UserInfoDto
        {
            UserId = user.Id.ToString(),
            UserName = user.UserName ?? string.Empty,
            NickName = user.NickName ?? string.Empty,
            Avatar = null, // TODO: 从User表或其他地方获取头像
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            DepartmentId = null, // TODO: 从User表获取
            DepartmentName = null, // TODO: 从Department表获取
            Sex = null, // TODO: 从User表获取
            Status = user.Status
        };
    }
}

/// <summary>
/// 绑定账号输入
/// </summary>
public class BindAccountInput
{
    /// <summary>
    /// 临时令牌
    /// </summary>
    public string TempToken { get; set; } = string.Empty;
}

/// <summary>
/// 扫描二维码输入
/// </summary>
public class ScanQrCodeInput
{
    /// <summary>
    /// 二维码UUID
    /// </summary>
    public string Uuid { get; set; } = string.Empty;
}

/// <summary>
/// 确认二维码输入
/// </summary>
public class ConfirmQrCodeInput
{
    /// <summary>
    /// 二维码UUID
    /// </summary>
    public string Uuid { get; set; } = string.Empty;
}
