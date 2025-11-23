using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    /// <summary>
    /// 记录服务操作日志
    /// </summary>
    private void LogServiceOperation(string operation, string message, object? data = null)
    {
        Logger.LogInformation(
            "服务操作 | 服务: {Service} | 操作: {Operation} | 租户: {TenantId} | 消息: {Message} | 数据: {@Data}",
            nameof(AppUserService),
            operation,
            CurrentTenant.Id,
            message,
            data
        );
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
        LogServiceOperation("CreateUser", "开始创建用户", new { input.UserName, input.Email });

        try
        {
            // 检查用户名是否存在
            var existingUser = await _userManager.FindByNameAsync(input.UserName);
            if (existingUser != null)
            {
                Logger.LogWarning("创建用户失败: 用户名 '{UserName}' 已存在", input.UserName);
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
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Logger.LogError("创建用户失败: {Errors} | 用户名: {UserName}", errors, input.UserName);
                throw new Volo.Abp.UserFriendlyException($"创建用户失败: {errors}");
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
                    Logger.LogDebug("为用户 {UserName} 分配角色: {Roles}", input.UserName, string.Join(", ", roleNames));
                }
            }

            LogServiceOperation("CreateUser", "用户创建成功", new { UserId = user.Id, UserName = input.UserName });
            return (await GetByIdAsync(user.Id))!;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "创建用户异常 | 用户名: {UserName}", input.UserName);
            throw;
        }
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
        LogServiceOperation("DeleteUser", $"开始删除用户，数量: {ids.Count}", new { UserIds = ids });

        try
        {
            // 批量查询用户
            var users = await _userManager.Users
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();

            Logger.LogDebug("查询到 {Count} 个用户待删除", users.Count);

            // 批量删除
            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);
                Logger.LogDebug("已删除用户: {UserId} - {UserName}", user.Id, user.UserName);
            }

            LogServiceOperation("DeleteUser", $"删除用户成功，数量: {users.Count}", new { DeletedCount = users.Count });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "删除用户异常 | 用户IDs: {UserIds}", ids);
            throw;
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

