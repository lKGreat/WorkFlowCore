using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

[Authorize]
[Route("api/system/dict")]
public class DictController : BaseController
{
    private readonly IDictService _dictService;

    public DictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    // 字典类型
    [HttpGet("type/list")]
    [OperationLog("查询字典类型", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<DictTypeDto>>>> GetTypes([FromQuery] PagedRequest request)
    {
        var result = await _dictService.GetTypesPagedAsync(request);
        return ApiResponse<PagedResponse<DictTypeDto>>.Ok(result).ToActionResult();
    }

    [HttpGet("type/{id}")]
    public async Task<ActionResult<ApiResponse<DictTypeDto>>> GetTypeById(long id)
    {
        var dto = await _dictService.GetTypeByIdAsync(id);
        if (dto == null) return ApiResponse<DictTypeDto>.Fail("字典类型不存在").ToActionResult();
        return ApiResponse<DictTypeDto>.Ok(dto).ToActionResult();
    }

    [HttpPost("type")]
    [OperationLog("新增字典类型", "INSERT")]
    public async Task<ActionResult<ApiResponse<DictTypeDto>>> CreateType([FromBody] DictTypeDto dto)
    {
        var result = await _dictService.CreateTypeAsync(dto);
        return ApiResponse<DictTypeDto>.Ok(result, "创建成功").ToActionResult();
    }

    [HttpPut("type/{id}")]
    [OperationLog("修改字典类型", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> UpdateType(long id, [FromBody] DictTypeDto dto)
    {
        dto.DictId = id;
        await _dictService.UpdateTypeAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("type/{ids}")]
    [OperationLog("删除字典类型", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteType(string ids)
    {
        var idList = ids.Split(',').Select(long.Parse).ToList();
        await _dictService.DeleteTypeAsync(idList);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    // 字典数据
    [HttpGet("data/list")]
    [OperationLog("查询字典数据", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<DictDataDto>>>> GetData(
        [FromQuery] PagedRequest request,
        [FromQuery] long? dictTypeId)
    {
        var result = await _dictService.GetDataPagedAsync(request, dictTypeId);
        return ApiResponse<PagedResponse<DictDataDto>>.Ok(result).ToActionResult();
    }

    [HttpGet("data/type/{dictType}")]
    public async Task<ActionResult<ApiResponse<List<DictDataDto>>>> GetDataByType(string dictType)
    {
        var list = await _dictService.GetDataByTypeCodeAsync(dictType);
        return ApiResponse<List<DictDataDto>>.Ok(list).ToActionResult();
    }

    [HttpPost("data")]
    [OperationLog("新增字典数据", "INSERT")]
    public async Task<ActionResult<ApiResponse<DictDataDto>>> CreateData([FromBody] DictDataDto dto)
    {
        var result = await _dictService.CreateDataAsync(dto);
        return ApiResponse<DictDataDto>.Ok(result, "创建成功").ToActionResult();
    }

    [HttpPut("data/{id}")]
    [OperationLog("修改字典数据", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> UpdateData(long id, [FromBody] DictDataDto dto)
    {
        dto.DictCode = id;
        await _dictService.UpdateDataAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("data/{ids}")]
    [OperationLog("删除字典数据", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteData(string ids)
    {
        var idList = ids.Split(',').Select(long.Parse).ToList();
        await _dictService.DeleteDataAsync(idList);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }
}

