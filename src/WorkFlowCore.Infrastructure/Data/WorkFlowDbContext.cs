using Microsoft.EntityFrameworkCore;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Data;

/// <summary>
/// 工作流数据库上下文
/// </summary>
public class WorkFlowDbContext : DbContext
{
    public WorkFlowDbContext(DbContextOptions<WorkFlowDbContext> options) : base(options)
    {
    }

    // DbSet 定义
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<ProcessDefinition> ProcessDefinitions => Set<ProcessDefinition>();
    public DbSet<ProcessInstance> ProcessInstances => Set<ProcessInstance>();
    public DbSet<TaskInstance> TaskInstances => Set<TaskInstance>();
    public DbSet<FileResource> FileResources => Set<FileResource>();
    public DbSet<FileUploadSession> FileUploadSessions => Set<FileUploadSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置表名
        modelBuilder.Entity<Tenant>().ToTable("Tenants");
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Department>().ToTable("Departments");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<ProcessDefinition>().ToTable("ProcessDefinitions");
        modelBuilder.Entity<ProcessInstance>().ToTable("ProcessInstances");
        modelBuilder.Entity<TaskInstance>().ToTable("TaskInstances");
        modelBuilder.Entity<FileResource>().ToTable("FileResources");
        modelBuilder.Entity<FileUploadSession>().ToTable("FileUploadSessions");

        // 配置索引
        modelBuilder.Entity<Tenant>().HasIndex(t => t.Code).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.UserName);
        modelBuilder.Entity<ProcessDefinition>().HasIndex(p => new { p.Key, p.Version });
        modelBuilder.Entity<FileResource>().HasIndex(f => new { f.TenantId, f.StorageProvider });
        modelBuilder.Entity<FileResource>().HasIndex(f => f.StorageKey);
        modelBuilder.Entity<FileUploadSession>().HasIndex(s => new { s.TenantId, s.StorageProvider, s.IsCompleted });

        // 全局查询过滤器：软删除
        modelBuilder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProcessDefinition>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<FileResource>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<FileUploadSession>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 更新审计字段
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Entity<Guid> entity)
            {
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}

