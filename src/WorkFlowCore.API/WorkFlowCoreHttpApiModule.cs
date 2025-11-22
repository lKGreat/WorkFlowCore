using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using WorkFlowCore.Application;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Infrastructure;
using WorkFlowCore.Infrastructure.Data;
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
        ConfigureAuthentication(context.Services, configuration);
        ConfigureWorkflowCore(context.Services, configuration);
        ConfigureExceptionHandling(context.Services);
        ConfigureValidation(context.Services);
        ConfigureAutoMapper(context.Services);
        ConfigureCustomServices(context.Services, configuration);
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

    private void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        
        // 配置JWT认证
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // 注册JWT服务
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

    private void ConfigureExceptionHandling(IServiceCollection services)
    {
        // 配置ABP异常处理（ABP会自动添加异常过滤器）
        Configure<AbpExceptionHandlingOptions>(options =>
        {
            options.SendExceptionsDetailsToClients = true; // 开发环境发送详细异常信息
            options.SendStackTraceToClients = true; // 开发环境发送堆栈跟踪
        });
    }

    private void ConfigureValidation(IServiceCollection services)
    {
        // 配置模型验证统一响应
        Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                var response = ApiResponse.Fail("请求参数验证失败", ErrorCodes.ValidationError, errors);
                response.TraceId = context.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(response);
            };
        });
    }

    private void ConfigureAutoMapper(IServiceCollection services)
    {
        // AutoMapper在ApplicationModule中已配置，这里无需重复
    }

    private void ConfigureCustomServices(IServiceCollection services, IConfiguration configuration)
    {
        // 注册HttpContextAccessor
        services.AddHttpContextAccessor();

        // 注册当前用户服务
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 注册流程定义服务
        services.AddScoped<Application.Services.IProcessDefinitionService, ProcessDefinitionService>();

        // 注册文件存储服务
        services.AddScoped<Application.Services.IFileStorageProviderService, FileStorageProviderService>();
        services.AddScoped<Application.Services.IFileUploadService, FileUploadService>();
        services.AddScoped<Application.Services.IFileAccessService, FileAccessService>();
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
