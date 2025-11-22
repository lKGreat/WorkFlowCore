using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Application.Services;

public class TenantService : CrudAppService<Tenant, TenantDto, Guid, PagedAndSortedResultRequestDto, TenantDto, TenantDto>, ITenantService
{
    public TenantService(IRepository<Tenant, Guid> repository) : base(repository)
    {
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = await Repository.GetListAsync();
        return ObjectMapper.Map<List<Tenant>, List<TenantDto>>(tenants);
    }

    public async Task<PagedResponse<TenantDto>> GetPagedAsync(PagedRequest request)
    {
        var queryable = await Repository.GetQueryableAsync();
        
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

    public override async Task<TenantDto> CreateAsync(TenantDto input)
    {
        if (await Repository.AnyAsync(t => t.Code == input.Code))
        {
            throw new UserFriendlyException($"租户编码 '{input.Code}' 已存在");
        }

        var tenant = new Tenant(GuidGenerator.Create(), input.Name, input.Code)
        {
            ContactPerson = input.ContactPerson,
            ContactPhone = input.ContactPhone,
            ContactEmail = input.ContactEmail,
            IsEnabled = input.IsEnabled
        };

        await Repository.InsertAsync(tenant);

        return ObjectMapper.Map<Tenant, TenantDto>(tenant);
    }

    public override async Task<TenantDto> UpdateAsync(Guid id, TenantDto input)
    {
        var tenant = await Repository.GetAsync(id);
        
        if (await Repository.AnyAsync(t => t.Code == input.Code && t.Id != id))
        {
            throw new UserFriendlyException($"租户编码 '{input.Code}' 已存在");
        }

        tenant.Name = input.Name;
        tenant.Code = input.Code;
        tenant.ContactPerson = input.ContactPerson;
        tenant.ContactPhone = input.ContactPhone;
        tenant.ContactEmail = input.ContactEmail;
        tenant.IsEnabled = input.IsEnabled;

        await Repository.UpdateAsync(tenant);
        
        return ObjectMapper.Map<Tenant, TenantDto>(tenant);
    }
}
