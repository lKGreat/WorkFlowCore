namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 登录请求
/// </summary>
public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
}

/// <summary>
/// 登录响应
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// 刷新Token请求
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

