using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;
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
                // 设置用户为激活状态
                await userManager.SetLockoutEnabledAsync(adminUser, true);
                (adminUser as Volo.Abp.Identity.IdentityUser).SetEmailConfirmed(true);
                (adminUser as Volo.Abp.Identity.IdentityUser).SetIsActive(true);
                await userManager.UpdateAsync(adminUser);
                
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
                // 设置用户为激活状态
                await userManager.SetLockoutEnabledAsync(testUser, true);
                (testUser as Volo.Abp.Identity.IdentityUser).SetEmailConfirmed(true);
                (testUser as Volo.Abp.Identity.IdentityUser).SetIsActive(true);
                await userManager.UpdateAsync(testUser);
                
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

    /// <summary>
    /// 初始化部门数据
    /// </summary>
    public static async Task SeedDepartmentsAsync(WorkFlowDbContext context)
    {
        // 检查是否已存在部门数据
        var hasDepartments = await context.Departments.IgnoreQueryFilters().AnyAsync();
        if (hasDepartments)
        {
            Console.WriteLine("✓ 部门数据已存在，跳过初始化");
            return;
        }

        var tenantId = (Guid?)null; // 主租户

        // 创建根部门（总公司）
        var rootDeptId = SnowflakeIdGenerator.NextId();
        var rootDept = new Department(rootDeptId, tenantId, "总公司")
        {
            Code = "ROOT",
            ParentId = null,
            Ancestors = "0",
            OrderNum = 1,
            Status = "0"
        };
        context.Departments.Add(rootDept);

        // 创建技术部
        var techDeptId = SnowflakeIdGenerator.NextId();
        var techDept = new Department(techDeptId, tenantId, "技术部")
        {
            Code = "TECH",
            ParentId = rootDeptId,
            Ancestors = $"0,{rootDeptId}",
            OrderNum = 1,
            Leader = "技术总监",
            Status = "0"
        };
        context.Departments.Add(techDept);

        // 创建市场部
        var marketDeptId = SnowflakeIdGenerator.NextId();
        var marketDept = new Department(marketDeptId, tenantId, "市场部")
        {
            Code = "MARKET",
            ParentId = rootDeptId,
            Ancestors = $"0,{rootDeptId}",
            OrderNum = 2,
            Leader = "市场总监",
            Status = "0"
        };
        context.Departments.Add(marketDept);

        // 创建人事部
        var hrDeptId = SnowflakeIdGenerator.NextId();
        var hrDept = new Department(hrDeptId, tenantId, "人事部")
        {
            Code = "HR",
            ParentId = rootDeptId,
            Ancestors = $"0,{rootDeptId}",
            OrderNum = 3,
            Leader = "人事总监",
            Status = "0"
        };
        context.Departments.Add(hrDept);

        await context.SaveChangesAsync();
        Console.WriteLine("✓ 部门数据已初始化");
        Console.WriteLine($"  - 总公司");
        Console.WriteLine($"  - 技术部");
        Console.WriteLine($"  - 市场部");
        Console.WriteLine($"  - 人事部");
    }

    /// <summary>
    /// 初始化业务角色
    /// </summary>
    public static async Task SeedRolesAsync(WorkFlowDbContext context)
    {
        var tenantId = (Guid?)null; // 主租户

        // 检查是否已存在业务角色
        var hasRoles = await context.WorkFlowRoles.IgnoreQueryFilters().AnyAsync();
        if (hasRoles)
        {
            Console.WriteLine("✓ 业务角色数据已存在，跳过初始化");
            return;
        }

        // 创建普通用户角色
        var userRoleId = SnowflakeIdGenerator.NextId();
        var userRole = new Role(userRoleId, tenantId, "普通用户", "USER")
        {
            Description = "普通用户角色"
        };
        context.WorkFlowRoles.Add(userRole);

        // 创建部门经理角色
        var managerRoleId = SnowflakeIdGenerator.NextId();
        var managerRole = new Role(managerRoleId, tenantId, "部门经理", "MANAGER")
        {
            Description = "部门经理角色"
        };
        context.WorkFlowRoles.Add(managerRole);

        await context.SaveChangesAsync();
        Console.WriteLine("✓ 业务角色数据已初始化");
        Console.WriteLine($"  - 普通用户 (USER)");
        Console.WriteLine($"  - 部门经理 (MANAGER)");
    }

    /// <summary>
    /// 初始化菜单数据
    /// </summary>
    public static async Task SeedMenusAsync(WorkFlowDbContext context, IServiceProvider serviceProvider)
    {
        // 检查是否已存在菜单数据
        var hasMenus = await context.Menus.IgnoreQueryFilters().AnyAsync();
        if (hasMenus)
        {
            Console.WriteLine("✓ 菜单数据已存在，跳过初始化");
            return;
        }

        var tenantId = (Guid?)null; // 主租户
        var roleManager = serviceProvider.GetRequiredService<IdentityRoleManager>();

        // 获取 Admin 角色
        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole == null)
        {
            Console.WriteLine("✗ Admin 角色不存在，无法初始化菜单权限");
            return;
        }

        // 1. 系统管理目录
        var systemMenuId = SnowflakeIdGenerator.NextId();
        var systemMenu = new Menu(systemMenuId, "系统管理", "M")
        {
            TenantId = tenantId,
            ParentId = null,
            Path = "/system",
            Icon = "setting",
            OrderNum = 1,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(systemMenu);

        // 1.1 用户管理
        var userMenuId = SnowflakeIdGenerator.NextId();
        var userMenu = new Menu(userMenuId, "用户管理", "C")
        {
            TenantId = tenantId,
            ParentId = systemMenuId,
            Path = "/system/users",
            Component = "system/user/index",
            PermissionCode = "system:user:list",
            Icon = "user",
            OrderNum = 1,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(userMenu);

        // 1.2 角色管理
        var roleMenuId = SnowflakeIdGenerator.NextId();
        var roleMenu = new Menu(roleMenuId, "角色管理", "C")
        {
            TenantId = tenantId,
            ParentId = systemMenuId,
            Path = "/system/roles",
            Component = "system/role/index",
            PermissionCode = "system:role:list",
            Icon = "peoples",
            OrderNum = 2,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(roleMenu);

        // 1.3 部门管理
        var deptMenuId = SnowflakeIdGenerator.NextId();
        var deptMenu = new Menu(deptMenuId, "部门管理", "C")
        {
            TenantId = tenantId,
            ParentId = systemMenuId,
            Path = "/system/departments",
            Component = "system/department/index",
            PermissionCode = "system:dept:list",
            Icon = "tree",
            OrderNum = 3,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(deptMenu);

        // 1.4 菜单管理
        var menuManageId = SnowflakeIdGenerator.NextId();
        var menuManage = new Menu(menuManageId, "菜单管理", "C")
        {
            TenantId = tenantId,
            ParentId = systemMenuId,
            Path = "/system/menus",
            Component = "system/menu/index",
            PermissionCode = "system:menu:list",
            Icon = "tree-table",
            OrderNum = 4,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(menuManage);

        // 2. 工作流目录
        var workflowMenuId = SnowflakeIdGenerator.NextId();
        var workflowMenu = new Menu(workflowMenuId, "工作流", "M")
        {
            TenantId = tenantId,
            ParentId = null,
            Path = "/workflow",
            Icon = "guide",
            OrderNum = 2,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(workflowMenu);

        // 2.1 流程定义
        var processDefMenuId = SnowflakeIdGenerator.NextId();
        var processDefMenu = new Menu(processDefMenuId, "流程定义", "C")
        {
            TenantId = tenantId,
            ParentId = workflowMenuId,
            Path = "/workflow/definitions",
            Component = "workflow/definition/index",
            PermissionCode = "workflow:definition:list",
            Icon = "documentation",
            OrderNum = 1,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(processDefMenu);

        // 2.2 流程实例
        var processInstMenuId = SnowflakeIdGenerator.NextId();
        var processInstMenu = new Menu(processInstMenuId, "流程实例", "C")
        {
            TenantId = tenantId,
            ParentId = workflowMenuId,
            Path = "/workflow/instances",
            Component = "workflow/instance/index",
            PermissionCode = "workflow:instance:list",
            Icon = "list",
            OrderNum = 2,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(processInstMenu);

        // 3. 系统设置目录
        var settingsMenuId = SnowflakeIdGenerator.NextId();
        var settingsMenu = new Menu(settingsMenuId, "系统设置", "M")
        {
            TenantId = tenantId,
            ParentId = null,
            Path = "/settings",
            Icon = "tool",
            OrderNum = 3,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(settingsMenu);

        // 3.1 字典管理
        var dictMenuId = SnowflakeIdGenerator.NextId();
        var dictMenu = new Menu(dictMenuId, "字典管理", "C")
        {
            TenantId = tenantId,
            ParentId = settingsMenuId,
            Path = "/settings/dict",
            Component = "settings/dict/index",
            PermissionCode = "settings:dict:list",
            Icon = "dict",
            OrderNum = 1,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(dictMenu);

        // 3.2 系统配置
        var configMenuId = SnowflakeIdGenerator.NextId();
        var configMenu = new Menu(configMenuId, "系统配置", "C")
        {
            TenantId = tenantId,
            ParentId = settingsMenuId,
            Path = "/settings/config",
            Component = "settings/config/index",
            PermissionCode = "settings:config:list",
            Icon = "edit",
            OrderNum = 2,
            Visible = true,
            Status = "0"
        };
        context.Menus.Add(configMenu);

        await context.SaveChangesAsync();

        // 为 Admin 角色分配所有菜单权限
        var allMenus = new[] { systemMenuId, userMenuId, roleMenuId, deptMenuId, menuManageId, 
                               workflowMenuId, processDefMenuId, processInstMenuId, 
                               settingsMenuId, dictMenuId, configMenuId };
        
        foreach (var menuId in allMenus)
        {
            var roleMenuRelation = new RoleMenu(SnowflakeIdGenerator.NextId(), adminRole.Id, menuId);
            context.RoleMenus.Add(roleMenuRelation);
        }

        await context.SaveChangesAsync();
        Console.WriteLine("✓ 菜单数据已初始化");
        Console.WriteLine($"  - 系统管理（4个子菜单）");
        Console.WriteLine($"  - 工作流（2个子菜单）");
        Console.WriteLine($"  - 系统设置（2个子菜单）");
    }

    /// <summary>
    /// 初始化字典类型和数据
    /// </summary>
    public static async Task SeedDictTypesAsync(WorkFlowDbContext context)
    {
        // 检查是否已存在字典数据
        var hasDictTypes = await context.DictTypes.IgnoreQueryFilters().AnyAsync();
        if (hasDictTypes)
        {
            Console.WriteLine("✓ 字典数据已存在，跳过初始化");
            return;
        }

        // 1. 用户状态字典
        var userStatusTypeId = SnowflakeIdGenerator.NextId();
        var userStatusType = new DictType(userStatusTypeId, "用户状态", "user_status")
        {
            Status = "0",
            Remark = "用户状态字典"
        };
        context.DictTypes.Add(userStatusType);

        // 用户状态字典数据
        var userStatusNormalId = SnowflakeIdGenerator.NextId();
        var userStatusNormal = new DictData(userStatusNormalId, userStatusTypeId, "正常", "0")
        {
            DictSort = 1,
            Status = "0",
            IsDefault = true
        };
        context.DictDatas.Add(userStatusNormal);

        var userStatusDisabledId = SnowflakeIdGenerator.NextId();
        var userStatusDisabled = new DictData(userStatusDisabledId, userStatusTypeId, "停用", "1")
        {
            DictSort = 2,
            Status = "0"
        };
        context.DictDatas.Add(userStatusDisabled);

        // 2. 性别字典
        var genderTypeId = SnowflakeIdGenerator.NextId();
        var genderType = new DictType(genderTypeId, "性别", "gender")
        {
            Status = "0",
            Remark = "性别字典"
        };
        context.DictTypes.Add(genderType);

        // 性别字典数据
        var genderMaleId = SnowflakeIdGenerator.NextId();
        var genderMale = new DictData(genderMaleId, genderTypeId, "男", "M")
        {
            DictSort = 1,
            Status = "0",
            IsDefault = true
        };
        context.DictDatas.Add(genderMale);

        var genderFemaleId = SnowflakeIdGenerator.NextId();
        var genderFemale = new DictData(genderFemaleId, genderTypeId, "女", "F")
        {
            DictSort = 2,
            Status = "0"
        };
        context.DictDatas.Add(genderFemale);

        // 3. 是否字典
        var yesNoTypeId = SnowflakeIdGenerator.NextId();
        var yesNoType = new DictType(yesNoTypeId, "是否", "yes_no")
        {
            Status = "0",
            Remark = "是否字典"
        };
        context.DictTypes.Add(yesNoType);

        // 是否字典数据
        var yesId = SnowflakeIdGenerator.NextId();
        var yes = new DictData(yesId, yesNoTypeId, "是", "Y")
        {
            DictSort = 1,
            Status = "0",
            IsDefault = true
        };
        context.DictDatas.Add(yes);

        var noId = SnowflakeIdGenerator.NextId();
        var no = new DictData(noId, yesNoTypeId, "否", "N")
        {
            DictSort = 2,
            Status = "0"
        };
        context.DictDatas.Add(no);

        await context.SaveChangesAsync();
        Console.WriteLine("✓ 字典数据已初始化");
        Console.WriteLine($"  - 用户状态 (user_status): 正常、停用");
        Console.WriteLine($"  - 性别 (gender): 男、女");
        Console.WriteLine($"  - 是否 (yes_no): 是、否");
    }

    /// <summary>
    /// 初始化系统配置
    /// </summary>
    public static async Task SeedSystemConfigsAsync(WorkFlowDbContext context)
    {
        // 检查是否已存在系统配置
        var hasConfigs = await context.SystemConfigs.IgnoreQueryFilters().AnyAsync();
        if (hasConfigs)
        {
            Console.WriteLine("✓ 系统配置已存在，跳过初始化");
            return;
        }

        // 系统名称
        var systemNameId = SnowflakeIdGenerator.NextId();
        var systemName = new SystemConfig(systemNameId, "system.name", "WorkFlowCore", "系统名称")
        {
            ConfigType = "Y",
            Remark = "系统名称配置"
        };
        context.SystemConfigs.Add(systemName);

        // 系统Logo
        var systemLogoId = SnowflakeIdGenerator.NextId();
        var systemLogo = new SystemConfig(systemLogoId, "system.logo", "/logo.png", "系统Logo")
        {
            ConfigType = "Y",
            Remark = "系统Logo路径"
        };
        context.SystemConfigs.Add(systemLogo);

        // 系统版本
        var systemVersionId = SnowflakeIdGenerator.NextId();
        var systemVersion = new SystemConfig(systemVersionId, "system.version", "1.0.0", "系统版本")
        {
            ConfigType = "Y",
            Remark = "系统版本号"
        };
        context.SystemConfigs.Add(systemVersion);

        await context.SaveChangesAsync();
        Console.WriteLine("✓ 系统配置已初始化");
        Console.WriteLine($"  - system.name: WorkFlowCore");
        Console.WriteLine($"  - system.logo: /logo.png");
        Console.WriteLine($"  - system.version: 1.0.0");
    }
}

