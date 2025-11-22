using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using WorkFlowCore.Domain;

namespace WorkFlowCore.Application;

[DependsOn(
    typeof(WorkFlowCoreDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class WorkFlowCoreApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WorkFlowCoreApplicationModule>();
        });
    }
}
