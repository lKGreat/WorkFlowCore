using AutoMapper;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Repositories;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IPagedRepository<User> _repository;
    private readonly IMapper _mapper;

    public UserService(IPagedRepository<User> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(UserDto dto)
    {
        // 验证用户名唯一性
        var exists = await _repository.FindAsync(u => u.UserName == dto.UserName);
        if (exists.Any())
        {
            throw new InvalidOperationException($"用户名 '{dto.UserName}' 已存在");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            RealName = dto.RealName,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = "temp_hash", // TODO: 实际应该进行密码哈希处理
            DepartmentId = dto.DepartmentId,
            ManagerId = dto.ManagerId,
            IsEnabled = dto.IsEnabled
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task UpdateAsync(UserDto dto)
    {
        var user = await _repository.GetByIdAsync(dto.Id);
        if (user == null)
        {
            throw new KeyNotFoundException($"用户 ID '{dto.Id}' 不存在");
        }

        // 验证用户名唯一性（排除自己）
        var exists = await _repository.FindAsync(u => u.UserName == dto.UserName && u.Id != dto.Id);
        if (exists.Any())
        {
            throw new InvalidOperationException($"用户名 '{dto.UserName}' 已存在");
        }

        user.UserName = dto.UserName;
        user.RealName = dto.RealName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.DepartmentId = dto.DepartmentId;
        user.ManagerId = dto.ManagerId;
        user.IsEnabled = dto.IsEnabled;

        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"用户 ID '{id}' 不存在");
        }

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();
    }
}

