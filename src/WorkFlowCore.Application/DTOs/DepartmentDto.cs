using Volo.Abp.Application.Dtos;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 部门DTO
/// </summary>
public class DepartmentDto : FullAuditedEntityDto<long>
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public long? ParentId { get; set; }
    public long? ManagerId { get; set; }
    public int SortOrder { get; set; }
}

