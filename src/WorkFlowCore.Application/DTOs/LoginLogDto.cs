using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 登录日志DTO
/// </summary>
public class LoginLogDto
{
    public long InfoId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string Status { get; set; } = "0";
    public string Ipaddr { get; set; } = string.Empty;
    public string LoginLocation { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Os { get; set; } = string.Empty;
    public string Msg { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public string? ClientId { get; set; }
}

/// <summary>
/// 登录日志查询DTO
/// </summary>
public class LoginLogQueryDto : PagedRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 登录状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 登录IP
    /// </summary>
    public string? Ipaddr { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

