using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Application.Services;

public class DepartmentService : ApplicationService, IDepartmentService
{
    private readonly IRepository<Department, long> _repository;

    public DepartmentService(IRepository<Department, long> repository)
    {
        _repository = repository;
    }

    public async Task<DepartmentDto?> GetByIdAsync(long id)
    {
        var department = await _repository.FindAsync(id);
        return department == null ? null : ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var departments = await _repository.GetListAsync();
        return ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
    {
        if (dto.ParentId.HasValue)
        {
            if (!await _repository.AnyAsync(d => d.Id == dto.ParentId.Value))
            {
                throw new UserFriendlyException($"父部门 ID '{dto.ParentId}' 不存在");
            }
        }

        var department = new Department(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, dto.Name)
        {
            Code = dto.Code,
            ParentId = dto.ParentId,
            ManagerId = dto.ManagerId,
            SortOrder = dto.SortOrder
        };

        await _repository.InsertAsync(department);

        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public async Task UpdateAsync(DepartmentDto dto)
    {
        var department = await _repository.GetAsync(dto.Id);

        if (dto.ParentId.HasValue)
        {
            if (dto.ParentId.Value == dto.Id)
            {
                throw new UserFriendlyException("父部门不能是自己");
            }

            if (!await _repository.AnyAsync(d => d.Id == dto.ParentId.Value))
            {
                throw new UserFriendlyException($"父部门 ID '{dto.ParentId}' 不存在");
            }
        }

        department.Name = dto.Name;
        department.Code = dto.Code;
        department.ParentId = dto.ParentId;
        department.ManagerId = dto.ManagerId;
        department.SortOrder = dto.SortOrder;

        await _repository.UpdateAsync(department);
    }

    public async Task DeleteAsync(long id)
    {
        if (await _repository.AnyAsync(d => d.ParentId == id))
        {
            throw new UserFriendlyException("该部门下存在子部门，无法删除");
        }

        await _repository.DeleteAsync(id);
    }
}
