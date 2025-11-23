using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

public interface IConfigService : IApplicationService
{
    Task<PagedResponse<ConfigDto>> GetPagedAsync(PagedRequest request);
    Task<ConfigDto?> GetByIdAsync(long id);
    Task<string?> GetByKeyAsync(string key);
    Task<ConfigDto> CreateAsync(ConfigDto dto);
    Task UpdateAsync(ConfigDto dto);
    Task DeleteAsync(List<long> ids);
}

public class ConfigDto
{
    public long ConfigId { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
    public string ConfigName { get; set; } = string.Empty;
    public string ConfigType { get; set; } = "N";
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
}

