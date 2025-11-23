# WorkFlowCore ABP 模块架构文档

## 📋 **模块依赖总览**

WorkFlowCore 基于 ABP vNext 框架构建，采用模块化架构。本文档明确各 ABP 模块的使用状态与职责划分。

---

## 🏗️ **模块依赖图**

```
WorkFlowCore.API (主入口)
├─> WorkFlowCore.Application (应用层)
│   ├─> WorkFlowCore.Domain (领域层)
│   └─> ABP Application 模块组
│       ├─> AbpDddApplicationModule
│       ├─> AbpAutoMapperModule
│       ├─> AbpIdentityApplicationModule
│       ├─> AbpPermissionManagementApplicationModule (预留)
│       ├─> AbpSettingManagementApplicationModule
│       └─> AbpAccountApplicationModule
│
├─> WorkFlowCore.Infrastructure (基础设施层)
│   ├─> WorkFlowCore.Domain (领域层)
│   └─> ABP Infrastructure 模块组
│       ├─> AbpEntityFrameworkCoreSqliteModule
│       ├─> AbpBlobStoringModule
│       ├─> AbpBlobStoringFileSystemModule
│       ├─> AbpIdentityEntityFrameworkCoreModule
│       ├─> AbpOpenIddictEntityFrameworkCoreModule (预留 OAuth2)
│       ├─> AbpPermissionManagementEntityFrameworkCoreModule (预留)
│       ├─> AbpSettingManagementEntityFrameworkCoreModule
│       ├─> AbpAuditLoggingEntityFrameworkCoreModule
│       └─> AbpBackgroundJobsEntityFrameworkCoreModule (预留)
│
└─> ABP HTTP API 模块组
    ├─> AbpAspNetCoreMvcModule
    ├─> AbpAutofacModule
    ├─> AbpIdentityHttpApiModule
    ├─> AbpPermissionManagementHttpApiModule (预留)
    ├─> AbpSettingManagementHttpApiModule
    ├─> AbpAccountHttpApiModule
    └─> AbpAccountWebOpenIddictModule (预留 OAuth2)
```

---

## 📦 **ABP 模块使用状态**

### **✅ 当前使用中的模块**

#### 1. **Identity 模块**
**用途**：用户认证与授权基础

**包含组件**：
- `AbpIdentityDomainModule`：IdentityUser、IdentityRole 实体
- `AbpIdentityApplicationModule`：用户/角色管理服务
- `AbpIdentityEntityFrameworkCoreModule`：Identity 表映射
- `AbpIdentityHttpApiModule`：Identity API 端点

**当前集成方式**：
- `AppUser` 继承 `IdentityUser`，扩展字段（NickName、Avatar、LastLoginIp 等）
- 使用 ABP 内置的密码哈希、邮箱验证功能
- 自定义 `AppUserRepository` 用于业务查询

**数据库表**：
```
AbpUsers (主表)
AbpRoles
AbpUserRoles
AbpUserClaims
AbpRoleClaims
AbpUserLogins
AbpUserTokens
AbpOrganizationUnits
AbpUserDelegations
AbpSessions
AbpSecurityLogs
```

---

#### 2. **Setting Management 模块**
**用途**：系统配置管理

**包含组件**：
- `AbpSettingManagementDomainModule`：配置存储
- `AbpSettingManagementApplicationModule`：配置读写服务
- `AbpSettingManagementEntityFrameworkCoreModule`：配置表映射
- `AbpSettingManagementHttpApiModule`：配置 API

**当前使用场景**：
- 租户级配置（邮件服务器、短信接口）
- 全局配置（系统名称、LOGO）

**数据库表**：
```
AbpSettings
```

---

#### 3. **Audit Logging 模块**
**用途**：操作审计日志

**包含组件**：
- `AbpAuditLoggingDomainModule`：审计日志实体
- `AbpAuditLoggingEntityFrameworkCoreModule`：审计表映射

