using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs.Role;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[Authorize]
[Route("api/system/role")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("list")]
    [OperationLog("查询角色", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<RoleDto>>>> GetPaged([FromQuery] PagedRequest request)
    {
        var result = await _roleService.GetPagedAsync(request);
        return ApiResponse<PagedResponse<RoleDto>>.Ok(result).ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
        {
            return ApiResponse<RoleDto>.Fail("角色不存在").ToActionResult();
        }
        return ApiResponse<RoleDto>.Ok(role).ToActionResult();
    }

    [HttpPost]
    [OperationLog("新增角色", "INSERT")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Create([FromBody] CreateRoleInput input)
    {
        var role = await _roleService.CreateAsync(input);
        return ApiResponse<RoleDto>.Ok(role, "创建成功").ToActionResult();
    }

    [HttpPut("{id}")]
    [OperationLog("修改角色", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update(Guid id, [FromBody] UpdateRoleInput input)
    {
        input.RoleId = id.ToString();
        await _roleService.UpdateAsync(input);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("{ids}")]
    [OperationLog("删除角色", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string ids)
    {
        var idList = ids.Split(',').Select(Guid.Parse).ToList();
        await _roleService.DeleteAsync(idList);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    [HttpGet("optionselect")]
    public async Task<ActionResult<ApiResponse<List<RoleOption>>>> GetOptions()
    {
        var options = await _roleService.GetOptionsAsync();
        return ApiResponse<List<RoleOption>>.Ok(options).ToActionResult();
    }

    [HttpPut("{id}/menu")]
    [OperationLog("分配角色菜单", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> AssignMenus(Guid id, [FromBody] List<long> menuIds)
    {
        await _roleService.AssignMenusAsync(id, menuIds);
        return ApiResponse<object?>.Ok(null, "菜单分配成功").ToActionResult();
    }

    [HttpGet("{id}/menu")]
    public async Task<ActionResult<ApiResponse<List<long>>>> GetRoleMenuIds(Guid id)
    {
        var menuIds = await _roleService.GetRoleMenuIdsAsync(id);
        return ApiResponse<List<long>>.Ok(menuIds).ToActionResult();
    }
}

