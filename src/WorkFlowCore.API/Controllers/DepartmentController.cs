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

    [HttpGet("list")]
    [OperationLog("查询部门", "QUERY")]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetList()
    {
        var list = await _deptService.GetAllAsync();
        return ApiResponse<List<DepartmentDto>>.Ok(list).ToActionResult();
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
    [OperationLog("新增部门", "INSERT")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create([FromBody] DepartmentDto dto)
    {
        var result = await _deptService.CreateAsync(dto);
        return ApiResponse<DepartmentDto>.Ok(result, "创建成功").ToActionResult();
    }

    [HttpPut("{id}")]
    [OperationLog("修改部门", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update(long id, [FromBody] DepartmentDto dto)
    {
        dto.Id = id;
        await _deptService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("{id}")]
    [OperationLog("删除部门", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(long id)
    {
        await _deptService.DeleteAsync(id);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    [HttpGet("treeselect")]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> TreeSelect()
    {
        var list = await _deptService.GetAllAsync();
        return ApiResponse<List<DepartmentDto>>.Ok(list).ToActionResult();
    }
}