**当前使用场景**：
- 自动记录所有 HTTP 请求（启用后）
- 记录实体变更历史

**数据库表**：
```
AbpAuditLogs
AbpAuditLogActions
AbpEntityChanges
AbpEntityPropertyChanges
```

**注意事项**：
- 当前项目使用自定义 `OperationLog` 表，与 ABP AuditLog 并存
- 未来可考虑统一到 ABP AuditLog

---

#### 4. **Blob Storing 模块**
**用途**：文件存储抽象层

**包含组件**：
- `AbpBlobStoringModule`：存储接口定义
- `AbpBlobStoringFileSystemModule`：本地文件系统实现

**当前使用场景**：
- 文件上传中间存储（分块文件）
- 审批附件存储

**配置**：
```csharp
Configure<AbpBlobStoringOptions>(options =>
{
    options.Containers.Configure<FileStorageBlobContainer>(container =>
    {
        container.UseFileSystem(fileSystem =>
        {
            fileSystem.BasePath = "FileStorage";
        });
    });
});
```

---

### **🔮 预留未来使用的模块**

#### 5. **OpenIddict 模块** ⚠️ 重要
**预留用途**：OAuth 2.0 / OpenID Connect 认证服务器

**包含组件**：
- `AbpOpenIddictDomainModule`：OAuth2 实体
- `AbpOpenIddictEntityFrameworkCoreModule`：OAuth2 表映射
- `AbpAccountWebOpenIddictModule`：OAuth2 授权端点

**未来使用场景**：
- 第三方应用接入（企业内部系统 SSO）
- 移动端 App 授权
- 外部 API 客户端认证

**数据库表**：
```
OpenIddictApplications
OpenIddictAuthorizations
OpenIddictScopes
OpenIddictTokens
```

**当前状态**：
- ✅ 表结构已创建（通过 EF Core 迁移）
- ❌ OAuth2 端点未启用
- ❌ 前端未对接 OAuth2 流程
- ✅ 保留所有模块依赖，随时可激活

**激活步骤（未来）**：
1. 配置 OpenIddict 选项（appsettings.json）
2. 注册 OAuth2 Clients
3. 前端改用 Authorization Code Flow
4. 移除自定义 JWT 认证

---

#### 6. **Permission Management 模块** ⚠️ 重要
**预留用途**：细粒度权限控制

**包含组件**：
- `AbpPermissionManagementDomainModule`：权限授予实体
- `AbpPermissionManagementApplicationModule`：权限管理服务
- `AbpPermissionManagementEntityFrameworkCoreModule`：权限表映射
- `AbpPermissionManagementHttpApiModule`：权限管理 API

**未来使用场景**：
- 按钮级权限控制（查看/编辑/删除/导出）
- 数据权限（仅看本部门/本人数据）
- 动态权限分配（不重启服务）

**数据库表**：
```
AbpPermissionGrants
```

**当前状态**：
- ✅ 表结构已创建
- ❌ 前端未对接权限校验
- ❌ 后端 API 未使用 `[Authorize(Permission)]` 特性
- ✅ 保留模块依赖，当前使用简单 Role-based 授权

**激活步骤（未来）**：
1. 定义权限常量（如 `ProcessDefinitions.Create`）
2. Controller 添加 `[Authorize("ProcessDefinitions.Create")]`
3. 前端根据 `permissions[]` 隐藏按钮
4. 实现数据权限过滤器

---

#### 7. **Background Jobs 模块** ⚠️ 重要
**预留用途**：后台任务调度

**包含组件**：
- `AbpBackgroundJobsDomainModule`：任务实体
- `AbpBackgroundJobsEntityFrameworkCoreModule`：任务表映射

**未来使用场景**：
- 流程超时自动处理
- 定时报表生成
- 批量数据导入/导出
- 邮件异步发送

**数据库表**：
```
AbpBackgroundJobs
```

