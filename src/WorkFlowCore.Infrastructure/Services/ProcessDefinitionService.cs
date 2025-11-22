using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Data;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 流程定义服务实现
/// </summary>
public class ProcessDefinitionService : IProcessDefinitionService
{
    private readonly WorkFlowDbContext _context;
    private readonly IMapper _mapper;

    public ProcessDefinitionService(WorkFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 创建流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto> CreateAsync(CreateProcessDefinitionRequest request, Guid tenantId)
    {
        // 检查同名流程Key是否已存在
        var existingProcess = await _context.ProcessDefinitions
            .Where(p => p.TenantId == tenantId && p.Key == request.Key)
            .OrderByDescending(p => p.Version)
            .FirstOrDefaultAsync();

        int version = 1;
        if (existingProcess != null)
        {
            // 如果已存在同Key的流程,版本号自增
            version = existingProcess.Version + 1;
        }

        var processDefinition = new ProcessDefinition(
            Guid.NewGuid(),
            tenantId,
            request.Name,
            request.Key,
            request.Content
        )
        {
            Version = version,
            Description = request.Description,
            ContentFormat = request.ContentFormat,
            IsEnabled = request.IsEnabled
        };

        _context.ProcessDefinitions.Add(processDefinition);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 更新流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto> UpdateAsync(Guid id, UpdateProcessDefinitionRequest request, Guid tenantId, bool createNewVersion = false)
    {
        var processDefinition = await _context.ProcessDefinitions
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        if (processDefinition == null)
        {
            throw new Exception("流程定义不存在");
        }

        if (createNewVersion)
        {
            // 创建新版本
            var newVersion = new ProcessDefinition(
                Guid.NewGuid(),
                tenantId,
                request.Name ?? processDefinition.Name,
                processDefinition.Key,
                request.Content ?? processDefinition.Content
            )
            {
                Version = processDefinition.Version + 1,
                Description = request.Description ?? processDefinition.Description,
                ContentFormat = request.ContentFormat ?? processDefinition.ContentFormat,
                IsEnabled = request.IsEnabled ?? processDefinition.IsEnabled
            };

            _context.ProcessDefinitions.Add(newVersion);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProcessDefinitionDto>(newVersion);
        }
        else
        {
            // 更新当前版本
            if (request.Name != null) processDefinition.Name = request.Name;
            if (request.Description != null) processDefinition.Description = request.Description;
            if (request.Content != null) processDefinition.Content = request.Content;
            if (request.ContentFormat != null) processDefinition.ContentFormat = request.ContentFormat;
            if (request.IsEnabled != null) processDefinition.IsEnabled = request.IsEnabled.Value;

            await _context.SaveChangesAsync();

            return _mapper.Map<ProcessDefinitionDto>(processDefinition);
        }
    }

    /// <summary>
    /// 删除流程定义（软删除）
    /// </summary>
    public async Task DeleteAsync(Guid id, Guid tenantId)
    {
        var processDefinition = await _context.ProcessDefinitions
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        if (processDefinition == null)
        {
            throw new Exception("流程定义不存在");
        }

        processDefinition.IsDeleted = true;
        // ABP的FullAuditedAggregateRoot会自动处理软删除，无需手动设置DeletedAt
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 根据ID获取流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto?> GetByIdAsync(Guid id, Guid tenantId)
    {
        var processDefinition = await _context.ProcessDefinitions
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        return processDefinition == null ? null : _mapper.Map<ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 根据Key和版本获取流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto?> GetByKeyAndVersionAsync(string key, int version, Guid tenantId)
    {
        var processDefinition = await _context.ProcessDefinitions
            .FirstOrDefaultAsync(p => p.Key == key && p.Version == version && p.TenantId == tenantId);

        return processDefinition == null ? null : _mapper.Map<ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 获取流程定义的最新版本
    /// </summary>
    public async Task<ProcessDefinitionDto?> GetLatestVersionAsync(string key, Guid tenantId)
    {
        var processDefinition = await _context.ProcessDefinitions
            .Where(p => p.Key == key && p.TenantId == tenantId)
            .OrderByDescending(p => p.Version)
            .FirstOrDefaultAsync();

        return processDefinition == null ? null : _mapper.Map<ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 获取流程定义的版本历史
    /// </summary>
    public async Task<List<ProcessDefinitionVersionDto>> GetVersionHistoryAsync(string key, Guid tenantId)
    {
        var versions = await _context.ProcessDefinitions
            .Where(p => p.Key == key && p.TenantId == tenantId)
            .OrderByDescending(p => p.Version)
            .ToListAsync();

        return _mapper.Map<List<ProcessDefinitionVersionDto>>(versions);
    }

    /// <summary>
    /// 分页查询流程定义
    /// </summary>
    public async Task<PagedResponse<ProcessDefinitionListDto>> GetPagedAsync(PagedRequest request, Guid tenantId)
    {
        var query = _context.ProcessDefinitions
            .Where(p => p.TenantId == tenantId);

        // 只显示每个Key的最新版本
        var latestVersions = await query
            .GroupBy(p => p.Key)
            .Select(g => g.OrderByDescending(p => p.Version).FirstOrDefault())
            .ToListAsync();

        var total = latestVersions.Count;
        var items = latestVersions
            .Skip(request.Skip)
            .Take(request.Take)
            .Where(p => p != null)
            .ToList();

        var dtos = _mapper.Map<List<ProcessDefinitionListDto>>(items);

        return new PagedResponse<ProcessDefinitionListDto>
        {
            Items = dtos,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}

