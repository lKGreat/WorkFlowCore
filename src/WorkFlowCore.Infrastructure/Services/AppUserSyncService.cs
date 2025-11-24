using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// AppUser 与 User 同步服务
/// 提供手动同步方法，由 AppUserService 在 CRUD 操作时调用
/// </summary>
public class AppUserSyncService : ITransientDependency
{
    private readonly IRepository<User, long> _userRepository;

    public AppUserSyncService(IRepository<User, long> userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// 创建 AppUser 后同步创建业务 User
    /// </summary>
    public async Task SyncOnCreateAsync(AppUser appUser)
    {
        // 检查是否已存在对应的 User
        var existingUser = await _userRepository.FindAsync(u => u.AbpUserId == appUser.Id);
        if (existingUser != null)
        {
            return; // 已存在，跳过
        }

        // 创建业务 User
        var user = new User(
            SnowflakeIdGenerator.NextId(),
            appUser.TenantId,
            appUser.UserName,
            appUser.Name ?? appUser.UserName
        )
        {
            AbpUserId = appUser.Id,
            Email = appUser.Email,
            Phone = appUser.PhoneNumber,
            DepartmentId = appUser.DepartmentId,
            ManagerId = appUser.ManagerId,
            IsEnabled = appUser.IsActive,
            PasswordHash = string.Empty // 密码由 ABP Identity 管理
        };

        await _userRepository.InsertAsync(user, autoSave: true);
    }

    /// <summary>
    /// 更新 AppUser 后同步更新业务 User
    /// </summary>
    public async Task SyncOnUpdateAsync(AppUser appUser)
    {
        var user = await _userRepository.FindAsync(u => u.AbpUserId == appUser.Id);
        if (user == null)
        {
            // 未找到对应的 User，尝试创建
            await SyncOnCreateAsync(appUser);
            return;
        }

        // 同步更新字段
        user.UserName = appUser.UserName;
        user.RealName = appUser.Name ?? appUser.UserName;
        user.Email = appUser.Email;
        user.Phone = appUser.PhoneNumber;
        user.DepartmentId = appUser.DepartmentId;
        user.ManagerId = appUser.ManagerId;
        user.IsEnabled = appUser.IsActive;

        await _userRepository.UpdateAsync(user, autoSave: true);
    }

    /// <summary>
    /// 删除 AppUser 后同步软删除业务 User
    /// </summary>
    public async Task SyncOnDeleteAsync(Guid appUserId)
    {
        var user = await _userRepository.FindAsync(u => u.AbpUserId == appUserId);
        if (user == null)
        {
            return; // 未找到，跳过
        }

        // 软删除业务 User（ABP 自动处理 IsDeleted 标记）
        await _userRepository.DeleteAsync(user, autoSave: true);
    }
}



