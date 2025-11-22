using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using WorkFlowCore.Application;
using WorkFlowCore.Infrastructure;
using WorkFlowCore.Infrastructure.Services; 

namespace WorkFlowCore.API;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(WorkFlowCoreApplicationModule),
    typeof(WorkFlowCoreEntityFrameworkCoreModule)
)]
public class WorkFlowCoreHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureSwaggerServices(context.Services);
        ConfigureCors(context, configuration);
        ConfigureJwt(context.Services, configuration);
        ConfigureWorkflowCore(context.Services, configuration);
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkFlow Core API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }

    private void ConfigureJwt(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        if (jwtSettings.Exists())
        {
             services.AddSingleton(sp => new JwtService(
                jwtSettings["SecretKey"]!,
                jwtSettings["Issuer"]!,
                jwtSettings["Audience"]!,
                int.Parse(jwtSettings["ExpirationMinutes"]!)
            ));
        }
    }

    private void ConfigureWorkflowCore(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddWorkflow(x => x.UseSqlite(connectionString!, true));
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkFlow Core API v1");
            });
        }

        app.UseCors("AllowAll");
        app.UseHttpsRedirection();
        app.UseRouting(); 
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseConfiguredEndpoints();

        // Start WorkflowCore
        var host = context.ServiceProvider.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
        host.Start();
    }
}
