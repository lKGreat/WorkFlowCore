using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Application.Services;

public class TenantService : ApplicationService, ITenantService
{
    private readonly IRepository<Tenant, Guid> _repository;

    public TenantService(IRepository<Tenant, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _repository.FindAsync(id);
        return tenant == null ? null : ObjectMapper.Map<Tenant, TenantDto>(tenant);
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = await _repository.GetListAsync();
        return ObjectMapper.Map<List<Tenant>, List<TenantDto>>(tenants);
    }

    public async Task<PagedResponse<TenantDto>> GetPagedAsync(PagedRequest request)
    {
        var queryable = await _repository.GetQueryableAsync();
        
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            queryable = queryable.Where(t => t.Name.Contains(request.Keyword) || t.Code.Contains(request.Keyword));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        
        queryable = queryable.OrderBy(t => t.CreationTime)
                             .Skip(request.Skip)
                             .Take(request.Take);

        var entities = await AsyncExecuter.ToListAsync(queryable);
        var dtos = ObjectMapper.Map<List<Tenant>, List<TenantDto>>(entities);

        return PagedResponse<TenantDto>.Create(dtos, totalCount, request.PageIndex, request.PageSize);
    }

    public async Task<TenantDto> CreateAsync(TenantDto dto)
    {
        if (await _repository.AnyAsync(t => t.Code == dto.Code))
        {
            throw new UserFriendlyException($"租户编码 '{dto.Code}' 已存在");
        }

        var tenant = new Tenant(GuidGenerator.Create(), dto.Name, dto.Code)
        {
            ContactPerson = dto.ContactPerson,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            IsEnabled = dto.IsEnabled
        };

        await _repository.InsertAsync(tenant);

        return ObjectMapper.Map<Tenant, TenantDto>(tenant);
    }

    public async Task UpdateAsync(TenantDto dto)
    {
        var tenant = await _repository.GetAsync(dto.Id);
        
        if (await _repository.AnyAsync(t => t.Code == dto.Code && t.Id != dto.Id))
        {
            throw new UserFriendlyException($"租户编码 '{dto.Code}' 已存在");
        }

        tenant.Name = dto.Name;
        tenant.Code = dto.Code;
        tenant.ContactPerson = dto.ContactPerson;
        tenant.ContactPhone = dto.ContactPhone;
        tenant.ContactEmail = dto.ContactEmail;
        tenant.IsEnabled = dto.IsEnabled;

        await _repository.UpdateAsync(tenant);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
