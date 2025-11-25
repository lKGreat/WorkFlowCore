using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 定时任务控制器
/// </summary>
[Authorize]
[Route("api/monitor/job")]
public class TaskController : BaseController
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("list")]
    [ActionPermissionFilter(Permission = "monitor:job:list")]
    [OperationLog("查询定时任务", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<TaskDto>>>> GetList([FromQuery] TaskQueryDto query)
    {
        var result = await _taskService.GetPagedListAsync(query);
        return ApiResponse<PagedResponse<TaskDto>>.Ok(result).ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> GetById(long id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
        {
            return ApiResponse<TaskDto>.Fail("任务不存在").ToActionResult();
        }
        return ApiResponse<TaskDto>.Ok(task).ToActionResult();
    }

    [HttpPost]
    [ActionPermissionFilter(Permission = "monitor:job:add")]
    [OperationLog("新增定时任务", "INSERT")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> Create([FromBody] TaskDto dto)
    {
        var result = await _taskService.CreateAsync(dto);
        return ApiResponse<TaskDto>.Ok(result, "创建成功").ToActionResult();
    }

    [HttpPut]
    [ActionPermissionFilter(Permission = "monitor:job:edit")]
    [OperationLog("修改定时任务", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update([FromBody] TaskDto dto)
    {
        await _taskService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("{ids}")]
    [ActionPermissionFilter(Permission = "monitor:job:remove")]
    [OperationLog("删除定时任务", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string ids)
    {
        var idArray = ids.Split(',').Select(long.Parse).ToArray();
        await _taskService.DeleteAsync(idArray);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    [HttpGet("log/list")]
    [ActionPermissionFilter(Permission = "monitor:job:list")]
    public async Task<ActionResult<ApiResponse<PagedResponse<TaskLogDto>>>> GetLogList([FromQuery] TaskLogQueryDto query)
    {
        var result = await _taskService.GetLogPagedListAsync(query);
        return ApiResponse<PagedResponse<TaskLogDto>>.Ok(result).ToActionResult();
    }

    [HttpDelete("log/clear/{taskId}")]
    [ActionPermissionFilter(Permission = "monitor:job:remove")]
    [OperationLog("清空任务日志", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> ClearLogs(long taskId)
    {
        await _taskService.ClearLogsAsync(taskId);
        return ApiResponse<object?>.Ok(null, "清空成功").ToActionResult();
    }
}

