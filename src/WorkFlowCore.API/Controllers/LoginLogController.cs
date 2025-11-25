using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 登录日志控制器
/// </summary>
[Authorize]
[Route("api/monitor/logininfor")]
public class LoginLogController : BaseController
{
    private readonly ILoginLogService _loginLogService;

    public LoginLogController(ILoginLogService loginLogService)
    {
        _loginLogService = loginLogService;
    }

    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    [HttpGet("list")]
    [ActionPermissionFilter(Permission = "monitor:logininfor:list")]
    [OperationLog("查询登录日志", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<LoginLogDto>>>> GetList([FromQuery] LoginLogQueryDto query)
    {
        var result = await _loginLogService.GetPagedListAsync(query);
        return ApiResponse<PagedResponse<LoginLogDto>>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取我的登录日志
    /// </summary>
    [HttpGet("mylist")]
    public async Task<ActionResult<ApiResponse<PagedResponse<LoginLogDto>>>> GetMyList([FromQuery] LoginLogQueryDto query)
    {
        query.UserId = CurrentUser.Id;
        var result = await _loginLogService.GetPagedListAsync(query);
        return ApiResponse<PagedResponse<LoginLogDto>>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 删除登录日志
    /// </summary>
    [HttpDelete("{infoIds}")]
    [ActionPermissionFilter(Permission = "monitor:logininfor:remove")]
    [OperationLog("删除登录日志", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string infoIds)
    {
        var ids = infoIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        await _loginLogService.DeleteAsync(ids);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    /// <summary>
    /// 清空登录日志
    /// </summary>
    [HttpDelete("clean")]
    [ActionPermissionFilter(Permission = "monitor:logininfor:remove")]
    [OperationLog("清空登录日志", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Clean()
    {
        await _loginLogService.TruncateAsync();
        return ApiResponse<object?>.Ok(null, "清空成功").ToActionResult();
    }

    /// <summary>
    /// 获取登录日志统计（最近7天）
    /// </summary>
    [HttpGet("statiLoginLog")]
    [ActionPermissionFilter(Permission = "common")]
    public async Task<ActionResult<ApiResponse<object>>> GetStatistics()
    {
        var list = await _loginLogService.GetStatisticsAsync();
        var categories = list.Select(x => x.Date.ToString("dd日")).ToList();
        var numList = list.Select(x => x.Num).ToList();

        return ApiResponse<object>.Ok(new { categories, numList }).ToActionResult();
    }
}

