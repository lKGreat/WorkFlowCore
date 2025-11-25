using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 服务器监控控制器
/// </summary>
[Authorize]
[Route("api/monitor/server")]
public class ServerMonitorController : BaseController
{
    private readonly IServerMonitorService _serverMonitorService;

    public ServerMonitorController(IServerMonitorService serverMonitorService)
    {
        _serverMonitorService = serverMonitorService;
    }

    /// <summary>
    /// 获取服务器信息
    /// </summary>
    [HttpGet]
    [ActionPermissionFilter(Permission = "monitor:server:list")]
    [OperationLog("查询服务器信息", "QUERY")]
    public async Task<ActionResult<ApiResponse<ServerInfoDto>>> GetServerInfo()
    {
        var info = await _serverMonitorService.GetServerInfoAsync();
        return ApiResponse<ServerInfoDto>.Ok(info).ToActionResult();
    }
}

