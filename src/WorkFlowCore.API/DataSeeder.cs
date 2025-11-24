using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using WorkFlowCore.Domain.Identity;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Services;

namespace WorkFlowCore.API;

/// <summary>
/// 数据种子初始化服务
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// 初始化默认管理员账号
    /// </summary>
    public static async Task SeedAdminUserAsync(
        IServiceProvider serviceProvider, 
        WorkFlowDbContext context)
    {
        var userManager = serviceProvider.GetRequiredService<IdentityUserManager>();
        var roleManager = serviceProvider.GetRequiredService<IdentityRoleManager>();
        var appUserSyncService = serviceProvider.GetRequiredService<AppUserSyncService>();

        // 1. 创建管理员角色
        var adminRoleName = "Admin";
        var adminRole = await roleManager.FindByNameAsync(adminRoleName);
        if (adminRole == null)
        {
            adminRole = new IdentityRole(Guid.NewGuid(), adminRoleName, tenantId: null);
            await roleManager.CreateAsync(adminRole);
            Console.WriteLine($"✓ 管理员角色已创建: {adminRoleName}");
        }

        // 2. 创建默认管理员用户
        var adminUserName = "admin";
        var existingAdmin = await context.Set<AppUser>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserName == adminUserName);

        if (existingAdmin == null)
        {
            var adminUser = new AppUser(
                Guid.NewGuid(),
                adminUserName,
                email: "admin@workflowcore.com",
                tenantId: null // 主租户
            )
            {
                Name = "系统管理员",
                NickName = "Admin",
                Status = "0"
            };

            // 使用 ABP Identity 创建用户（自动哈希密码）
            var result = await userManager.CreateAsync(adminUser, "admin123");
            
            if (result.Succeeded)
            {
                // 设置邮箱已确认（通过 EF Core Entry API）
                context.Entry(adminUser).Property(nameof(adminUser.EmailConfirmed)).CurrentValue = true;
                await context.SaveChangesAsync();
                
                // 分配管理员角色
                await userManager.AddToRoleAsync(adminUser, adminRoleName);

                // 同步到业务 User 表
                await appUserSyncService.SyncOnCreateAsync(adminUser);

                Console.WriteLine("✓ 默认管理员账号已创建");
                Console.WriteLine($"  - 用户名: {adminUser.UserName}");
                Console.WriteLine($"  - 密码: admin123");
                Console.WriteLine($"  - 邮箱: {adminUser.Email}");
                Console.WriteLine($"  - 角色: {adminRoleName}");
            }
            else
            {
                Console.WriteLine("✗ 管理员账号创建失败:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("✓ 管理员账号已存在");
            Console.WriteLine($"  - 用户名: {existingAdmin.UserName}");
            Console.WriteLine($"  - 邮箱: {existingAdmin.Email}");
        }

        // 3. 创建测试普通用户（可选）
        var testUserName = "test";
        var existingTestUser = await context.Set<AppUser>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserName == testUserName);

        if (existingTestUser == null)
        {
            var testUser = new AppUser(
                Guid.NewGuid(),
                testUserName,
                email: "test@workflowcore.com",
                tenantId: null
            )
            {
                Name = "测试用户",
                NickName = "Test",
                Status = "0"
            };

            var result = await userManager.CreateAsync(testUser, "test123");
            
            if (result.Succeeded)
            {
                // 设置邮箱已确认（通过 EF Core Entry API）
                context.Entry(testUser).Property(nameof(testUser.EmailConfirmed)).CurrentValue = true;
                await context.SaveChangesAsync();
                
                // 同步到业务 User 表
                await appUserSyncService.SyncOnCreateAsync(testUser);

                Console.WriteLine("✓ 测试用户账号已创建");
                Console.WriteLine($"  - 用户名: {testUser.UserName}");
                Console.WriteLine($"  - 密码: test123");
                Console.WriteLine($"  - 邮箱: {testUser.Email}");
            }
        }
        else
        {
            Console.WriteLine("✓ 测试用户账号已存在");
        }
    }
}

