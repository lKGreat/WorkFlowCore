#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
数据库初始化脚本
初始化基础数据：用户、部门、角色、菜单、字典、系统配置
"""
import sqlite3
import sys
import os
import uuid
from datetime import datetime, timezone

# 设置输出编码
sys.stdout.reconfigure(encoding='utf-8')

# 数据库路径
DB_PATH = r'D:\Code\WorkFlowCore\WorkFlowCore\src\WorkFlowCore.API\workflow_dev.db'

def get_connection():
    """获取数据库连接"""
    return sqlite3.connect(DB_PATH)

def execute_sql(cursor, sql, params=None):
    """执行SQL语句"""
    if params:
        cursor.execute(sql, params)
    else:
        cursor.execute(sql)
    return cursor.fetchone()

def init_tenants(cursor):
    """初始化租户数据"""
    print("\n1. 初始化租户数据...")
    
    # 检查是否已存在
    cursor.execute("SELECT COUNT(*) FROM Tenants WHERE Id = ?", (1,))
    if cursor.fetchone()[0] > 0:
        print("   ✓ 租户数据已存在，跳过")
        return
    
    # 创建测试租户
    now = datetime.now(timezone.utc).isoformat()
    cursor.execute("""
        INSERT INTO Tenants (Id, Name, Code, ContactEmail, IsEnabled, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (1, "测试租户", "test-tenant", "test@example.com", 1, 0, now, now, "{}", str(uuid.uuid4())))
    
    print("   ✓ 测试租户已创建")

