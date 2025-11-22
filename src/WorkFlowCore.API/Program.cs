using Microsoft.EntityFrameworkCore;
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

// 添加ABP应用程序
await builder.AddApplicationAsync<WorkFlowCoreHttpApiModule>();

// 应用数据库迁移
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    await context.Database.MigrateAsync();
}

var app = builder.Build();

// 初始化ABP应用程序
await app.InitializeApplicationAsync();

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
