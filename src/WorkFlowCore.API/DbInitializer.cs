using Microsoft.EntityFrameworkCore;
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
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在测试租户
        var testTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var existingTenant = await context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == testTenantId);

        if (existingTenant == null)
        {
            // 插入测试租户
            var testTenant = new Tenant
            {
                Id = testTenantId,
                Name = "测试租户",
                Code = "test-tenant",
                ContactEmail = "test@example.com",
                IsEnabled = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
}

