using System.Net;

namespace WorkFlowCore.Application.Common.Exceptions;

/// <summary>
/// 应用统一异常基类
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message, string errorCode, HttpStatusCode statusCode, IDictionary<string, string[]>? errors = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Errors = errors;
    }

    /// <summary>
    /// 错误代码
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// 应返回的 HTTP 状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// 详细错误信息（一般用于校验失败）
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; }
}

/// <summary>
/// 业务异常
/// </summary>
public sealed class BusinessException : AppException
{
    public BusinessException(string message, string errorCode = ErrorCodes.BusinessError, HttpStatusCode statusCode = HttpStatusCode.BadRequest, IDictionary<string, string[]>? errors = null, Exception? innerException = null)
        : base(message, errorCode, statusCode, errors, innerException)
    {
    }
}

/// <summary>
/// 资源不存在异常
/// </summary>
public sealed class NotFoundException : AppException
{
    public NotFoundException(string message, string errorCode = ErrorCodes.NotFound, Exception? innerException = null)
        : base(message, errorCode, HttpStatusCode.NotFound, null, innerException)
    {
    }
}

/// <summary>
/// 未授权异常
/// </summary>
public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message, string errorCode = ErrorCodes.Unauthorized, Exception? innerException = null)
        : base(message, errorCode, HttpStatusCode.Unauthorized, null, innerException)
    {
    }
}

/// <summary>
/// 校验异常
/// </summary>
public sealed class ValidationException : AppException
{
    public ValidationException(string message, IDictionary<string, string[]> errors, string errorCode = ErrorCodes.ValidationError, Exception? innerException = null)
        : base(message, errorCode, HttpStatusCode.BadRequest, errors, innerException)
    {
    }
}
