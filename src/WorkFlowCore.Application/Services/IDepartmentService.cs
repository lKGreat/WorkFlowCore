using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 部门服务接口
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    Task<DepartmentDto?> GetByIdAsync(long id);

    /// <summary>
    /// 获取所有部门（列表）
    /// </summary>
    Task<List<DepartmentDto>> GetAllAsync();

    /// <summary>
    /// 获取部门树
    /// </summary>
    Task<List<DepartmentDto>> GetTreeAsync();

    /// <summary>
    /// 获取部门树（排除指定节点及其子节点）
    /// </summary>
    Task<List<DepartmentDto>> GetTreeExcludeAsync(long excludeId);

    /// <summary>
    /// 创建部门
    /// </summary>
    Task<DepartmentDto> CreateAsync(DepartmentDto dto);

    /// <summary>
    /// 更新部门
    /// </summary>
    Task UpdateAsync(DepartmentDto dto);

    /// <summary>
    /// 删除部门
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 检查部门名称是否唯一
    /// </summary>
    Task<bool> CheckNameUniqueAsync(string name, long? parentId, long? excludeId = null);
}

