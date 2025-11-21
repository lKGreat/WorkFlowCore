using Microsoft.EntityFrameworkCore;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WorkFlowDbContext>(options =>
    options.UseSqlite(connectionString));

// 注册仓储
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 配置 AutoMapper
builder.Services.AddAutoMapper(typeof(WorkFlowCore.Application.Mappings.MappingProfile));

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.MapControllers();

app.Run();
