using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

[Authorize]
[Route("api/monitor/operlog")]
public class OperationLogController : BaseController
{
    private readonly IOperationLogService _logService;

    public OperationLogController(IOperationLogService logService)
    {
        _logService = logService;
    }

    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<PagedResponse<OperationLogDto>>>> GetPaged([FromQuery] OperationLogPagedRequest request)
    {
        var result = await _logService.GetPagedAsync(request);
        return ApiResponse<PagedResponse<OperationLogDto>>.Ok(result).ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OperationLogDto>>> GetById(long id)
    {
        var log = await _logService.GetByIdAsync(id);
        if (log == null) return ApiResponse<OperationLogDto>.Fail("日志不存在").ToActionResult();
        return ApiResponse<OperationLogDto>.Ok(log).ToActionResult();
    }

    [HttpDelete("{ids}")]
    [OperationLog("删除操作日志", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string ids)
    {
        var idList = ids.Split(',').Select(long.Parse).ToList();
        await _logService.DeleteAsync(idList);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    [HttpDelete("clean")]
    [OperationLog("清空操作日志", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Clean()
    {
        await _logService.CleanAsync();
        return ApiResponse<object?>.Ok(null, "清空成功").ToActionResult();
    }
}

