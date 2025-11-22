using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using WorkFlowCore.Domain;
using WorkFlowCore.Infrastructure.Data;

namespace WorkFlowCore.Infrastructure;

[DependsOn(
    typeof(WorkFlowCoreDomainModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class WorkFlowCoreEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WorkFlowDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also WorkFlowDbContextFactory for EF Core tooling. */
            options.UseSqlite();
        });
    }
}
