using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs.User;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[Authorize]
[Route("api/system/user")]
public class UserController : BaseController
{
    private readonly IAppUserService _userService;

    public UserController(IAppUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    [HttpGet("list")]
    [OperationLog("查询用户", "QUERY")]
    public async Task<ActionResult<ApiResponse<Domain.Common.PagedResponse<UserListDto>>>> GetPaged([FromQuery] UserPagedRequest request)
    {
        var result = await _userService.GetPagedAsync(request);
        return ApiResponse<Domain.Common.PagedResponse<UserListDto>>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserListDto>>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserListDto>.Fail("用户不存在").ToActionResult();
        }

        return ApiResponse<UserListDto>.Ok(user).ToActionResult();
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    [OperationLog("新增用户", "INSERT")]
    public async Task<ActionResult<ApiResponse<UserListDto>>> Create([FromBody] CreateUserInput input)
    {
        var user = await _userService.CreateAsync(input);
        return ApiResponse<UserListDto>.Ok(user, "创建成功").ToActionResult();
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id}")]
    [OperationLog("修改用户", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update(Guid id, [FromBody] UpdateUserInput input)
    {
        input.UserId = id;
        await _userService.UpdateAsync(input);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    /// <summary>
    /// 删除用户(批量)
    /// </summary>
    [HttpDelete("{ids}")]
    [OperationLog("删除用户", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string ids)
    {
        var idList = ids.Split(',').Select(Guid.Parse).ToList();
        await _userService.DeleteAsync(idList);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    [HttpPut("resetPwd")]
    [OperationLog("重置密码", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> ResetPassword([FromBody] ResetPasswordInput input)
    {
        await _userService.ResetPasswordAsync(input);
        return ApiResponse<object?>.Ok(null, "密码重置成功").ToActionResult();
    }

    /// <summary>
    /// 更改状态
    /// </summary>
    [HttpPost("changeStatus")]
    [OperationLog("更改用户状态", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> ChangeStatus([FromBody] ChangeStatusInput input)
    {
        await _userService.ChangeStatusAsync(input);
        return ApiResponse<object?>.Ok(null, "状态更新成功").ToActionResult();
    }
}

