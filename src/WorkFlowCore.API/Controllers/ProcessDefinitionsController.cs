using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.Common.Exceptions;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 流程定义控制器
/// </summary>
[ApiController]
[Route("api/processdefinitions")]
public class ProcessDefinitionsController : BaseController
{
    private readonly IProcessDefinitionService _processDefinitionService;

    public ProcessDefinitionsController(IProcessDefinitionService processDefinitionService)
    {
        _processDefinitionService = processDefinitionService;
    }

    /// <summary>
    /// 创建流程定义
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建的流程定义</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProcessDefinitionDto>>> Create([FromBody] CreateProcessDefinitionRequest request)
    {
        var result = await _processDefinitionService.CreateAsync(request, CurrentTenantId);
        return ApiResponse<ProcessDefinitionDto>.Ok(result, "流程定义创建成功").ToActionResult();
    }

    /// <summary>
    /// 更新流程定义
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <param name="request">更新请求</param>
    /// <param name="createNewVersion">是否创建新版本</param>
    /// <returns>更新后的流程定义</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProcessDefinitionDto>>> Update(
        long id,
        [FromBody] UpdateProcessDefinitionRequest request,
        [FromQuery] bool createNewVersion = false)
    {
        var result = await _processDefinitionService.UpdateAsync(id, request, CurrentTenantId, createNewVersion);
        var message = createNewVersion ? "新版本创建成功" : "流程定义更新成功";
        return ApiResponse<ProcessDefinitionDto>.Ok(result, message).ToActionResult();
    }

    /// <summary>
    /// 删除流程定义（软删除）
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        await _processDefinitionService.DeleteAsync(id, CurrentTenantId);
        return ApiResponse.Ok("流程定义删除成功").ToActionResult();
    }

    /// <summary>
    /// 获取单个流程定义
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>流程定义</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProcessDefinitionDto>>> GetById(long id)
    {
        var result = await _processDefinitionService.GetByIdAsync(id, CurrentTenantId);
        if (result == null)
        {
            throw new NotFoundException("流程定义不存在");
        }
        return ApiResponse<ProcessDefinitionDto>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取流程定义的最新版本
    /// </summary>
    /// <param name="key">流程Key</param>
    /// <returns>最新版本的流程定义</returns>
    [HttpGet("by-key/{key}/latest")]
    public async Task<ActionResult<ApiResponse<ProcessDefinitionDto>>> GetLatestVersion(string key)
    {
        var result = await _processDefinitionService.GetLatestVersionAsync(key, CurrentTenantId);
        if (result == null)
        {
            throw new NotFoundException("流程定义不存在");
        }
        return ApiResponse<ProcessDefinitionDto>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取流程定义的版本历史
    /// </summary>
    /// <param name="key">流程Key</param>
    /// <returns>版本历史列表</returns>
    [HttpGet("by-key/{key}/versions")]
    public async Task<ActionResult<ApiResponse<List<ProcessDefinitionVersionDto>>>> GetVersionHistory(string key)
    {
        var result = await _processDefinitionService.GetVersionHistoryAsync(key, CurrentTenantId);
        return ApiResponse<List<ProcessDefinitionVersionDto>>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 分页查询流程定义
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>分页结果</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<ProcessDefinitionListDto>>>> GetPaged(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest { PageIndex = pageIndex, PageSize = pageSize };
        var result = await _processDefinitionService.GetPagedAsync(request, CurrentTenantId);
        return ApiResponse<PagedResponse<ProcessDefinitionListDto>>.Ok(result).ToActionResult();
    }
}
