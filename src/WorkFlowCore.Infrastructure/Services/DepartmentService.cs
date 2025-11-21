using AutoMapper;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Repositories;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 部门服务实现
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IPagedRepository<Department> _repository;
    private readonly IMapper _mapper;

    public DepartmentService(IPagedRepository<Department> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto?> GetByIdAsync(Guid id)
    {
        var department = await _repository.GetByIdAsync(id);
        return department == null ? null : _mapper.Map<DepartmentDto>(department);
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();
        return _mapper.Map<List<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
    {
        // 如果指定了父部门，验证父部门是否存在
        if (dto.ParentId.HasValue)
        {
            var parentExists = await _repository.GetByIdAsync(dto.ParentId.Value);
            if (parentExists == null)
            {
                throw new KeyNotFoundException($"父部门 ID '{dto.ParentId}' 不存在");
            }
        }

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            ParentId = dto.ParentId,
            ManagerId = dto.ManagerId,
            SortOrder = dto.SortOrder
        };

        await _repository.AddAsync(department);
        await _repository.SaveChangesAsync();

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task UpdateAsync(DepartmentDto dto)
    {
        var department = await _repository.GetByIdAsync(dto.Id);
        if (department == null)
        {
            throw new KeyNotFoundException($"部门 ID '{dto.Id}' 不存在");
        }

        // 如果指定了父部门，验证父部门是否存在且不是自己
        if (dto.ParentId.HasValue)
        {
            if (dto.ParentId.Value == dto.Id)
            {
                throw new InvalidOperationException("父部门不能是自己");
            }

            var parentExists = await _repository.GetByIdAsync(dto.ParentId.Value);
            if (parentExists == null)
            {
                throw new KeyNotFoundException($"父部门 ID '{dto.ParentId}' 不存在");
            }
        }

        department.Name = dto.Name;
        department.Code = dto.Code;
        department.ParentId = dto.ParentId;
        department.ManagerId = dto.ManagerId;
        department.SortOrder = dto.SortOrder;

        await _repository.UpdateAsync(department);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department == null)
        {
            throw new KeyNotFoundException($"部门 ID '{id}' 不存在");
        }

        // 检查是否有子部门
        var hasChildren = await _repository.FindAsync(d => d.ParentId == id);
        if (hasChildren.Any())
        {
            throw new InvalidOperationException("该部门下存在子部门，无法删除");
        }

        department.IsDeleted = true;
        department.DeletedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(department);
        await _repository.SaveChangesAsync();
    }
}

