using System.Net;
using System.Text.Json;
using WorkFlowCore.Application.Common;

namespace WorkFlowCore.API.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "请求处理时发生异常");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = exception switch
        {
            ArgumentNullException => ApiResponse.Fail("参数不能为空", "ARGUMENT_NULL"),
            ArgumentException => ApiResponse.Fail(exception.Message, "ARGUMENT_ERROR"),
            InvalidOperationException => ApiResponse.Fail(exception.Message, "INVALID_OPERATION"),
            UnauthorizedAccessException => ApiResponse.Fail("未授权访问", "UNAUTHORIZED"),
            KeyNotFoundException => ApiResponse.Fail("资源不存在", "NOT_FOUND"),
            _ => ApiResponse.Fail("服务器内部错误", "INTERNAL_ERROR")
        };

        // 根据异常类型设置状态码
        context.Response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// 中间件扩展方法
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

