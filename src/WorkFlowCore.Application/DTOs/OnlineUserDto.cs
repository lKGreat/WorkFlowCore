namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 在线用户DTO
/// </summary>
public class OnlineUserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 登录IP
    /// </summary>
    public string Ipaddr { get; set; } = string.Empty;

    /// <summary>
    /// 登录地点
    /// </summary>
    public string? LoginLocation { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    public string? Os { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// Token（前端不返回，仅用于强制下线）
    /// </summary>
    public string? Token { get; set; }
}

