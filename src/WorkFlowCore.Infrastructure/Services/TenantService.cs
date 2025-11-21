using AutoMapper;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Repositories;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 租户服务实现
/// </summary>
public class TenantService : ITenantService
{
    private readonly IPagedRepository<Tenant> _repository;
    private readonly IMapper _mapper;

    public TenantService(IPagedRepository<Tenant> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _repository.GetByIdAsync(id);
        return tenant == null ? null : _mapper.Map<TenantDto>(tenant);
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = await _repository.GetAllAsync();
        return _mapper.Map<List<TenantDto>>(tenants);
    }

    public async Task<TenantDto> CreateAsync(TenantDto dto)
    {
        // 验证租户编码唯一性
        var exists = await _repository.FindAsync(t => t.Code == dto.Code);
        if (exists.Any())
        {
            throw new InvalidOperationException($"租户编码 '{dto.Code}' 已存在");
        }

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

        return _mapper.Map<TenantDto>(tenant);
    }

    public async Task UpdateAsync(TenantDto dto)
    {
        var tenant = await _repository.GetByIdAsync(dto.Id);
        if (tenant == null)
        {
            throw new KeyNotFoundException($"租户 ID '{dto.Id}' 不存在");
        }

        // 验证编码唯一性（排除自己）
        var exists = await _repository.FindAsync(t => t.Code == dto.Code && t.Id != dto.Id);
        if (exists.Any())
        {
            throw new InvalidOperationException($"租户编码 '{dto.Code}' 已存在");
        }

        tenant.Name = dto.Name;
        tenant.Code = dto.Code;
        tenant.ContactPerson = dto.ContactPerson;
        tenant.ContactPhone = dto.ContactPhone;
        tenant.ContactEmail = dto.ContactEmail;
        tenant.IsEnabled = dto.IsEnabled;

        await _repository.UpdateAsync(tenant);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenant = await _repository.GetByIdAsync(id);
        if (tenant == null)
        {
            throw new KeyNotFoundException($"租户 ID '{id}' 不存在");
        }

        tenant.IsDeleted = true;
        tenant.DeletedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(tenant);
        await _repository.SaveChangesAsync();
    }
}

