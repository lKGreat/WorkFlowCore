namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 用户DTO
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ManagerId { get; set; }
    public bool IsEnabled { get; set; }
}

