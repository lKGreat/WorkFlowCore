using System.Net;
using System.Text.Json;
using WorkFlowCore.Application.Common;

namespace WorkFlowCore.API.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "INTERNAL_ERROR";
        var message = "服务器内部错误";

        // 根据不同的异常类型返回不同的错误信息
        switch (exception)
        {
            case Volo.Abp.UserFriendlyException userFriendlyException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "USER_FRIENDLY_ERROR";
                message = userFriendlyException.Message;
                _logger.LogWarning(exception, "用户友好异常: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case Volo.Abp.Authorization.AbpAuthorizationException:
                statusCode = HttpStatusCode.Forbidden;
                errorCode = "AUTHORIZATION_ERROR";
                message = "您没有权限执行此操作";
                _logger.LogWarning(exception, "授权异常: {Message} | 用户: {User} | 请求路径: {Path}", 
                    message, context.User?.Identity?.Name, context.Request.Path);
                break;

            case Volo.Abp.Validation.AbpValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "VALIDATION_ERROR";
                message = "数据验证失败: " + string.Join(", ", validationException.ValidationErrors.Select(e => e.ErrorMessage));
                _logger.LogWarning(exception, "验证异常: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorCode = "UNAUTHORIZED";
                message = "未授权访问";
                _logger.LogWarning(exception, "未授权访问: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorCode = "NOT_FOUND";
                message = exception.Message;
                _logger.LogWarning(exception, "资源未找到: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case ArgumentException argumentException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "INVALID_ARGUMENT";
                message = argumentException.Message;
                _logger.LogWarning(exception, "参数错误: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            default:
                _logger.LogError(exception, "未处理的异常: {Message} | 请求路径: {Path} | 堆栈: {StackTrace}", 
                    exception.Message, context.Request.Path, exception.StackTrace);
                break;
        }

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

