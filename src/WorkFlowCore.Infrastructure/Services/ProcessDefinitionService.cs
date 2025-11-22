using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 流程定义服务实现
/// </summary>
public class ProcessDefinitionService : ApplicationService, IProcessDefinitionService
{
    private readonly IRepository<ProcessDefinition, Guid> _repository;

    public ProcessDefinitionService(IRepository<ProcessDefinition, Guid> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 创建流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto> CreateAsync(CreateProcessDefinitionRequest request, Guid tenantId)
    {
        // 检查同名流程Key是否已存在
        var queryable = await _repository.GetQueryableAsync();
        var existingProcesses = queryable
            .Where(p => p.TenantId == tenantId && p.Key == request.Key)
            .OrderByDescending(p => p.Version);
        
        var existingProcess = await AsyncExecuter.FirstOrDefaultAsync(existingProcesses);

        int version = 1;
        if (existingProcess != null)
        {
            // 如果已存在同Key的流程,版本号自增
            version = existingProcess.Version + 1;
        }

        var processDefinition = new ProcessDefinition(
            GuidGenerator.Create(),
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

        await _repository.InsertAsync(processDefinition);

        return ObjectMapper.Map<ProcessDefinition, ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 更新流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto> UpdateAsync(Guid id, UpdateProcessDefinitionRequest request, Guid tenantId, bool createNewVersion = false)
    {
        var queryable = await _repository.GetQueryableAsync();
        var processDefinition = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(p => p.Id == id && p.TenantId == tenantId)
        );

        if (processDefinition == null)
        {
            throw new Volo.Abp.UserFriendlyException("流程定义不存在");
        }

        if (createNewVersion)
        {
            // 创建新版本
            var newVersion = new ProcessDefinition(
                GuidGenerator.Create(),
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

            await _repository.InsertAsync(newVersion);

            return ObjectMapper.Map<ProcessDefinition, ProcessDefinitionDto>(newVersion);
        }
        else
        {
            // 更新当前版本
            if (request.Name != null) processDefinition.Name = request.Name;
            if (request.Description != null) processDefinition.Description = request.Description;
            if (request.Content != null) processDefinition.Content = request.Content;
            if (request.ContentFormat != null) processDefinition.ContentFormat = request.ContentFormat;
            if (request.IsEnabled != null) processDefinition.IsEnabled = request.IsEnabled.Value;

            await _repository.UpdateAsync(processDefinition);

            return ObjectMapper.Map<ProcessDefinition, ProcessDefinitionDto>(processDefinition);
        }
    }

    /// <summary>
    /// 删除流程定义（软删除）
    /// </summary>
    public async Task DeleteAsync(Guid id, Guid tenantId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var processDefinition = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(p => p.Id == id && p.TenantId == tenantId)
        );

        if (processDefinition == null)
        {
            throw new Volo.Abp.UserFriendlyException("流程定义不存在");
        }

        // ABP的DeleteAsync会自动处理软删除
        await _repository.DeleteAsync(processDefinition);
    }

    /// <summary>
    /// 根据ID获取流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto?> GetByIdAsync(Guid id, Guid tenantId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var processDefinition = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(p => p.Id == id && p.TenantId == tenantId)
        );

        return processDefinition == null ? null : ObjectMapper.Map<ProcessDefinition, ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 根据Key和版本获取流程定义
    /// </summary>
    public async Task<ProcessDefinitionDto?> GetByKeyAndVersionAsync(string key, int version, Guid tenantId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var processDefinition = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(p => p.Key == key && p.Version == version && p.TenantId == tenantId)
        );

        return processDefinition == null ? null : ObjectMapper.Map<ProcessDefinition, ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 获取流程定义的最新版本
    /// </summary>
    public async Task<ProcessDefinitionDto?> GetLatestVersionAsync(string key, Guid tenantId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var processDefinition = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(p => p.Key == key && p.TenantId == tenantId)
                     .OrderByDescending(p => p.Version)
        );

        return processDefinition == null ? null : ObjectMapper.Map<ProcessDefinition, ProcessDefinitionDto>(processDefinition);
    }

    /// <summary>
    /// 获取流程定义的版本历史
    /// </summary>
    public async Task<List<ProcessDefinitionVersionDto>> GetVersionHistoryAsync(string key, Guid tenantId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var versions = await AsyncExecuter.ToListAsync(
            queryable.Where(p => p.Key == key && p.TenantId == tenantId)
                     .OrderByDescending(p => p.Version)
        );

        return ObjectMapper.Map<List<ProcessDefinition>, List<ProcessDefinitionVersionDto>>(versions);
    }

    /// <summary>
    /// 分页查询流程定义
    /// </summary>
    public async Task<PagedResponse<ProcessDefinitionListDto>> GetPagedAsync(PagedRequest request, Guid tenantId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable.Where(p => p.TenantId == tenantId);

        // 只显示每个Key的最新版本
        var latestVersions = await AsyncExecuter.ToListAsync(
            query.GroupBy(p => p.Key)
                 .Select(g => g.OrderByDescending(p => p.Version).FirstOrDefault())
        );

        var total = latestVersions.Count;
        var items = latestVersions
            .Skip(request.Skip)
            .Take(request.Take)
            .Where(p => p != null)
            .ToList();

        var dtos = ObjectMapper.Map<List<ProcessDefinition>, List<ProcessDefinitionListDto>>(items!);

        return new PagedResponse<ProcessDefinitionListDto>
        {
            Items = dtos,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}

