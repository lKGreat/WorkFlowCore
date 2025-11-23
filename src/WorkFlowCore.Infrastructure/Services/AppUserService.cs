using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using WorkFlowCore.Application.DTOs.User;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 应用用户服务实现
/// </summary>
public class AppUserService : ApplicationService, IAppUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRepository<Department, long> _deptRepository;
    private readonly IRepository<IdentityUserRole> _userRoleRepository;

    public AppUserService(
        UserManager<AppUser> userManager,
        IRepository<Department, long> deptRepository,
        IRepository<IdentityUserRole> userRoleRepository)
    {
        _userManager = userManager;
        _deptRepository = deptRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<PagedResponse<UserListDto>> GetPagedAsync(UserPagedRequest request)
    {
        var query = _userManager.Users.AsQueryable();

        // 条件过滤
        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(u => u.UserName!.Contains(request.UserName));
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(request.PhoneNumber));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(u => u.DepartmentId == request.DepartmentId);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(u => u.Status == request.Status);
        }

        if (request.BeginTime.HasValue)
        {
            query = query.Where(u => u.CreationTime >= request.BeginTime);
        }

        if (request.EndTime.HasValue)
        {
            query = query.Where(u => u.CreationTime <= request.EndTime);
        }

        // 分页
        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreationTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // 获取部门信息
        var deptIds = users.Where(u => u.DepartmentId.HasValue).Select(u => u.DepartmentId!.Value).Distinct();
        var depts = await _deptRepository.GetListAsync(d => deptIds.Contains(d.Id));
        var deptDict = depts.ToDictionary(d => d.Id, d => d.DeptName);

        // 映射到DTO
        var items = new List<UserListDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            items.Add(new UserListDto
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName ?? string.Empty,
                NickName = user.NickName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.DepartmentId.HasValue && deptDict.ContainsKey(user.DepartmentId.Value) 
                    ? deptDict[user.DepartmentId.Value] 
                    : null,
                Roles = roles.ToList(),
                Sex = "0",
                Status = user.Status,
                CreationTime = user.CreationTime,
                LastLoginTime = user.LastLoginTime
            });
        }

        return new PagedResponse<UserListDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<UserListDto?> GetByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        string? deptName = null;
        if (user.DepartmentId.HasValue)
        {
            var dept = await _deptRepository.FindAsync(user.DepartmentId.Value);
            deptName = dept?.DeptName;
        }

        return new UserListDto
        {
            UserId = user.Id.ToString(),
            UserName = user.UserName ?? string.Empty,
            NickName = user.NickName ?? string.Empty,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DepartmentId = user.DepartmentId,
            DepartmentName = deptName,
            Roles = roles.ToList(),
            Sex = "0",
            Status = user.Status,
            CreationTime = user.CreationTime,
            LastLoginTime = user.LastLoginTime
        };
    }

    public async Task<UserListDto> CreateAsync(CreateUserInput input)
    {
        // 检查用户名是否存在
        var existingUser = await _userManager.FindByNameAsync(input.UserName);
        if (existingUser != null)
        {
            throw new Volo.Abp.UserFriendlyException($"用户名 '{input.UserName}' 已存在");
        }

        // 创建用户
        var user = new AppUser(Guid.NewGuid(), input.UserName, input.Email, CurrentTenant.Id)
        {
            NickName = input.NickName,
            DepartmentId = input.DepartmentId,
            Status = input.Status
        };

        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException($"创建用户失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // 设置手机号
        if (!string.IsNullOrEmpty(input.PhoneNumber))
        {
            await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);
        }

        // 分配角色 (批量查询优化)
        if (input.RoleIds.Any())
        {
            var roleManager = LazyServiceProvider.LazyGetRequiredService<RoleManager<IdentityRole>>();
            var roleQuery = roleManager.Roles.Where(r => input.RoleIds.Contains(r.Id));
            var roleList = await roleQuery.ToListAsync();
            var roleNames = roleList.Select(r => r.Name!).ToList();
            
            if (roleNames.Any())
            {
                await _userManager.AddToRolesAsync(user, roleNames);
            }
        }

        return (await GetByIdAsync(user.Id))!;
    }

    public async Task UpdateAsync(UpdateUserInput input)
    {
        var user = await _userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null)
        {
            throw new Volo.Abp.UserFriendlyException("用户不存在");
        }

        // 更新基本信息
        user.NickName = input.NickName;
        await _userManager.SetEmailAsync(user, input.Email);
        await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);
        user.DepartmentId = input.DepartmentId;
        user.Status = input.Status;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException($"更新用户失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // 更新角色 (批量查询优化)
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (input.RoleIds.Any())
        {
            var roleManager = LazyServiceProvider.LazyGetRequiredService<RoleManager<IdentityRole>>();
            var roleQuery = roleManager.Roles.Where(r => input.RoleIds.Contains(r.Id));
            var roleList = await roleQuery.ToListAsync();
            var roleNames = roleList.Select(r => r.Name!).ToList();
            
            if (roleNames.Any())
            {
                await _userManager.AddToRolesAsync(user, roleNames);
            }
        }
    }

    public async Task DeleteAsync(List<Guid> ids)
    {
        // 批量查询用户
        var users = await _userManager.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();

        // 批量删除
        foreach (var user in users)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordInput input)
    {
        var user = await _userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null)
        {
            throw new Volo.Abp.UserFriendlyException("用户不存在");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, input.NewPassword);
        
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException($"重置密码失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task ChangeStatusAsync(ChangeStatusInput input)
    {
        var user = await _userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null)
        {
            throw new Volo.Abp.UserFriendlyException("用户不存在");
        }

        user.Status = input.Status;
        await _userManager.UpdateAsync(user);
    }
}