**当前状态**：
- ✅ 表结构已创建
- ❌ 未使用 `IBackgroundJobManager` 创建任务
- ❌ 未启用后台工作线程
- ✅ 保留模块依赖，可随时接入

**激活步骤（未来）**：
```csharp
// 1. 定义任务
public class SendEmailArgs
{
    public string To { get; set; }
    public string Subject { get; set; }
}

public class SendEmailJob : AsyncBackgroundJob<SendEmailArgs>
{
    public override async Task ExecuteAsync(SendEmailArgs args)
    {
        // 发送邮件逻辑
    }
}

// 2. 入队任务
await _backgroundJobManager.EnqueueAsync(new SendEmailArgs 
{ 
    To = "user@example.com", 
    Subject = "审批通知" 
});
```

---

## 🔄 **双用户体系设计**

### **问题背景**
当前项目同时存在两套用户实体：
1. `AppUser (继承 IdentityUser)`：ABP Identity 用户表（`AbpUsers`）
2. `User`：自定义用户表（`Users`）

### **设计决策**

#### **方案一（当前实施）：双体系并存**

**职责划分**：
- **AppUser**：负责认证授权
  - 登录验证（用户名/密码）
  - 密码哈希存储
  - 角色关联（`AbpUserRoles`）
  - 第三方账号绑定（`UserThirdPartyAccounts`）

- **User**：负责业务流程
  - 流程发起人/审批人关联
  - 部门层级关系
  - 业务字段（工号、职位等）

**同步机制**：
```csharp
// 创建 AppUser 时自动创建 User
public class AppUserCreatedEventHandler : 
    ILocalEventHandler<EntityCreatedEventArgs<AppUser>>,
    ITransientDependency
{
    private readonly IRepository<User, long> _userRepo;
    
    public async Task HandleEventAsync(EntityCreatedEventArgs<AppUser> eventData)
    {
        var appUser = eventData.Entity;
        
        // 同步创建业务 User
        var user = new User(
            SnowflakeIdGenerator.NextId(),
            appUser.TenantId,
            appUser.UserName,
            appUser.Name
        )
        {
            AbpUserId = appUser.Id, // 外键关联
            Email = appUser.Email,
            Phone = appUser.PhoneNumber
        };
        
        await _userRepo.InsertAsync(user);
    }
}
```

**优点**：
- ✅ 保留 ABP Identity 所有功能
- ✅ 业务表结构灵活
- ✅ 未来可无缝接入 OpenIddict

**缺点**：
- ⚠️ 数据冗余（姓名、邮箱等字段重复）
- ⚠️ 需维护同步逻辑

---

#### **方案二（未来可选）：统一为 AppUser**

**改造步骤**：
1. 迁移 `User` 表的业务字段到 `AppUser` 扩展属性
2. 修改所有流程实体外键（`UserId` → `AppUserId`）
3. 删除 `User` 实体与同步逻辑

**优点**：
- ✅ 单一数据源
- ✅ 无同步成本

**缺点**：
- ⚠️ 破坏 ABP Identity 表结构纯净性
- ⚠️ 业务字段与认证字段耦合

---

## 🗂️ **数据库表分类**

### **业务核心表（WorkFlowCore 自定义）**
```
Tenants              # 租户
Users                # 业务用户（与 AppUser 同步）
Departments          # 部门
Roles                # 业务角色（与 AbpRoles 映射）
ProcessDefinitions   # 流程定义
ProcessInstances     # 流程实例
TaskInstances        # 任务实例
FileAttachments      # 文件附件
Menus                # 菜单
DictTypes/DictDatas  # 字典
SystemConfigs        # 系统配置
OperationLogs        # 操作日志
```

### **ABP Identity 表（当前使用）**
```
AbpUsers             # AppUser 主表
AbpRoles
AbpUserRoles
AbpUserClaims
AbpRoleClaims
AbpSessions
AbpSecurityLogs
AbpOrganizationUnits
```

### **ABP 基础设施表（当前使用）**
```
AbpSettings          # 配置管理
AbpAuditLogs         # 审计日志
AbpAuditLogActions
AbpEntityChanges
```