def init_abp_roles(cursor):
    """初始化ABP角色"""
    print("\n2. 初始化ABP角色...")
    
    # 检查Admin角色是否存在
    cursor.execute("SELECT Id FROM AbpRoles WHERE Name = ?", ("Admin",))
    result = cursor.fetchone()
    if result:
        role_id = result[0]
        print(f"   ✓ Admin角色已存在 (ID: {role_id})")
        return role_id
    
    # 创建Admin角色
    role_id = str(uuid.uuid4())
    now = datetime.now(timezone.utc).isoformat()
    cursor.execute("""
        INSERT INTO AbpRoles (Id, Name, NormalizedName, IsDefault, IsPublic, IsStatic, ConcurrencyStamp, ExtraProperties, CreationTime)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (role_id, "Admin", "ADMIN", 0, 0, 0, str(uuid.uuid4()), "{}", now))
    
    print(f"   ✓ Admin角色已创建 (ID: {role_id})")
    return role_id

def init_users(cursor, admin_role_id):
    """初始化用户数据"""
    print("\n3. 初始化用户数据...")
    
    # 检查admin用户是否存在
    cursor.execute("SELECT COUNT(*) FROM AbpUsers WHERE UserName = ?", ("admin",))
    if cursor.fetchone()[0] > 0:
        print("   ✓ admin用户已存在，跳过")
        return
    
    # 创建admin用户
    admin_user_id = str(uuid.uuid4())
    now = datetime.now(timezone.utc).isoformat()
    
    # 插入ABP用户（密码: admin123，使用BCrypt哈希）
    password_hash = "$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy"  # admin123的BCrypt哈希
    security_stamp = str(uuid.uuid4())
    concurrency_stamp = str(uuid.uuid4())
    
    cursor.execute("""
        INSERT INTO AbpUsers (Id, TenantId, UserName, NormalizedUserName, Name, Surname, Email, NormalizedEmail, 
                              EmailConfirmed, PasswordHash, SecurityStamp, IsExternal, PhoneNumber, PhoneNumberConfirmed, 
                              IsActive, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount, 
                              ShouldChangePasswordOnNextLogin, EntityVersion, LastPasswordChangeTime, Discriminator,
                              NickName, Avatar, DepartmentId, ManagerId, LoginFailCount, LastLoginTime, LastLoginIp,
                              Status, ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, 
                              LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (admin_user_id, None, "admin", "ADMIN", "系统管理员", None, "admin@workflowcore.com", "ADMIN@WORKFLOWCORE.COM",
          1, password_hash, security_stamp, 0, None, 0, 1, 0, None, 1, 0, 0, 1, None, "AppUser",
          "Admin", None, None, None, 0, None, None, "0", "{}", concurrency_stamp, now, None, None, None, 0, None, None))
    
    # 关联Admin角色
    cursor.execute("""
        INSERT INTO AbpUserRoles (UserId, RoleId, TenantId)
        VALUES (?, ?, ?)
    """, (admin_user_id, admin_role_id, None))
    
    # 创建业务User记录
    cursor.execute("SELECT Id FROM Users WHERE UserName = ?", ("admin",))
    if not cursor.fetchone():
        user_id = 1000000000000000000
        concurrency_stamp_user = str(uuid.uuid4())
        cursor.execute("""
            INSERT INTO Users (Id, AbpUserId, ConcurrencyStamp, CreationTime, CreatorId, DeleterId, DeletionTime,
                              DepartmentId, Email, ExtraProperties, IsDeleted, IsEnabled, LastModificationTime,
                              LastModifierId, ManagerId, PasswordHash, Phone, RealName, TenantId, UserName)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (user_id, admin_user_id, concurrency_stamp_user, now, None, None, None,
              None, "admin@workflowcore.com", "{}", 0, 1, None, None, None, "", None, "系统管理员", None, "admin"))
    
    # 创建test用户
    cursor.execute("SELECT COUNT(*) FROM AbpUsers WHERE UserName = ?", ("test",))
    if cursor.fetchone()[0] == 0:
        test_user_id = str(uuid.uuid4())
        test_password_hash = "$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy"  # test123的BCrypt哈希
        test_security_stamp = str(uuid.uuid4())
        test_concurrency_stamp = str(uuid.uuid4())
        
        cursor.execute("""
            INSERT INTO AbpUsers (Id, TenantId, UserName, NormalizedUserName, Name, Surname, Email, NormalizedEmail, 
                                  EmailConfirmed, PasswordHash, SecurityStamp, IsExternal, PhoneNumber, PhoneNumberConfirmed, 
                                  IsActive, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount, 
                                  ShouldChangePasswordOnNextLogin, EntityVersion, LastPasswordChangeTime, Discriminator,
                                  NickName, Avatar, DepartmentId, ManagerId, LoginFailCount, LastLoginTime, LastLoginIp,
                                  Status, ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, 
                                  LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (test_user_id, None, "test", "TEST", "测试用户", None, "test@workflowcore.com", "TEST@WORKFLOWCORE.COM",
              1, test_password_hash, test_security_stamp, 0, None, 0, 1, 0, None, 1, 0, 0, 1, None, "AppUser",
              "Test", None, None, None, 0, None, None, "0", "{}", test_concurrency_stamp, now, None, None, None, 0, None, None))
        
        # 创建业务User记录
        user_id2 = 1000000000000000001
        concurrency_stamp_user2 = str(uuid.uuid4())
        cursor.execute("""
            INSERT INTO Users (Id, AbpUserId, ConcurrencyStamp, CreationTime, CreatorId, DeleterId, DeletionTime,
                              DepartmentId, Email, ExtraProperties, IsDeleted, IsEnabled, LastModificationTime,
                              LastModifierId, ManagerId, PasswordHash, Phone, RealName, TenantId, UserName)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (user_id2, test_user_id, concurrency_stamp_user2, now, None, None, None,
              None, "test@workflowcore.com", "{}", 0, 1, None, None, None, "", None, "测试用户", None, "test"))
    
    print("   ✓ 用户数据已初始化 (admin, test)")

def init_departments(cursor):
    """初始化部门数据"""
    print("\n4. 初始化部门数据...")
    
    cursor.execute("SELECT COUNT(*) FROM Departments")
    if cursor.fetchone()[0] > 0:
        print("   ✓ 部门数据已存在，跳过")
        return
    
    now = datetime.now(timezone.utc).isoformat()
    
    # 创建根部门（总公司）
    root_id = 2000000000000000000
    concurrency_stamp_dept = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO Departments (Id, DeptName, Code, ParentId, Ancestors, OrderNum, Status, 
                                 TenantId, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (root_id, "总公司", "ROOT", None, "0", 1, "0", None, 0, now, now, "{}", concurrency_stamp_dept))
    
    # 创建子部门
    depts = [
        (2000000000000000001, "技术部", "TECH", root_id, f"0,{root_id}", 1),
        (2000000000000000002, "市场部", "MARKET", root_id, f"0,{root_id}", 2),
        (2000000000000000003, "人事部", "HR", root_id, f"0,{root_id}", 3),
    ]
    
    for dept_id, name, code, parent_id, ancestors, order_num in depts:
        dept_concurrency_stamp = str(uuid.uuid4())
        cursor.execute("""
            INSERT INTO Departments (Id, DeptName, Code, ParentId, Ancestors, OrderNum, Status, 
                                     TenantId, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (dept_id, name, code, parent_id, ancestors, order_num, "0", None, 0, now, now, "{}", dept_concurrency_stamp))
    
    print("   ✓ 部门数据已初始化 (总公司、技术部、市场部、人事部)")

def init_business_roles(cursor):
    """初始化业务角色"""
    print("\n5. 初始化业务角色...")
    
    cursor.execute("SELECT COUNT(*) FROM Roles")
    if cursor.fetchone()[0] > 0:
        print("   ✓ 业务角色数据已存在，跳过")
        return
    
    now = datetime.now(timezone.utc).isoformat()
    
    roles = [
        (3000000000000000000, "普通用户", "USER", "普通用户角色"),
        (3000000000000000001, "部门经理", "MANAGER", "部门经理角色"),
    ]
    
    for role_id, name, code, desc in roles:
        role_concurrency_stamp = str(uuid.uuid4())
        cursor.execute("""
            INSERT INTO Roles (Id, Name, Code, Description, TenantId, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (role_id, name, code, desc, None, 0, now, now, "{}", role_concurrency_stamp))
    
    print("   ✓ 业务角色数据已初始化 (普通用户、部门经理)")

def init_menus(cursor, admin_role_id):
    """初始化菜单数据"""
    print("\n6. 初始化菜单数据...")
    
    cursor.execute("SELECT COUNT(*) FROM Menus")
    if cursor.fetchone()[0] > 0:
        print("   ✓ 菜单数据已存在，跳过")
        return
    
    now = datetime.now(timezone.utc).isoformat()
    
    # 创建菜单（目录和菜单项）
    menus = [
        # 系统管理目录
        (4000000000000000000, "系统管理", "M", None, "/system", None, None, "setting", 1, 1, "0"),
        (4000000000000000001, "用户管理", "C", 4000000000000000000, "/system/users", "system/user/index", "system:user:list", "user", 1, 1, "0"),
        (4000000000000000002, "角色管理", "C", 4000000000000000000, "/system/roles", "system/role/index", "system:role:list", "peoples", 2, 1, "0"),
        (4000000000000000003, "部门管理", "C", 4000000000000000000, "/system/departments", "system/department/index", "system:dept:list", "tree", 3, 1, "0"),
        (4000000000000000004, "菜单管理", "C", 4000000000000000000, "/system/menus", "system/menu/index", "system:menu:list", "tree-table", 4, 1, "0"),
        # 工作流目录
        (4000000000000000005, "工作流", "M", None, "/workflow", None, None, "guide", 2, 1, "0"),
        (4000000000000000006, "流程定义", "C", 4000000000000000005, "/workflow/definitions", "workflow/definition/index", "workflow:definition:list", "documentation", 1, 1, "0"),
        (4000000000000000007, "流程实例", "C", 4000000000000000005, "/workflow/instances", "workflow/instance/index", "workflow:instance:list", "list", 2, 1, "0"),
        # 系统设置目录
        (4000000000000000008, "系统设置", "M", None, "/settings", None, None, "tool", 3, 1, "0"),
        (4000000000000000009, "字典管理", "C", 4000000000000000008, "/settings/dict", "settings/dict/index", "settings:dict:list", "dict", 1, 1, "0"),
        (4000000000000000010, "系统配置", "C", 4000000000000000008, "/settings/config", "settings/config/index", "settings:config:list", "edit", 2, 1, "0"),
    ]
    
    for menu_id, name, menu_type, parent_id, path, component, permission, icon, order_num, visible, status in menus:
        menu_concurrency_stamp = str(uuid.uuid4())
        cursor.execute("""
            INSERT INTO Menus (Id, MenuName, MenuType, ParentId, Path, Component, PermissionCode, 
                              Icon, OrderNum, Visible, IsFrame, Status, TenantId, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (menu_id, name, menu_type, parent_id, path, component, permission, icon, order_num, visible, 0, status, None, 0, now, now, "{}", menu_concurrency_stamp))
    
    # 为Admin角色分配所有菜单权限
    menu_ids = [m[0] for m in menus]
    for idx, menu_id in enumerate(menu_ids):
        role_menu_id = 5000000000000000000 + idx
        cursor.execute("""
            INSERT INTO RoleMenus (Id, RoleId, MenuId)
            VALUES (?, ?, ?)
        """, (role_menu_id, admin_role_id, menu_id))
    
    print("   ✓ 菜单数据已初始化 (11个菜单项)")

def init_dicts(cursor):
    """初始化字典数据"""
    print("\n7. 初始化字典数据...")
    
    cursor.execute("SELECT COUNT(*) FROM DictTypes")
    if cursor.fetchone()[0] > 0:
        print("   ✓ 字典数据已存在，跳过")
        return
    
    now = datetime.now(timezone.utc).isoformat()
    
    # 用户状态字典
    user_status_type_id = 6000000000000000000
    dict_type_stamp1 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictTypes (Id, DictName, DictTypeCode, Status, Remark, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (user_status_type_id, "用户状态", "user_status", "0", "用户状态字典", 0, now, now, "{}", dict_type_stamp1))
    
    dict_data_stamp1 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictDatas (Id, DictTypeId, DictLabel, DictValue, DictSort, Status, IsDefault, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (6000000000000000001, user_status_type_id, "正常", "0", 1, "0", 1, 0, now, now, "{}", dict_data_stamp1))
    
    dict_data_stamp2 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictDatas (Id, DictTypeId, DictLabel, DictValue, DictSort, Status, IsDefault, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (6000000000000000002, user_status_type_id, "停用", "1", 2, "0", 0, 0, now, now, "{}", dict_data_stamp2))
    
    # 性别字典
    gender_type_id = 6000000000000000003
    dict_type_stamp2 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictTypes (Id, DictName, DictTypeCode, Status, Remark, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (gender_type_id, "性别", "gender", "0", "性别字典", 0, now, now, "{}", dict_type_stamp2))
    
    dict_data_stamp3 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictDatas (Id, DictTypeId, DictLabel, DictValue, DictSort, Status, IsDefault, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (6000000000000000004, gender_type_id, "男", "M", 1, "0", 1, 0, now, now, "{}", dict_data_stamp3))
    
    dict_data_stamp4 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictDatas (Id, DictTypeId, DictLabel, DictValue, DictSort, Status, IsDefault, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (6000000000000000005, gender_type_id, "女", "F", 2, "0", 0, 0, now, now, "{}", dict_data_stamp4))
    
    # 是否字典
    yes_no_type_id = 6000000000000000006
    dict_type_stamp3 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictTypes (Id, DictName, DictTypeCode, Status, Remark, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (yes_no_type_id, "是否", "yes_no", "0", "是否字典", 0, now, now, "{}", dict_type_stamp3))
    
    dict_data_stamp5 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictDatas (Id, DictTypeId, DictLabel, DictValue, DictSort, Status, IsDefault, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (6000000000000000007, yes_no_type_id, "是", "Y", 1, "0", 1, 0, now, now, "{}", dict_data_stamp5))
    
    dict_data_stamp6 = str(uuid.uuid4())
    cursor.execute("""
        INSERT INTO DictDatas (Id, DictTypeId, DictLabel, DictValue, DictSort, Status, IsDefault, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (6000000000000000008, yes_no_type_id, "否", "N", 2, "0", 0, 0, now, now, "{}", dict_data_stamp6))
    
    print("   ✓ 字典数据已初始化 (3个字典类型，6个字典项)")

def init_system_configs(cursor):
    """初始化系统配置"""
    print("\n8. 初始化系统配置...")
    
    cursor.execute("SELECT COUNT(*) FROM SystemConfigs")
    if cursor.fetchone()[0] > 0:
        print("   ✓ 系统配置已存在，跳过")
        return
    
    now = datetime.now(timezone.utc).isoformat()
    
    configs = [
        (7000000000000000000, "system.name", "WorkFlowCore", "系统名称", "Y", "系统名称配置"),
        (7000000000000000001, "system.logo", "/logo.png", "系统Logo", "Y", "系统Logo路径"),
        (7000000000000000002, "system.version", "1.0.0", "系统版本", "Y", "系统版本号"),
    ]
    
    for config_id, key, value, name, config_type, remark in configs:
        config_stamp = str(uuid.uuid4())
        cursor.execute("""
            INSERT INTO SystemConfigs (Id, ConfigKey, ConfigValue, ConfigName, ConfigType, Remark, IsDeleted, CreationTime, LastModificationTime, ExtraProperties, ConcurrencyStamp)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (config_id, key, value, name, config_type, remark, 0, now, now, "{}", config_stamp))
    
    print("   ✓ 系统配置已初始化 (3个配置项)")

def vacuum_database(cursor):
    """优化数据库"""
    print("\n9. 优化数据库...")
    cursor.execute("VACUUM")
    print("   ✓ 数据库优化完成")

def main():
    """主函数"""
    print("=" * 60)
    print("数据库初始化脚本")
    print("=" * 60)
    
    if not os.path.exists(DB_PATH):
        print(f"错误: 数据库文件不存在: {DB_PATH}")
        return
    
    conn = get_connection()
    cursor = conn.cursor()
    
    try:
        # 开始事务
        cursor.execute("BEGIN TRANSACTION")
        
        # 初始化各项数据
        init_tenants(cursor)
        admin_role_id = init_abp_roles(cursor)
        init_users(cursor, admin_role_id)
        init_departments(cursor)
        init_business_roles(cursor)
        init_menus(cursor, admin_role_id)
        init_dicts(cursor)
        init_system_configs(cursor)
        
        # 提交事务
        cursor.execute("COMMIT")
        
        # 优化数据库
        vacuum_database(cursor)
        
        print("\n" + "=" * 60)
        print("数据库初始化完成！")
        print("=" * 60)
        print("\n默认账号信息:")
        print("  - 用户名: admin")
        print("  - 密码: admin123")
        print("  - 用户名: test")
        print("  - 密码: test123")
        
    except Exception as e:
        cursor.execute("ROLLBACK")
        print(f"\n错误: {e}")
        import traceback
        traceback.print_exc()
    finally:
        conn.close()

if __name__ == "__main__":
    main()

