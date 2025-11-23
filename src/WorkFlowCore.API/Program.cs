using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Volo.Abp;
using WorkFlowCore.API;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 初始化雪花算法主键生成器
var workerId = builder.Configuration.GetValue<long>("SnowflakeId:WorkerId", 1);
var datacenterId = builder.Configuration.GetValue<long>("SnowflakeId:DatacenterId", 1);
SnowflakeIdGenerator.Initialize(workerId, datacenterId);

// 使用Autofac作为DI容器
builder.Host.UseAutofac();

// 添加ABP应用程序（增加详细错误日志）
try
{
    await builder.AddApplicationAsync<WorkFlowCoreHttpApiModule>();
}
catch (AbpInitializationException ex) when (ex.InnerException is ReflectionTypeLoadException rtle)
{
    Console.WriteLine("AbpInitializationException during AddApplicationAsync. Loader exceptions:");
    foreach (var loaderEx in rtle.LoaderExceptions)
    {
        if (loaderEx is not null)
        {
            Console.WriteLine(loaderEx.ToString());
        }
    }

    throw;
}

var app = builder.Build();

// 初始化ABP应用程序
await app.InitializeApplicationAsync();

// 应用数据库迁移
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    await context.Database.MigrateAsync();
}

// 初始化测试数据
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    await DbInitializer.InitializeAsync(context);
}

// 启动WorkflowCore
var workflowHost = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
workflowHost.Start();

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

await app.RunAsync();
