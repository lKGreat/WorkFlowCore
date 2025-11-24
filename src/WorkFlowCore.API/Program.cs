using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using Volo.Abp;
using WorkFlowCore.API;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("启动 WorkFlowCore 应用程序...");

    // 初始化雪花算法主键生成器
    var workerId = builder.Configuration.GetValue<long>("SnowflakeId:WorkerId", 1);
    var datacenterId = builder.Configuration.GetValue<long>("SnowflakeId:DatacenterId", 1);
    SnowflakeIdGenerator.Initialize(workerId, datacenterId);
    Log.Information("雪花算法主键生成器已初始化 WorkerId={WorkerId}, DatacenterId={DatacenterId}", workerId, datacenterId);

    // 使用Autofac作为DI容器
    builder.Host.UseAutofac();

    // 添加ABP应用程序（增加详细错误日志）
    try
    {
        await builder.AddApplicationAsync<WorkFlowCoreHttpApiModule>();
        Log.Information("ABP 应用程序模块已添加");
    }
    catch (AbpInitializationException ex) when (ex.InnerException is ReflectionTypeLoadException rtle)
    {
        Log.Error(ex, "ABP 初始化失败，Loader 异常信息：");
        foreach (var loaderEx in rtle.LoaderExceptions)
        {
            if (loaderEx is not null)
            {
                Log.Error(loaderEx, "Loader Exception");
            }
        }

        throw;
    }

    var app = builder.Build();

    // 初始化ABP应用程序（必须先初始化，WorkflowCore 服务才能正常注册）
    await app.InitializeApplicationAsync();
    Log.Information("ABP 应用程序已初始化");

    // 应用数据库迁移（ABP 初始化后执行）
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
        await context.Database.MigrateAsync();
        Log.Information("数据库迁移已完成");
    }

    // 初始化测试数据
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
        await DbInitializer.InitializeAsync(context);
        Log.Information("测试数据已初始化");
        
        // 初始化默认管理员账号
        await DataSeeder.SeedAdminUserAsync(scope.ServiceProvider, context);
        Log.Information("默认用户数据已初始化");
    }

    // 启动WorkflowCore（使用独立数据库 workflow_engine.db，自动创建表）
    var workflowHost = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
    workflowHost.Start();
    Log.Information("WorkflowCore 引擎已启动");

    // 健康检查端点
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
        .WithName("HealthCheck")
        .WithTags("Health");

    Log.Information("应用程序启动成功，开始监听请求...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序启动失败");
    throw;
}
finally
{
    Log.Information("应用程序正在关闭...");
    await Log.CloseAndFlushAsync();
}
