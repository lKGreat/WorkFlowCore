using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 租户DTO
/// </summary>
public class TenantDto : FullAuditedEntityDto<Guid>
{
    [Required(ErrorMessage = "租户名称不能为空")]
    [StringLength(100, ErrorMessage = "租户名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "租户编码不能为空")]
    [StringLength(50, ErrorMessage = "租户编码长度不能超过50个字符")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "租户编码只能包含小写字母、数字和连字符")]
    public string Code { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "联系人长度不能超过100个字符")]
    public string? ContactPerson { get; set; }

    [Phone(ErrorMessage = "联系电话格式不正确")]
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string? ContactPhone { get; set; }

    [EmailAddress(ErrorMessage = "联系邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "联系邮箱长度不能超过100个字符")]
    public string? ContactEmail { get; set; }

    public bool IsEnabled { get; set; }
}

