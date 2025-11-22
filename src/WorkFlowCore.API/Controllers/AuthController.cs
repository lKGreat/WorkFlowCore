using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
public class AuthController : BaseController
{
    private readonly IRepository<User, Guid> _userRepository;
    private readonly JwtService _jwtService;

    public AuthController(IRepository<User, Guid> userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        // 查找用户
        var queryable = await _userRepository.GetQueryableAsync();
        var user = queryable.FirstOrDefault(u => u.UserName == request.UserName);

        if (user == null || !user.IsEnabled)
        {
            return ApiResponse<LoginResponse>.Fail("用户名或密码错误", "INVALID_CREDENTIALS").ToActionResult();
        }

        // TODO: 验证密码哈希
        // 这里暂时跳过密码验证

        // 生成 Token
        var token = _jwtService.GenerateToken(user.Id, user.UserName, user.TenantId ?? Guid.Empty);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var response = new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                RealName = user.RealName,
                Email = user.Email,
                Phone = user.Phone,
                DepartmentId = user.DepartmentId,
                IsEnabled = user.IsEnabled
            }
        };

        return ApiResponse<LoginResponse>.Ok(response, "登录成功").ToActionResult();
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // TODO: 实现刷新Token逻辑（需要存储RefreshToken到数据库）
        await Task.CompletedTask;
        return ApiResponse<LoginResponse>.Fail("刷新Token功能待实现").ToActionResult();
    }
}
