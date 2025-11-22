using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 租户管理控制器
/// </summary>
public class TenantsController : BaseController
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// 获取租户分页列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<TenantDto>>>> GetPaged([FromQuery] PagedRequest request)
    {
        var result = await _tenantService.GetPagedAsync(request);
        return ApiResponse<PagedResponse<TenantDto>>.Ok(result, "获取成功").ToActionResult();
    }

    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> GetById(long id)
    {
        var dto = await _tenantService.GetAsync(id);
        
        if (dto == null)
        {
            return ApiResponse<TenantDto>.Fail("租户不存在", ErrorCodes.NotFound).ToActionResult();
        }

        return ApiResponse<TenantDto>.Ok(dto).ToActionResult();
    }

    /// <summary>
    /// 创建租户
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantDto>>> Create([FromBody] TenantDto dto)
    {
        var result = await _tenantService.CreateAsync(dto);
        return ApiResponse<TenantDto>.Ok(result, "创建成功").ToActionResult();
    }

    /// <summary>
    /// 更新租户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] TenantDto dto)
    {
        if (id != dto.Id)
        {
            return ApiResponse.Fail("租户不存在", ErrorCodes.NotFound).ToActionResult();
        }

        await _tenantService.UpdateAsync(id, dto);
        return ApiResponse.Ok("更新成功").ToActionResult();
    }

    /// <summary>
    /// 删除租户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        await _tenantService.DeleteAsync(id);
        return ApiResponse.Ok("删除成功").ToActionResult();
    }
}
