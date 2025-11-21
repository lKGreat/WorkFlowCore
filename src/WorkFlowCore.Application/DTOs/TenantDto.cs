namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 租户DTO
/// </summary>
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsEnabled { get; set; }
}

