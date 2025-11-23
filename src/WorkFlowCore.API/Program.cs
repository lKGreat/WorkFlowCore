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

// 初始化ABP应用程序（必须先初始化，WorkflowCore 服务才能正常注册）
await app.InitializeApplicationAsync();

// 应用数据库迁移（ABP 初始化后执行）
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    // #region agent log
    var connStr = context.Database.GetConnectionString();
    System.IO.File.AppendAllText(@"d:\Code\WorkFlowCore\WorkFlowCore\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId="debug-session", runId="post-fix", hypothesisId="H6", location="Program.cs:46", message="EF迁移：连接字符串", data=new{ connectionString=connStr }, timestamp=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
    // #endregion
    await context.Database.MigrateAsync();
    // #region agent log
    System.IO.File.AppendAllText(@"d:\Code\WorkFlowCore\WorkFlowCore\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId="debug-session", runId="post-fix", hypothesisId="H6", location="Program.cs:50", message="EF迁移：完成", data=new{ success=true }, timestamp=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
    // #endregion
}

// 初始化测试数据
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    await DbInitializer.InitializeAsync(context);
}

// 启动WorkflowCore（使用独立数据库 workflow_engine.db，自动创建表）
var workflowHost = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
// #region agent log
var wfConnStr = builder.Configuration.GetConnectionString("WorkflowEngine") ?? builder.Configuration.GetConnectionString("Default");
System.IO.File.AppendAllText(@"d:\Code\WorkFlowCore\WorkFlowCore\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId="debug-session", runId="final-fix-v2", hypothesisId="H10", location="Program.cs:65", message="WorkflowCore启动前：连接字符串", data=new{ connectionString=wfConnStr }, timestamp=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
// #endregion
try
{
    workflowHost.Start();
    // #region agent log
    System.IO.File.AppendAllText(@"d:\Code\WorkFlowCore\WorkFlowCore\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId="debug-session", runId="final-fix-v2", hypothesisId="H10", location="Program.cs:71", message="WorkflowCore启动：成功", data=new{ started=true }, timestamp=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
    // #endregion
}
catch (Exception ex)
{
    // #region agent log
    System.IO.File.AppendAllText(@"d:\Code\WorkFlowCore\WorkFlowCore\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId="debug-session", runId="final-fix-v2", hypothesisId="H10", location="Program.cs:76", message="WorkflowCore启动：失败", data=new{ error=ex.Message, type=ex.GetType().Name }, timestamp=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
    // #endregion
    Console.WriteLine($"WorkflowCore 启动失败: {ex.Message}");
    throw;
}

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

await app.RunAsync();
