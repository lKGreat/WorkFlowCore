using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 流程定义服务接口
/// </summary>
public interface IProcessDefinitionService
{
    /// <summary>
    /// 创建流程定义
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>创建的流程定义</returns>
    Task<ProcessDefinitionDto> CreateAsync(CreateProcessDefinitionRequest request, Guid tenantId);

    /// <summary>
    /// 更新流程定义
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <param name="request">更新请求</param>
    /// <param name="tenantId">租户ID</param>
    /// <param name="createNewVersion">是否创建新版本</param>
    /// <returns>更新后的流程定义</returns>
    Task<ProcessDefinitionDto> UpdateAsync(Guid id, UpdateProcessDefinitionRequest request, Guid tenantId, bool createNewVersion = false);

    /// <summary>
    /// 删除流程定义（软删除）
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <param name="tenantId">租户ID</param>
    Task DeleteAsync(Guid id, Guid tenantId);

    /// <summary>
    /// 根据ID获取流程定义
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>流程定义</returns>
    Task<ProcessDefinitionDto?> GetByIdAsync(Guid id, Guid tenantId);

    /// <summary>
    /// 根据Key和版本获取流程定义
    /// </summary>
    /// <param name="key">流程Key</param>
    /// <param name="version">版本号</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>流程定义</returns>
    Task<ProcessDefinitionDto?> GetByKeyAndVersionAsync(string key, int version, Guid tenantId);

    /// <summary>
    /// 获取流程定义的最新版本
    /// </summary>
    /// <param name="key">流程Key</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>最新版本的流程定义</returns>
    Task<ProcessDefinitionDto?> GetLatestVersionAsync(string key, Guid tenantId);

    /// <summary>
    /// 获取流程定义的版本历史
    /// </summary>
    /// <param name="key">流程Key</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>版本历史列表</returns>
    Task<List<ProcessDefinitionVersionDto>> GetVersionHistoryAsync(string key, Guid tenantId);

    /// <summary>
    /// 分页查询流程定义
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>分页结果</returns>
    Task<PagedResponse<ProcessDefinitionListDto>> GetPagedAsync(PagedRequest request, Guid tenantId);
}

