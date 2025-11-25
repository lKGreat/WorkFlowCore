using System.Diagnostics;
using System.Net;
using System.Text.Json;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.Common.Exceptions;

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
        var errorCode = ErrorCodes.InternalError;
        var message = "服务器内部错误";
        IDictionary<string, string[]>? errors = null;

        // 根据不同的异常类型返回不同的错误信息
        switch (exception)
        {
            case AppException appException:
                statusCode = appException.StatusCode;
                errorCode = appException.ErrorCode;
                message = appException.Message;
                errors = appException.Errors;
                _logger.LogWarning(exception, "应用异常: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case Volo.Abp.UserFriendlyException userFriendlyException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = ErrorCodes.BusinessError;
                message = userFriendlyException.Message;
                _logger.LogWarning(exception, "用户友好异常: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case Volo.Abp.Authorization.AbpAuthorizationException:
                statusCode = HttpStatusCode.Forbidden;
                errorCode = ErrorCodes.Forbidden;
                message = "您没有权限执行此操作";
                _logger.LogWarning(exception, "授权异常: {Message} | 用户: {User} | 请求路径: {Path}", 
                    message, context.User?.Identity?.Name, context.Request.Path);
                break;

            case Volo.Abp.Validation.AbpValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = ErrorCodes.ValidationError;
                message = "数据验证失败";
                errors = validationException.ValidationErrors
                    .GroupBy(e => e.MemberNames.FirstOrDefault() ?? "")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage ?? "验证失败").ToArray()
                    );
                _logger.LogWarning(exception, "验证异常: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorCode = ErrorCodes.Unauthorized;
                message = "未授权访问";
                _logger.LogWarning(exception, "未授权访问: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorCode = ErrorCodes.NotFound;
                message = exception.Message;
                _logger.LogWarning(exception, "资源未找到: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            case ArgumentException argumentException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = ErrorCodes.ArgumentError;
                message = argumentException.Message;
                _logger.LogWarning(exception, "参数错误: {Message} | 请求路径: {Path}", message, context.Request.Path);
                break;

            default:
                _logger.LogError(exception, "未处理的异常: {Message} | 请求路径: {Path} | 堆栈: {StackTrace}", 
                    exception.Message, context.Request.Path, exception.StackTrace);
                break;
        }

        // 获取 TraceId
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            TraceId = traceId,
            Errors = errors,
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
