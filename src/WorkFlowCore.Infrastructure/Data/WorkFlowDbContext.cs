using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Data;

[ConnectionStringName("Default")]
public class WorkFlowDbContext : AbpDbContext<WorkFlowDbContext>
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<ProcessDefinition> ProcessDefinitions { get; set; }
    public DbSet<ProcessInstance> ProcessInstances { get; set; }
    public DbSet<TaskInstance> TaskInstances { get; set; }
    public DbSet<FileStorageProvider> FileStorageProviders { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }
    public DbSet<FileChunk> FileChunks { get; set; }

    public WorkFlowDbContext(DbContextOptions<WorkFlowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Configure your own tables/entities inside here */

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
    }
}
