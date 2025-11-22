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

public class UserService : ApplicationService, IUserService
{
    private readonly IRepository<User, long> _repository;

    public UserService(IRepository<User, long> repository)
    {
        _repository = repository;
    }

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        var user = await _repository.FindAsync(id);
        return user == null ? null : ObjectMapper.Map<User, UserDto>(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetListAsync();
        return ObjectMapper.Map<List<User>, List<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(UserDto dto)
    {
        if (await _repository.AnyAsync(u => u.UserName == dto.UserName))
        {
             throw new UserFriendlyException($"用户名 '{dto.UserName}' 已存在");
        }

        var user = new User(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, dto.UserName, dto.RealName)
        {
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = "temp_hash", // TODO
            DepartmentId = dto.DepartmentId,
            ManagerId = dto.ManagerId,
            IsEnabled = dto.IsEnabled
        };

        await _repository.InsertAsync(user);
        
        return ObjectMapper.Map<User, UserDto>(user);
    }

    public async Task UpdateAsync(UserDto dto)
    {
        var user = await _repository.GetAsync(dto.Id);
        
        if (await _repository.AnyAsync(u => u.UserName == dto.UserName && u.Id != dto.Id))
        {
            throw new UserFriendlyException($"用户名 '{dto.UserName}' 已存在");
        }

        // UserName setter is public in my updated entity? Yes, I made props public set.
        user.UserName = dto.UserName;
        user.RealName = dto.RealName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.DepartmentId = dto.DepartmentId;
        user.ManagerId = dto.ManagerId;
        user.IsEnabled = dto.IsEnabled;

        await _repository.UpdateAsync(user);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
    }
}
