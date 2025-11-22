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

    public WorkFlowDbContext(DbContextOptions<WorkFlowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Configure your own tables/entities inside here */

        builder.Entity<Tenant>(b =>
        {
            b.ToTable("Tenants");
            b.ConfigureByConvention();
            b.HasIndex(t => t.Code).IsUnique();
        });

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.ConfigureByConvention();
            b.HasIndex(u => u.UserName);
        });

        builder.Entity<Department>(b =>
        {
            b.ToTable("Departments");
            b.ConfigureByConvention();
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.ConfigureByConvention();
        });

        builder.Entity<ProcessDefinition>(b =>
        {
            b.ToTable("ProcessDefinitions");
            b.ConfigureByConvention();
            b.HasIndex(p => new { p.Key, p.Version });
        });

        builder.Entity<ProcessInstance>(b =>
        {
            b.ToTable("ProcessInstances");
            b.ConfigureByConvention();
        });

        builder.Entity<TaskInstance>(b =>
        {
            b.ToTable("TaskInstances");
            b.ConfigureByConvention();
        });
    }
}
