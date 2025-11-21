using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Repositories;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 租户管理控制器（示例）
/// </summary>
public class TenantsController : BaseController
{
    private readonly IPagedRepository<Tenant> _repository;

    public TenantsController(IPagedRepository<Tenant> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取租户分页列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<TenantDto>>>> GetPaged([FromQuery] PagedRequest request)
    {
        var result = await _repository.GetPagedAsync<TenantDto>(
            request.PageIndex,
            request.PageSize,
            predicate: t => string.IsNullOrEmpty(request.Keyword) || t.Name.Contains(request.Keyword),
            selector: t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Code = t.Code,
                ContactPerson = t.ContactPerson,
                ContactPhone = t.ContactPhone,
                ContactEmail = t.ContactEmail,
                IsEnabled = t.IsEnabled
            },
            orderBy: t => t.CreatedAt,
            descending: true
        );

        return ApiResponse<PagedResponse<TenantDto>>.Ok(result, "获取成功").ToActionResult();
    }

    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> GetById(Guid id)
    {
        var tenant = await _repository.GetByIdAsync(id);
        
        if (tenant == null)
        {
            return ApiResponse<TenantDto>.Fail("租户不存在", "NOT_FOUND").ToActionResult();
        }

        var dto = new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Code = tenant.Code,
            ContactPerson = tenant.ContactPerson,
            ContactPhone = tenant.ContactPhone,
            ContactEmail = tenant.ContactEmail,
            IsEnabled = tenant.IsEnabled
        };

        return ApiResponse<TenantDto>.Ok(dto).ToActionResult();
    }

    /// <summary>
    /// 创建租户
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantDto>>> Create([FromBody] TenantDto dto)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            ContactPerson = dto.ContactPerson,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            IsEnabled = dto.IsEnabled
        };

        await _repository.AddAsync(tenant);
        await _repository.SaveChangesAsync();

        dto.Id = tenant.Id;
        return ApiResponse<TenantDto>.Ok(dto, "创建成功").ToActionResult();
    }

    /// <summary>
    /// 更新租户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> Update(Guid id, [FromBody] TenantDto dto)
    {
        var tenant = await _repository.GetByIdAsync(id);
        
        if (tenant == null)
        {
            return ApiResponse.Fail("租户不存在", "NOT_FOUND").ToActionResult();
        }

        tenant.Name = dto.Name;
        tenant.Code = dto.Code;
        tenant.ContactPerson = dto.ContactPerson;
        tenant.ContactPhone = dto.ContactPhone;
        tenant.ContactEmail = dto.ContactEmail;
        tenant.IsEnabled = dto.IsEnabled;

        await _repository.UpdateAsync(tenant);
        await _repository.SaveChangesAsync();

        return ApiResponse.Ok("更新成功").ToActionResult();
    }

    /// <summary>
    /// 删除租户（软删除）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        var tenant = await _repository.GetByIdAsync(id);
        
        if (tenant == null)
        {
            return ApiResponse.Fail("租户不存在", "NOT_FOUND").ToActionResult();
        }

        tenant.IsDeleted = true;
        tenant.DeletedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(tenant);
        await _repository.SaveChangesAsync();

        return ApiResponse.Ok("删除成功").ToActionResult();
    }
}

