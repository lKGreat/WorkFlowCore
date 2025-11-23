namespace WorkFlowCore.Application.DTOs.Role;

/// <summary>
/// 角色DTO
/// </summary>
public class RoleDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? RoleKey { get; set; }
    public int RoleSort { get; set; }
    public string Status { get; set; } = "0";
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
    public List<long>? MenuIds { get; set; }
}

public class CreateRoleInput
{
    public string RoleName { get; set; } = string.Empty;
    public string? RoleKey { get; set; }
    public int RoleSort { get; set; }
    public string Status { get; set; } = "0";
    public string? Remark { get; set; }
    public List<long> MenuIds { get; set; } = new();
}

public class UpdateRoleInput
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? RoleKey { get; set; }
    public int RoleSort { get; set; }
    public string Status { get; set; } = "0";
    public string? Remark { get; set; }
}

public class RoleOption
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}

