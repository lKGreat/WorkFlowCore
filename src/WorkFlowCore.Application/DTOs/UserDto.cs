using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 用户DTO
/// </summary>
public class UserDto : FullAuditedEntityDto<long>
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "用户名只能包含字母、数字和下划线")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "真实姓名不能为空")]
    [StringLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
    public string RealName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "电话格式不正确")]
    [StringLength(20, ErrorMessage = "电话长度不能超过20个字符")]
    public string? Phone { get; set; }

    public long? DepartmentId { get; set; }
    public long? ManagerId { get; set; }
    public bool IsEnabled { get; set; }
}

