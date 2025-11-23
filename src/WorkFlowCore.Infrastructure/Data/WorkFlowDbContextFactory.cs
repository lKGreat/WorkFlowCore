using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WorkFlowCore.Infrastructure.Data;

/// <summary>
/// EF Core 设计时 DbContext 工厂
/// </summary>
public class WorkFlowDbContextFactory : IDesignTimeDbContextFactory<WorkFlowDbContext>
{
    public WorkFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorkFlowDbContext>();
        optionsBuilder.UseSqlite("Data Source=workflow.db");

        return new WorkFlowDbContext(optionsBuilder.Options);
    }
}

