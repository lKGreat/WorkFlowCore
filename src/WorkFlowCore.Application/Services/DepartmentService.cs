using System;
using System.Collections.Generic;
using System.Linq;
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
        var dtos = ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        
        // 设置父部门名称
        foreach (var dto in dtos)
        {
            if (dto.ParentId.HasValue && dto.ParentId.Value > 0)
            {
                var parent = departments.FirstOrDefault(d => d.Id == dto.ParentId.Value);
                dto.ParentName = parent?.DeptName;
            }
        }
        
        return dtos;
    }

    public async Task<List<DepartmentDto>> GetTreeAsync()
    {
        var departments = await _repository.GetListAsync();
        var dtos = ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        return BuildTree(dtos, null);
    }

    public async Task<List<DepartmentDto>> GetTreeExcludeAsync(long excludeId)
    {
        var departments = await _repository.GetListAsync();
        var dtos = ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        
        // 排除指定节点及其所有子孙节点
        var excludeIds = new HashSet<long> { excludeId };
        FindAllChildrenIds(dtos, excludeId, excludeIds);
        
        var filtered = dtos.Where(d => !excludeIds.Contains(d.Id)).ToList();
        return BuildTree(filtered, null);
    }

    public async Task<bool> CheckNameUniqueAsync(string name, long? parentId, long? excludeId = null)
    {
        var departments = await _repository.GetListAsync();
        var exists = departments.Any(d => 
            d.DeptName == name && 
            d.ParentId == parentId && 
            (excludeId == null || d.Id != excludeId.Value));
        return !exists;
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

        // 计算祖级列表
        var ancestors = "0";
        if (dto.ParentId.HasValue && dto.ParentId.Value > 0)
        {
            var parent = await _repository.GetAsync(dto.ParentId.Value);
            ancestors = parent.Ancestors + "," + dto.ParentId.Value;
        }

        var department = new Department(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, dto.Name)
        {
            Code = dto.Code,
            ParentId = dto.ParentId,
            Ancestors = ancestors,
            ManagerId = dto.ManagerId,
            Leader = dto.ManagerName,
            OrderNum = dto.SortOrder,
            Status = dto.Status
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

        // 计算祖级列表
        var ancestors = "0";
        if (dto.ParentId.HasValue && dto.ParentId.Value > 0)
        {
            var parent = await _repository.GetAsync(dto.ParentId.Value);
            ancestors = parent.Ancestors + "," + dto.ParentId.Value;
        }

        department.DeptName = dto.Name;
        department.Code = dto.Code;
        department.ParentId = dto.ParentId;
        department.Ancestors = ancestors;
        department.ManagerId = dto.ManagerId;
        department.Leader = dto.ManagerName;
        department.OrderNum = dto.SortOrder;
        department.Status = dto.Status;

        await _repository.UpdateAsync(department);
    }

    public async Task DeleteAsync(long id)
    {
        if (await _repository.AnyAsync(d => d.ParentId == id))
        {
            throw new UserFriendlyException("该部门下存在子部门，无法删除");
        }

        // TODO: 检查部门下是否有用户
        // if (await _userRepository.AnyAsync(u => u.DepartmentId == id))
        // {
        //     throw new UserFriendlyException("该部门下存在用户，无法删除");
        // }

        await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// 构建部门树
    /// </summary>
    private List<DepartmentDto> BuildTree(List<DepartmentDto> departments, long? parentId)
    {
        var children = departments.Where(d => d.ParentId == parentId).OrderBy(d => d.SortOrder).ToList();
        
        foreach (var child in children)
        {
            child.Children = BuildTree(departments, child.Id);
            if (child.Children.Count == 0)
            {
                child.Children = null;
            }
        }

        return children;
    }

    /// <summary>
    /// 递归查找所有子孙节点ID
    /// </summary>
    private void FindAllChildrenIds(List<DepartmentDto> departments, long parentId, HashSet<long> result)
    {
        var children = departments.Where(d => d.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            result.Add(child.Id);
            FindAllChildrenIds(departments, child.Id, result);
        }
    }
}