### **ABP 预留表（未来使用）**
```
OpenIddictApplications       # OAuth2 客户端
OpenIddictAuthorizations     # OAuth2 授权记录
OpenIddictScopes             # OAuth2 权限范围
OpenIddictTokens             # OAuth2 令牌
AbpPermissionGrants          # 细粒度权限
AbpBackgroundJobs            # 后台任务
```

---

## 🚀 **服务注册规范**

### **当前问题**
`WorkFlowCoreHttpApiModule` 中手动注册服务：
```csharp
services.AddScoped<IProcessDefinitionService, ProcessDefinitionService>();
services.AddScoped<IMenuService, MenuService>();
// ... 20+ 行手动注册
```

### **ABP 约定自动注册**

#### **方式一：实现 ABP 接口**
```csharp
// Application 层服务自动注册为 Scoped
public class ProcessDefinitionService : ApplicationService, IProcessDefinitionService
{
    // ABP 自动扫描并注册
}

// Transient 服务
public class SmsService : ITransientDependency
{
    // 自动注册为 Transient
}

// Singleton 服务
public class CaptchaService : ISingletonDependency
{
    // 自动注册为 Singleton
}
```

#### **方式二：使用 Dependency 特性**
```csharp
[Dependency(ServiceLifetime.Scoped)]
public class CustomService : ICustomService
{
    // 手动指定生命周期
}
```

### **迁移计划**
1. 删除 `ConfigureCustomServices` 方法内的手动注册
2. 为所有服务实现 `ITransientDependency`/`IScopedDependency`/`ISingletonDependency`
3. 特殊服务（如 JwtService）保留手动注册

---

## ✅ **架构健康度检查清单**

### **模块使用合规性**
- [x] 所有 ABP 模块依赖明确记录
- [x] 预留模块未来用途已文档化
- [ ] 双用户体系同步机制已实现
- [ ] 服务注册改为 ABP 约定自动注册

### **数据库表规范性**
- [x] 业务表与 ABP 表分离清晰
- [x] 预留表结构已通过 EF Core 迁移创建
- [ ] 表命名遵循 ABP 约定（Abp 前缀）

### **认证授权一致性**
- [x] 当前使用自定义 JWT 认证
- [x] OpenIddict 模块已配置但未激活
- [ ] 权限控制改用 ABP PermissionManagement

---

## 📝 **后续优化建议**

### **短期（1-2 周）**
1. **实现 AppUser 与 User 同步机制**
   - 使用 ABP Domain Events
   - 创建/更新/删除时自动同步

2. **规范化服务注册**
   - 删除手动 `services.AddScoped`
   - 改用 ABP 约定接口

### **中期（1-2 月）**
3. **接入 ABP PermissionManagement**
   - 定义权限常量
   - Controller 添加 `[Authorize]` 特性
   - 前端按钮级权限控制

4. **启用 ABP BackgroundJobs**
   - 流程超时自动处理
   - 定时任务调度

### **长期（3-6 月）**
5. **激活 OpenIddict OAuth2**
   - 企业内部 SSO
   - 第三方应用接入
   - 移动端 App 授权

6. **统一审计日志**
   - 迁移自定义 `OperationLog` 到 `AbpAuditLogs`
   - 接入 ELK 日志中心

---

## 🔗 **参考文档**

- [ABP vNext 官方文档](https://docs.abp.io/zh-Hans/abp/latest/)
- [OpenIddict 集成指南](https://docs.abp.io/zh-Hans/abp/latest/Modules/OpenIddict)
- [Permission Management 模块](https://docs.abp.io/zh-Hans/abp/latest/Modules/Permission-Management)
- [Background Jobs 模块](https://docs.abp.io/zh-Hans/abp/latest/Background-Jobs)

---

**文档维护者**：开发团队  
**最后更新**：2025-11-23  
**版本**：v1.0

