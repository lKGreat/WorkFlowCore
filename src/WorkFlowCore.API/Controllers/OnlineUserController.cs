using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 在线用户监控控制器
/// </summary>
[Authorize]
[Route("api/monitor/online")]
public class OnlineUserController : BaseController
{
    private readonly IOnlineUserService _onlineUserService;

    public OnlineUserController(IOnlineUserService onlineUserService)
    {
        _onlineUserService = onlineUserService;
    }

    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    [HttpGet("list")]
    [ActionPermissionFilter(Permission = "monitor:online:list")]
    [OperationLog("查询在线用户", "QUERY")]
    public async Task<ActionResult<ApiResponse<List<OnlineUserDto>>>> GetList()
    {
        var users = await _onlineUserService.GetOnlineUsersAsync();
        return ApiResponse<List<OnlineUserDto>>.Ok(users).ToActionResult();
    }

    /// <summary>
    /// 强制用户下线
    /// </summary>
    [HttpDelete("{userId}")]
    [ActionPermissionFilter(Permission = "monitor:online:forceLogout")]
    [OperationLog("强制用户下线", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> ForceLogout(Guid userId)
    {
        await _onlineUserService.ForceLogoutAsync(userId);
        return ApiResponse<object?>.Ok(null, "强制下线成功").ToActionResult();
    }
}

