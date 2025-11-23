using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

[Authorize]
[Route("api/system/config")]
public class ConfigController : BaseController
{
    private readonly IConfigService _configService;

    public ConfigController(IConfigService configService)
    {
        _configService = configService;
    }

    [HttpGet("list")]
    [OperationLog("查询系统配置", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<ConfigDto>>>> GetPaged([FromQuery] PagedRequest request)
    {
        var result = await _configService.GetPagedAsync(request);
        return ApiResponse<PagedResponse<ConfigDto>>.Ok(result).ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ConfigDto>>> GetById(long id)
    {
        var config = await _configService.GetByIdAsync(id);
        if (config == null) return ApiResponse<ConfigDto>.Fail("配置不存在").ToActionResult();
        return ApiResponse<ConfigDto>.Ok(config).ToActionResult();
    }

    [HttpGet("configKey/{key}")]
    public async Task<ActionResult<ApiResponse<string>>> GetByKey(string key)
    {
        var value = await _configService.GetByKeyAsync(key);
        if (value == null) return ApiResponse<string>.Fail("配置不存在").ToActionResult();
        return ApiResponse<string>.Ok(value).ToActionResult();
    }

    [HttpPost]
    [OperationLog("新增配置", "INSERT")]
    public async Task<ActionResult<ApiResponse<ConfigDto>>> Create([FromBody] ConfigDto dto)
    {
        var result = await _configService.CreateAsync(dto);
        return ApiResponse<ConfigDto>.Ok(result, "创建成功").ToActionResult();
    }

    [HttpPut("{id}")]
    [OperationLog("修改配置", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update(long id, [FromBody] ConfigDto dto)
    {
        dto.ConfigId = id;
        await _configService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    [HttpDelete("{ids}")]
    [OperationLog("删除配置", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string ids)
    {
        var idList = ids.Split(',').Select(long.Parse).ToList();
        await _configService.DeleteAsync(idList);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }
}

