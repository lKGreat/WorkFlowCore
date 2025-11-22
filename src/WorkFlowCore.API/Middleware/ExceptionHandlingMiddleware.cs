using System.Net;
using System.Text.Json;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.Common.Exceptions;

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

        ApiResponse response = exception switch
        {
            AppException appException => BuildResponse(context, appException.StatusCode, appException.Message, appException.ErrorCode, appException.Errors),
            ArgumentNullException => BuildResponse(context, HttpStatusCode.BadRequest, "参数不能为空", ErrorCodes.ArgumentError),
            ArgumentException => BuildResponse(context, HttpStatusCode.BadRequest, exception.Message, ErrorCodes.ArgumentError),
            InvalidOperationException => BuildResponse(context, HttpStatusCode.BadRequest, exception.Message, ErrorCodes.InvalidOperation),
            UnauthorizedAccessException => BuildResponse(context, HttpStatusCode.Unauthorized, "未授权访问", ErrorCodes.Unauthorized),
            KeyNotFoundException => BuildResponse(context, HttpStatusCode.NotFound, "资源不存在", ErrorCodes.NotFound),
            _ => BuildResponse(context, HttpStatusCode.InternalServerError, "服务器内部错误", ErrorCodes.InternalError)
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }

    private static ApiResponse BuildResponse(HttpContext context, HttpStatusCode statusCode, string message, string errorCode, IDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = (int)statusCode;
        var response = ApiResponse.Fail(message, errorCode, errors);
        response.TraceId = context.TraceIdentifier;
        return response;
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

