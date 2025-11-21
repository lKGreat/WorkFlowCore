using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 部门服务接口
/// </summary>
public interface IDepartmentService
{
    Task<DepartmentDto?> GetByIdAsync(Guid id);
    Task<List<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto> CreateAsync(DepartmentDto dto);
    Task UpdateAsync(DepartmentDto dto);
    Task DeleteAsync(Guid id);
}

