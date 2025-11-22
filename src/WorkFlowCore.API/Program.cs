using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorkFlowCore.API;
using WorkFlowCore.API.Middleware;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Repositories;
using WorkFlowCore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAutofac();

// 模型验证统一响应
builder.Services.Configure<ApiBehaviorOptions>(options =>
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
builder.Services.AddScoped<WorkFlowCore.Application.Services.ITenantService, WorkFlowCore.Infrastructure.Services.TenantService>();
builder.Services.AddScoped<WorkFlowCore.Application.Services.IUserService, WorkFlowCore.Infrastructure.Services.UserService>();
builder.Services.AddScoped<WorkFlowCore.Application.Services.IDepartmentService, WorkFlowCore.Infrastructure.Services.DepartmentService>();
builder.Services.AddScoped<WorkFlowCore.Application.Services.IProcessDefinitionService, WorkFlowCore.Infrastructure.Services.ProcessDefinitionService>();

// 注册 JWT 服务
builder.Services.AddSingleton(sp => new JwtService(
    jwtSettings["SecretKey"]!,
    jwtSettings["Issuer"]!,
    jwtSettings["Audience"]!,
    int.Parse(jwtSettings["ExpirationMinutes"]!)
));

// 配置 AutoMapper
builder.Services.AddAutoMapper(typeof(WorkFlowCore.Application.Mappings.MappingProfile));

// 确保数据库已创建和迁移
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    context.Database.Migrate(); // 应用迁移
}

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

// 初始化数据库和测试数据
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    await DbInitializer.InitializeAsync(context);
}

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
// MVP阶段：临时禁用认证和授权
// app.UseAuthentication();
// app.UseAuthorization();

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.MapControllers();

// 启动 WorkflowCore 引擎
var host = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
host.Start();

app.Run();
