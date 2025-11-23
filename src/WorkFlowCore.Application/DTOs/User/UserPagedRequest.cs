using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs.User;

/// <summary>
/// 用户分页查询请求
/// </summary>
public class UserPagedRequest : PagedRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

