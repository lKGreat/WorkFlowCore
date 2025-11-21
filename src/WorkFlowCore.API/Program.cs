using Microsoft.EntityFrameworkCore;
using WorkFlowCore.API.Middleware;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 配置 JSON 序列化选项
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WorkFlowDbContext>(options =>
    options.UseSqlite(connectionString));

// 注册仓储
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPagedRepository<>), typeof(PagedRepository<>));

// 注册服务
builder.Services.AddScoped<WorkFlowCore.Application.Services.ITenantService, WorkFlowCore.Infrastructure.Services.TenantService>();
builder.Services.AddScoped<WorkFlowCore.Application.Services.IUserService, WorkFlowCore.Infrastructure.Services.UserService>();
builder.Services.AddScoped<WorkFlowCore.Application.Services.IDepartmentService, WorkFlowCore.Infrastructure.Services.DepartmentService>();

// 配置 AutoMapper
builder.Services.AddAutoMapper(typeof(WorkFlowCore.Application.Mappings.MappingProfile));

// 配置 WorkflowCore
builder.Services.AddWorkflow(x => x.UseSqlite(connectionString!, true));

// 配置 CORS（可选）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 全局异常处理中间件（必须在最前面）
app.UseExceptionHandling();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkFlow Core API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.MapControllers();

// 启动 WorkflowCore 引擎
var host = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
host.Start();

app.Run();
