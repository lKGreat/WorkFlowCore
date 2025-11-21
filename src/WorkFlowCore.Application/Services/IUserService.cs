using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(UserDto dto);
    Task UpdateAsync(UserDto dto);
    Task DeleteAsync(Guid id);
}

