using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace WorkFlowCore.Domain;

[DependsOn(
    typeof(AbpDddDomainModule)
)]
public class WorkFlowCoreDomainModule : AbpModule
{
}
