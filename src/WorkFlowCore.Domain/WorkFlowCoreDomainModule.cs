using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace WorkFlowCore.Domain;

[DependsOn(
    typeof(AbpDomainModule)
)]
public class WorkFlowCoreDomainModule : AbpModule
{
}
