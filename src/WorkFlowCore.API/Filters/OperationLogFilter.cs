using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.API.Filters;

/// <summary>
/// 操作日志过滤器
/// </summary>
public class OperationLogFilter : IAsyncActionFilter
{
    private readonly IRepository<OperationLog, long> _logRepository;
    private readonly ICurrentUser _currentUser;

    public OperationLogFilter(
        IRepository<OperationLog, long> logRepository,
        ICurrentUser currentUser)
    {
        _logRepository = logRepository;
        _currentUser = currentUser;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<OperationLogAttribute>()
            .FirstOrDefault();

        if (attribute == null)
        {
            await next();
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var executedContext = await next();
        stopwatch.Stop();

        var log = new OperationLog(
            SnowflakeIdGenerator.NextId(),
            attribute.Title,
            attribute.BusinessType)
        {
            RequestMethod = context.HttpContext.Request.Method,
            RequestUrl = context.HttpContext.Request.Path,
            ExecutionTime = (int)stopwatch.ElapsedMilliseconds,
            Status = executedContext.Exception == null ? "0" : "1",
            OperatorName = _currentUser.UserName,
            OperatorIp = context.HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        if (attribute.SaveRequest)
        {
            try
            {
                log.RequestParams = JsonSerializer.Serialize(context.ActionArguments);
            }
            catch
            {
                // 忽略序列化错误
            }
        }

        if (attribute.SaveResponse && executedContext.Result is ObjectResult objectResult)
        {
            try
            {
                log.ResponseResult = JsonSerializer.Serialize(objectResult.Value);
            }
            catch
            {
                // 忽略序列化错误
            }
        }

        if (executedContext.Exception != null)
        {
            log.ErrorMsg = executedContext.Exception.Message;
        }

        await _logRepository.InsertAsync(log);
    }
}

