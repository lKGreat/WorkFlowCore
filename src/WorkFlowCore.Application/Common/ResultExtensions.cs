using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WorkFlowCore.Application.Common;

/// <summary>
/// 结果扩展方法
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// 转换为 ActionResult
    /// </summary>
    public static ActionResult<ApiResponse<T>> ToActionResult<T>(this ApiResponse<T> response)
    {
        response.TraceId = Activity.Current?.Id ?? response.TraceId;
        if (string.IsNullOrWhiteSpace(response.TraceId))
        {
            response.TraceId = Guid.NewGuid().ToString("N");
        }

        if (response.Success)
        {
            return new OkObjectResult(response);
        }

        // 根据错误代码返回不同的状态码
        return response.ErrorCode switch
        {
            ErrorCodes.NotFound => new NotFoundObjectResult(response),
            ErrorCodes.Unauthorized => new UnauthorizedResult(),
            ErrorCodes.Forbidden => new ObjectResult(response) { StatusCode = StatusCodes.Status403Forbidden },
            ErrorCodes.ValidationError => new BadRequestObjectResult(response),
            _ => new BadRequestObjectResult(response)
        };
    }

    /// <summary>
    /// 转换为 ActionResult（无数据）
    /// </summary>
    public static ActionResult<ApiResponse> ToActionResult(this ApiResponse response)
    {
        response.TraceId = Activity.Current?.Id ?? response.TraceId;
        if (string.IsNullOrWhiteSpace(response.TraceId))
        {
            response.TraceId = Guid.NewGuid().ToString("N");
        }

        if (response.Success)
        {
            return new OkObjectResult(response);
        }

        return response.ErrorCode switch
        {
            ErrorCodes.NotFound => new NotFoundObjectResult(response),
            ErrorCodes.Unauthorized => new UnauthorizedResult(),
            ErrorCodes.Forbidden => new ObjectResult(response) { StatusCode = StatusCodes.Status403Forbidden },
            ErrorCodes.ValidationError => new BadRequestObjectResult(response),
            _ => new BadRequestObjectResult(response)
        };
    }
}

