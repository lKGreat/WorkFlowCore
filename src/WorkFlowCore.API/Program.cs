using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Volo.Abp;
using WorkFlowCore.API.Middleware;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.FileStorage;
using WorkFlowCore.Infrastructure.Repositories;
using WorkFlowCore.Infrastructure.Services;

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

// 配置 JWT 认证
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
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

// 配置数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WorkFlowDbContext>(options =>
    options.UseSqlite(connectionString));

// 注册仓储
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPagedRepository<>), typeof(PagedRepository<>));

// 注册服务
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// 注册 JWT 服务
builder.Services.AddSingleton(sp => new JwtService(
    jwtSettings["SecretKey"]!,
    jwtSettings["Issuer"]!,
    jwtSettings["Audience"]!,
    int.Parse(jwtSettings["ExpirationMinutes"]!)
));

// 配置 AutoMapper
builder.Services.AddAutoMapper(typeof(WorkFlowCore.Application.Mappings.MappingProfile));

// 配置 WorkflowCore
builder.Services.AddWorkflow(x => x.UseSqlite(connectionString!, true));

// 注册 ABP 文件存储模块
builder.Services.AddApplication<WorkFlowCoreFileStorageModule>();

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
app.UseAuthentication();
app.UseAuthorization();

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.MapControllers();

app.InitializeApplication();

// 启动 WorkflowCore 引擎
var host = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
host.Start();

app.Run();
