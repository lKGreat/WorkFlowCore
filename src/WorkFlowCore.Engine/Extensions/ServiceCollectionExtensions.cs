using Microsoft.Extensions.DependencyInjection;

namespace WorkFlowCore.Engine.Extensions;

/// <summary>
/// 服务注册扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册工作流引擎服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWorkflowEngine(this IServiceCollection services)
    {
        // 注册工作流引擎服务（Scoped 生命周期，与 DbContext 一致）
        services.AddScoped<IWorkflowEngine, WorkflowEngineService>();

        return services;
    }
}

