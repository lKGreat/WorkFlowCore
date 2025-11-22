using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using WorkFlowCore.Domain;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Storage;

namespace WorkFlowCore.Infrastructure;

[DependsOn(
    typeof(WorkFlowCoreDomainModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringFileSystemModule)
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

        // 启用多租户数据过滤器(软删除过滤器ABP已默认启用)
        Configure<AbpDataFilterOptions>(options =>
        {
            options.DefaultStates[typeof(IMultiTenant)] = new DataFilterState(isEnabled: true);
        });

        // 配置 ABP BlobStoring
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

        // 注册存储服务
        context.Services.AddTransient<LocalStorageProvider>();
        context.Services.AddTransient<StorageProviderFactory>();
    }
}
