using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

public class ConfigService : ApplicationService, IConfigService
{
    private readonly IRepository<SystemConfig, long> _configRepository;

    public ConfigService(IRepository<SystemConfig, long> configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<PagedResponse<ConfigDto>> GetPagedAsync(PagedRequest request)
    {
        var query = await _configRepository.GetQueryableAsync();
        var total = await query.CountAsync();
        
        var items = await query
            .OrderBy(c => c.ConfigKey)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ConfigDto
            {
                ConfigId = c.Id,
                ConfigKey = c.ConfigKey,
                ConfigValue = c.ConfigValue,
                ConfigName = c.ConfigName,
                ConfigType = c.ConfigType,
                Remark = c.Remark,
                CreationTime = c.CreationTime
            })
            .ToListAsync();

        return new PagedResponse<ConfigDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<ConfigDto?> GetByIdAsync(long id)
    {
        var config = await _configRepository.FindAsync(id);
        if (config == null) return null;

        return new ConfigDto
        {
            ConfigId = config.Id,
            ConfigKey = config.ConfigKey,
            ConfigValue = config.ConfigValue,
            ConfigName = config.ConfigName,
            ConfigType = config.ConfigType,
            Remark = config.Remark,
            CreationTime = config.CreationTime
        };
    }

    public async Task<string?> GetByKeyAsync(string key)
    {
        var config = await _configRepository.FirstOrDefaultAsync(c => c.ConfigKey == key);
        return config?.ConfigValue;
    }

    public async Task<ConfigDto> CreateAsync(ConfigDto dto)
    {
        var config = new SystemConfig(
            SnowflakeIdGenerator.NextId(),
            dto.ConfigKey,
            dto.ConfigValue,
            dto.ConfigName)
        {
            ConfigType = dto.ConfigType,
            Remark = dto.Remark
        };

        await _configRepository.InsertAsync(config);

        dto.ConfigId = config.Id;
        dto.CreationTime = config.CreationTime;
        return dto;
    }

    public async Task UpdateAsync(ConfigDto dto)
    {
        var config = await _configRepository.GetAsync(dto.ConfigId);
        config.ConfigValue = dto.ConfigValue;
        config.ConfigName = dto.ConfigName;
        config.Remark = dto.Remark;

        await _configRepository.UpdateAsync(config);
    }

    public async Task DeleteAsync(List<long> ids)
    {
        await _configRepository.DeleteManyAsync(ids);
    }
}

