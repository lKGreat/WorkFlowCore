namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 部门DTO
/// </summary>
public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? ManagerId { get; set; }
    public int SortOrder { get; set; }
}

