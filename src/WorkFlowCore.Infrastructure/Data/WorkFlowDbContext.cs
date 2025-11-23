using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.Infrastructure.Data;

[ConnectionStringName("Default")]
public class WorkFlowDbContext : AbpDbContext<WorkFlowDbContext>, IIdentityDbContext
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> WorkFlowUsers { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> WorkFlowRoles { get; set; }
    public DbSet<ProcessDefinition> ProcessDefinitions { get; set; }
    public DbSet<ProcessInstance> ProcessInstances { get; set; }
    public DbSet<TaskInstance> TaskInstances { get; set; }
    public DbSet<FileStorageProvider> FileStorageProviders { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }
    public DbSet<FileChunk> FileChunks { get; set; }
    public DbSet<UserThirdPartyAccount> UserThirdPartyAccounts { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }
    public DbSet<DictType> DictTypes { get; set; }
    public DbSet<DictData> DictDatas { get; set; }
    public DbSet<SystemConfig> SystemConfigs { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }
    
    // ABP Identity tables (required by IIdentityDbContext)
    public DbSet<IdentityUser> Users => Set<IdentityUser>();
    public DbSet<IdentityRole> Roles => Set<IdentityRole>();
    public DbSet<IdentityClaimType> ClaimTypes => Set<IdentityClaimType>();
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<IdentitySecurityLog> SecurityLogs => Set<IdentitySecurityLog>();
    public DbSet<IdentityLinkUser> LinkUsers => Set<IdentityLinkUser>();
    public DbSet<IdentityUserDelegation> UserDelegations => Set<IdentityUserDelegation>();
    public DbSet<IdentitySession> Sessions => Set<IdentitySession>();

    public WorkFlowDbContext(DbContextOptions<WorkFlowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Configure ABP Identity module */
        builder.ConfigureIdentity();
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureAuditLogging();
        builder.ConfigureBackgroundJobs();

        /* Configure your own tables/entities inside here */

        // 配置 AppUser 扩展字段
        builder.Entity<AppUser>(b =>
        {
            b.ToTable("AbpUsers");
            b.Property(u => u.NickName).HasMaxLength(100);
            b.Property(u => u.Avatar).HasMaxLength(500);
            b.Property(u => u.LastLoginIp).HasMaxLength(50);
            b.Property(u => u.Status).HasMaxLength(10);
        });

        // 配置第三方账号绑定
        builder.Entity<UserThirdPartyAccount>(b =>
        {
            b.ToTable("UserThirdPartyAccounts");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(a => new { a.Provider, a.OpenId }).IsUnique();
            b.HasIndex(a => a.UserId);
            b.Property(a => a.Provider).HasMaxLength(50).IsRequired();
            b.Property(a => a.OpenId).HasMaxLength(200).IsRequired();
            b.Property(a => a.UnionId).HasMaxLength(200);
            b.Property(a => a.NickName).HasMaxLength(100);
            b.Property(a => a.Avatar).HasMaxLength(500);
            
            b.HasOne(a => a.User)
                .WithMany(u => u.ThirdPartyAccounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置所有实体主键不自动生成（使用雪花算法手动生成）
        builder.Entity<Tenant>(b =>
        {
            b.ToTable("Tenants");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(t => t.Code).IsUnique();
        });

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(u => u.UserName);
        });

        builder.Entity<Department>(b =>
        {
            b.ToTable("Departments");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
        });

        builder.Entity<ProcessDefinition>(b =>
        {
            b.ToTable("ProcessDefinitions");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(p => new { p.Key, p.Version });
        });

        builder.Entity<ProcessInstance>(b =>
        {
            b.ToTable("ProcessInstances");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
        });

        builder.Entity<TaskInstance>(b =>
        {
            b.ToTable("TaskInstances");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
        });

        builder.Entity<FileStorageProvider>(b =>
        {
            b.ToTable("FileStorageProviders");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(p => p.ProviderType);
            b.HasIndex(p => new { p.TenantId, p.IsEnabled });
            b.Property(p => p.Configuration).HasMaxLength(4000);
        });

        builder.Entity<FileAttachment>(b =>
        {
            b.ToTable("FileAttachments");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(a => new { a.BusinessType, a.BusinessId });
            b.HasIndex(a => a.Md5Hash);
            b.HasIndex(a => a.AccessToken);
            b.HasIndex(a => new { a.TenantId, a.UploadStatus });
            
            b.Property(a => a.FileName).HasMaxLength(500).IsRequired();
            b.Property(a => a.OriginalFileName).HasMaxLength(500).IsRequired();
            b.Property(a => a.ContentType).HasMaxLength(200).IsRequired();
            b.Property(a => a.FileExtension).HasMaxLength(50).IsRequired();
            b.Property(a => a.StoragePath).HasMaxLength(1000).IsRequired();
            b.Property(a => a.Md5Hash).HasMaxLength(64).IsRequired();
            b.Property(a => a.BusinessType).HasMaxLength(100).IsRequired();
            b.Property(a => a.BusinessId).HasMaxLength(100).IsRequired();
            b.Property(a => a.AccessToken).HasMaxLength(500);

            b.HasOne(a => a.StorageProvider)
                .WithMany()
                .HasForeignKey(a => a.StorageProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<FileChunk>(b =>
        {
            b.ToTable("FileChunks");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(c => new { c.AttachmentId, c.ChunkIndex }).IsUnique();
            b.HasIndex(c => new { c.AttachmentId, c.UploadStatus });
            
            b.Property(c => c.ChunkHash).HasMaxLength(64).IsRequired();
            b.Property(c => c.StoragePath).HasMaxLength(1000).IsRequired();

            b.HasOne(c => c.Attachment)
                .WithMany()
                .HasForeignKey(c => c.AttachmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置菜单
        builder.Entity<Menu>(b =>
        {
            b.ToTable("Menus");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.Property(m => m.MenuName).HasMaxLength(100).IsRequired();
            b.Property(m => m.MenuType).HasMaxLength(10).IsRequired();
            b.Property(m => m.Path).HasMaxLength(200);
            b.Property(m => m.Component).HasMaxLength(200);
            b.Property(m => m.PermissionCode).HasMaxLength(200);
            b.Property(m => m.Icon).HasMaxLength(100);
            b.Property(m => m.Status).HasMaxLength(10);
            b.HasIndex(m => new { m.ParentId, m.OrderNum });
        });

        // 配置角色菜单关系
        builder.Entity<RoleMenu>(b =>
        {
            b.ToTable("RoleMenus");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(rm => new { rm.RoleId, rm.MenuId }).IsUnique();
        });

        // 配置字典类型
        builder.Entity<DictType>(b =>
        {
            b.ToTable("DictTypes");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.Property(d => d.DictName).HasMaxLength(100).IsRequired();
            b.Property(d => d.DictTypeCode).HasMaxLength(100).IsRequired();
            b.Property(d => d.Status).HasMaxLength(10);
            b.Property(d => d.Remark).HasMaxLength(500);
            b.HasIndex(d => d.DictTypeCode).IsUnique();
        });

        // 配置字典数据
        builder.Entity<DictData>(b =>
        {
            b.ToTable("DictDatas");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.Property(d => d.DictLabel).HasMaxLength(100).IsRequired();
            b.Property(d => d.DictValue).HasMaxLength(100).IsRequired();
            b.Property(d => d.Status).HasMaxLength(10);
            b.Property(d => d.CssClass).HasMaxLength(100);
            b.Property(d => d.ListClass).HasMaxLength(100);
            b.HasIndex(d => new { d.DictTypeId, d.DictSort });
            
            b.HasOne(d => d.DictType)
                .WithMany(t => t.DictDatas)
                .HasForeignKey(d => d.DictTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置系统配置
        builder.Entity<SystemConfig>(b =>
        {
            b.ToTable("SystemConfigs");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.Property(c => c.ConfigKey).HasMaxLength(100).IsRequired();
            b.Property(c => c.ConfigValue).HasMaxLength(2000).IsRequired();
            b.Property(c => c.ConfigName).HasMaxLength(100).IsRequired();
            b.Property(c => c.ConfigType).HasMaxLength(10);
            b.Property(c => c.Remark).HasMaxLength(500);
            b.HasIndex(c => c.ConfigKey).IsUnique();
        });

        // 配置操作日志
        builder.Entity<OperationLog>(b =>
        {
            b.ToTable("OperationLogs");
            b.ConfigureByConvention();
            b.Property(e => e.Id).ValueGeneratedNever();
            b.Property(l => l.Title).HasMaxLength(100).IsRequired();
            b.Property(l => l.BusinessType).HasMaxLength(50).IsRequired();
            b.Property(l => l.RequestMethod).HasMaxLength(20);
            b.Property(l => l.RequestUrl).HasMaxLength(500);
            b.Property(l => l.Status).HasMaxLength(10);
            b.Property(l => l.OperatorName).HasMaxLength(100);
            b.Property(l => l.OperatorIp).HasMaxLength(50);
            b.Property(l => l.OperatorLocation).HasMaxLength(100);
            b.HasIndex(l => l.CreationTime);
            b.HasIndex(l => l.Status);
        });
    }
}
