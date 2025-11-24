using Microsoft.EntityFrameworkCore;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Data;

namespace WorkFlowCore.API;

/// <summary>
/// 数据库初始化器
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// 初始化测试数据
    /// </summary>
    public static async Task InitializeAsync(WorkFlowDbContext context)
    {
        // 检查是否已存在测试租户
        var testTenantId = 1L;
        var existingTenant = await context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == testTenantId);

        if (existingTenant == null)
        {
            // 插入测试租户（使用构造函数，ABP自动处理审计字段）
            var testTenant = new Tenant(testTenantId, "测试租户", "test-tenant")
            {
                ContactEmail = "test@example.com",
                IsEnabled = true
            };

            context.Tenants.Add(testTenant);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ 测试租户已成功创建");
            Console.WriteLine($"  - 租户ID: {testTenant.Id}");
            Console.WriteLine($"  - 租户名称: {testTenant.Name}");
            Console.WriteLine($"  - 租户代码: {testTenant.Code}");
        }
        else
        {
            Console.WriteLine("✓ 测试租户已存在");
            Console.WriteLine($"  - 租户ID: {existingTenant.Id}");
            Console.WriteLine($"  - 租户名称: {existingTenant.Name}");
        }
    }

    /// <summary>
    /// 初始化所有基础数据
    /// </summary>
    public static async Task InitializeAllAsync(WorkFlowDbContext context, IServiceProvider serviceProvider)
    {
        Console.WriteLine("\n开始初始化基础数据...");
        Console.WriteLine(new string('=', 60));

        // 1. 初始化租户
        await InitializeAsync(context);

        // 2. 初始化部门
        await DataSeeder.SeedDepartmentsAsync(context);

        // 3. 初始化业务角色
        await DataSeeder.SeedRolesAsync(context);

        // 4. 初始化用户（包含ABP角色创建）
        await DataSeeder.SeedAdminUserAsync(serviceProvider, context);

        // 5. 初始化菜单
        await DataSeeder.SeedMenusAsync(context, serviceProvider);

        // 6. 初始化字典
        await DataSeeder.SeedDictTypesAsync(context);

        // 7. 初始化系统配置
        await DataSeeder.SeedSystemConfigsAsync(context);

        Console.WriteLine(new string('=', 60));
        Console.WriteLine("基础数据初始化完成！\n");
    }
}

