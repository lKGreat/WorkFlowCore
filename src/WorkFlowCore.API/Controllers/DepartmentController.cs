using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 部门管理控制器
/// </summary>
[Authorize]
[Route("api/system/dept")]
public class DepartmentController : BaseController
{
    private readonly IDepartmentService _deptService;

    public DepartmentController(IDepartmentService deptService)
    {
        _deptService = deptService;
    }

    /// <summary>
    /// 获取部门列表（树形）
    /// </summary>
    [HttpGet("list")]
    [ActionPermissionFilter(Permission = "system:dept:list")]
    [OperationLog("查询部门", "QUERY")]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetList()
    {
        var tree = await _deptService.GetTreeAsync();
        return ApiResponse<List<DepartmentDto>>.Ok(tree).ToActionResult();
    }

    /// <summary>
    /// 获取部门列表（排除节点）
    /// </summary>
    [HttpGet("list/exclude/{deptId}")]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetListExclude(long deptId)
    {
        var tree = await _deptService.GetTreeExcludeAsync(deptId);
        return ApiResponse<List<DepartmentDto>>.Ok(tree).ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetById(long id)
    {
        var dept = await _deptService.GetByIdAsync(id);
        if (dept == null)
        {
            return ApiResponse<DepartmentDto>.Fail("部门不存在").ToActionResult();
        }
        return ApiResponse<DepartmentDto>.Ok(dept).ToActionResult();
    }

    [HttpPost]
    [ActionPermissionFilter(Permission = "system:dept:add")]
    [OperationLog("新增部门", "INSERT")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create([FromBody] DepartmentDto dto)
    {
        // 检查部门名称是否唯一
        if (!await _deptService.CheckNameUniqueAsync(dto.Name, dto.ParentId))
        {
            return ApiResponse<DepartmentDto>.Fail($"新增部门'{dto.Name}'失败，部门名称已存在").ToActionResult();
        }

        var result = await _deptService.CreateAsync(dto);
        return ApiResponse<DepartmentDto>.Ok(result, "创建成功").ToActionResult();
    }

    [HttpPut("{id}")]
    [ActionPermissionFilter(Permission = "system:dept:edit")]
    [OperationLog("修改部门", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update(long id, [FromBody] DepartmentDto dto)
    {
        dto.Id = id;

        // 检查父部门不能是自己
        if (dto.ParentId == id)
        {
            return ApiResponse<object?>.Fail("修改部门失败，上级部门不能是自己").ToActionResult();
        }

        // 检查部门名称是否唯一
        if (!await _deptService.CheckNameUniqueAsync(dto.Name, dto.ParentId, id))
        {
            return ApiResponse<object?>.Fail($"修改部门'{dto.Name}'失败，部门名称已存在").ToActionResult();
        }

        await _deptService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("{id}")]
    [ActionPermissionFilter(Permission = "system:dept:remove")]
    [OperationLog("删除部门", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(long id)
    {
        await _deptService.DeleteAsync(id);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    /// <summary>
    /// 获取部门下拉树
    /// </summary>
    [HttpGet("treeselect")]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> TreeSelect()
    {
        var tree = await _deptService.GetTreeAsync();
        return ApiResponse<List<DepartmentDto>>.Ok(tree).ToActionResult();
    }

    /// <summary>
    /// 根据角色ID查询部门树（用于角色分配部门）
    /// </summary>
    [HttpGet("roleDeptTreeselect/{roleId}")]
    public async Task<ActionResult<ApiResponse<object>>> RoleDeptTreeSelect(long roleId)
    {
        var tree = await _deptService.GetTreeAsync();
        // TODO: 获取角色已分配的部门ID列表
        var checkedKeys = new long[] { };
        return ApiResponse<object>.Ok(new { depts = tree, checkedKeys }).ToActionResult();
    }
}

