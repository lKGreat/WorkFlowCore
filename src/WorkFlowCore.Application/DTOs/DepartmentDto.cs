using Volo.Abp.Application.Dtos;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 部门DTO
/// </summary>
public class DepartmentDto : FullAuditedEntityDto<long>
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 父部门名称
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// 负责人ID
    /// </summary>
    public long? ManagerId { get; set; }

    /// <summary>
    /// 负责人名称
    /// </summary>
    public string? ManagerName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 祖级列表（逗号分隔，如：0,100,101）
    /// </summary>
    public string? Ancestors { get; set; }

    /// <summary>
    /// 状态（0=正常,1=停用）
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 子部门
    /// </summary>
    public List<DepartmentDto>? Children { get; set; }
}

