using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorkFlowCore.API;
using WorkFlowCore.API.Middleware;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAutofac();

// ģ����֤ͳһ��Ӧ
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var response = ApiResponse.Fail("���������֤ʧ��", ErrorCodes.ValidationError, errors);
        response.TraceId = context.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ���� JWT ��֤
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

// �������ݿ�������
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WorkFlowDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<WorkFlowCore.Application.Services.IProcessDefinitionService, WorkFlowCore.Infrastructure.Services.ProcessDefinitionService>();

// ע�� JWT ����
builder.Services.AddSingleton(sp => new JwtService(
    jwtSettings["SecretKey"]!,
    jwtSettings["Issuer"]!,
    jwtSettings["Audience"]!,
    int.Parse(jwtSettings["ExpirationMinutes"]!)
));

// ���� AutoMapper
builder.Services.AddAutoMapper(typeof(WorkFlowCore.Application.Mappings.MappingProfile));

// ȷ�����ݿ��Ѵ�����Ǩ��
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    context.Database.Migrate(); // Ӧ��Ǩ��
}

// ���� WorkflowCore
builder.Services.AddWorkflow(x => x.UseSqlite(connectionString!, true));

// ���� CORS����ѡ��
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

// ��ʼ�����ݿ�Ͳ�������
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkFlowDbContext>();
    await DbInitializer.InitializeAsync(context);
}

// ȫ���쳣�����м������������ǰ�棩
app.UseExceptionHandling();

// ���� HTTP ����ܵ�
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
// MVP�׶Σ���ʱ������֤����Ȩ
// app.UseAuthentication();
// app.UseAuthorization();

// �������˵�
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.MapControllers();

// ���� WorkflowCore ����
var host = app.Services.GetRequiredService<global::WorkflowCore.Interface.IWorkflowHost>();
host.Start();

app.Run();
