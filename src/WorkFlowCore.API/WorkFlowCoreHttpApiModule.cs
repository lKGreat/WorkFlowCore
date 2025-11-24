using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using WorkFlowCore.API.Middleware;
using WorkFlowCore.Application;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.Services.Sms;
using WorkFlowCore.Infrastructure;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Services;
using Microsoft.AspNetCore.HttpLogging;

namespace WorkFlowCore.API;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(WorkFlowCoreApplicationModule),
    typeof(WorkFlowCoreEntityFrameworkCoreModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpAccountWebOpenIddictModule)
)]
public class WorkFlowCoreHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureJson(context.Services);
        ConfigureSwaggerServices(context.Services);
        ConfigureCors(context, configuration);
        ConfigureAuthentication(context.Services, configuration);
        ConfigureCaptcha(context.Services);
        ConfigureWorkflowCore(context.Services, configuration);
        ConfigureExceptionHandling(context.Services);
        ConfigureValidation(context.Services);
        ConfigureAutoMapper(context.Services);
        ConfigureHttpLogging(context.Services, hostingEnvironment);
        ConfigureCustomServices(context.Services, configuration);
    }

    private void ConfigureJson(IServiceCollection services)
    {
        // 使用 Newtonsoft.Json 进行 JSON 序列化
        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                // 使用 camelCase 命名策略
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                // 忽略 null 值
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                // 日期格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 循环引用处理
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            //options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkFlow Core API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("CORS:AllowedOrigins").Get<string[]>()
                             ?? new[] { "http://localhost:5173" };

        context.Services.AddCors(options =>
        {
            options.AddPolicy("Default", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials(); // 允许携带凭证（Cookie、Authorization Header）
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
        
        // 只在配置了 ClientId 时才添加第三方登录（开发环境可选）
        var wechatAppId = configuration["Authentication:WeChat:AppId"];
        if (!string.IsNullOrEmpty(wechatAppId))
        {
            services.AddAuthentication().AddWeixin(options =>
            {
                options.ClientId = wechatAppId;
                options.ClientSecret = configuration["Authentication:WeChat:AppSecret"]!;
            });
        }
        
        var qqAppId = configuration["Authentication:QQ:AppId"];
        if (!string.IsNullOrEmpty(qqAppId))
        {
            services.AddAuthentication().AddQQ(options =>
            {
                options.ClientId = qqAppId;
                options.ClientSecret = configuration["Authentication:QQ:AppKey"]!;
            });
        }

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

    private void ConfigureCaptcha(IServiceCollection services)
    {
        // 配置 Lazy.Captcha (暂时注释,使用自定义实现)
        // services.AddCaptcha(configuration =>
        // {
        //     configuration.CodeLength = 4;
        //     configuration.ExpirySeconds = 120;
        //     configuration.IgnoreCase = true;
        //     configuration.StoreageKeyPrefix = "captcha:";
        // });
    }

    private void ConfigureWorkflowCore(IServiceCollection services, IConfiguration configuration)
    {
        // 使用独立的数据库文件，避免与 ABP 业务表冲突
        var connectionString = configuration.GetConnectionString("WorkflowEngine") 
                               ?? configuration.GetConnectionString("Default");
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

        // 注册操作日志过滤器
        Configure<MvcOptions>(options =>
        {
            options.Filters.Add<API.Filters.OperationLogFilter>();
        });
    }

    private void ConfigureAutoMapper(IServiceCollection services)
    {
        // AutoMapper在ApplicationModule中已配置，这里无需重复
    }

    private void ConfigureHttpLogging(IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddHttpLogging(options =>
        {
            // 记录请求和响应的详细信息
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;

            // 敏感信息头部脱敏
            options.RequestHeaders.Add("Authorization");
            options.RequestHeaders.Add("Cookie");
            options.ResponseHeaders.Add("Set-Cookie");

            // 请求体大小限制（32KB）
            options.RequestBodyLogLimit = 32 * 1024;
            options.ResponseBodyLogLimit = 32 * 1024;

            // 敏感字段脱敏
            options.MediaTypeOptions.AddText("application/json");
            options.MediaTypeOptions.AddText("application/xml");

            // 开发环境记录更多信息
            if (environment.IsDevelopment())
            {
                options.CombineLogs = false; // 分开记录请求和响应
            }
            else
            {
                options.CombineLogs = true; // 生产环境合并日志
                options.RequestBodyLogLimit = 4 * 1024;
                options.ResponseBodyLogLimit = 4 * 1024;
            }
        });
    }

    private void ConfigureCustomServices(IServiceCollection services, IConfiguration configuration)
    {
        // 注册HttpContextAccessor
        services.AddHttpContextAccessor();

        // 配置分布式内存缓存(开发环境使用,生产环境应使用Redis)
        services.AddDistributedMemoryCache();

        // 注册当前用户服务（特殊服务，依赖 IHttpContextAccessor）
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 其他服务已通过 ABP 约定自动注册（实现 IApplicationService 或 ITransientDependency 接口）
        // - ProcessDefinitionService, MenuService, AppUserService 等继承自 ApplicationService (自动注册为 Scoped)
        // - SmsCodeService, CaptchaService, QrCodeLoginService, ThirdPartyLoginService 实现 ITransientDependency
        // - SmsService 实现 ITransientDependency

        // #region agent log
        try
        {
            var smsServiceRegistered = services.Any(d => d.ServiceType == typeof(WorkFlowCore.Application.Services.Sms.SmsService));
            AgentDebugLog("H1", "WorkFlowCoreHttpApiModule.ConfigureCustomServices", "SmsService registration status", new { smsServiceRegistered });

            var smsProviderRegistrations = services.Where(d => d.ServiceType == typeof(WorkFlowCore.Application.Services.Sms.ISmsProvider)).Select(d => d.ImplementationType?.FullName).ToArray();
            AgentDebugLog("H2", "WorkFlowCoreHttpApiModule.ConfigureCustomServices", "Sms provider registrations", new { count = smsProviderRegistrations.Length, smsProviderRegistrations });

            var smsCodeServiceRegistered = services.Any(d => d.ServiceType == typeof(WorkFlowCore.Application.Services.Auth.ISmsCodeService));
            AgentDebugLog("H3", "WorkFlowCoreHttpApiModule.ConfigureCustomServices", "SmsCodeService registration status", new { smsCodeServiceRegistered });
        }
        catch
        {
            // ignored - instrumentation only
        }
        // #endregion
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        // 使用官方 HTTP Logging 中间件（放在最前面）
        app.UseHttpLogging();

        // 注册全局异常处理中间件
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkFlow Core API v1");
            });
        }

        app.UseCors("Default");
        app.UseHttpsRedirection();
        app.UseRouting(); 
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseConfiguredEndpoints();

        // WorkflowCore 在 Program.cs 中启动，避免重复启动
    }

    // #region agent log
    private const string AgentSessionId = "debug-session";
    private const string AgentRunId = "run1";
    private static readonly string AgentLogPath = @"d:\Code\WorkFlowCore\WorkFlowCore\.cursor\debug.log";

    private static void AgentDebugLog(string hypothesisId, string location, string message, object data)
    {
        try
        {
            var payload = new
            {
                sessionId = AgentSessionId,
                runId = AgentRunId,
                hypothesisId,
                location,
                message,
                data,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            File.AppendAllText(AgentLogPath, json + Environment.NewLine);
        }
        catch
        {
            // swallow instrumentation exceptions
        }
    }
    // #endregion
}
