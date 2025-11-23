using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 基础控制器
/// </summary>
[Route("api/[controller]")]
public abstract class BaseController : AbpControllerBase
{
    /// <summary>
    /// 测试租户ID（MVP阶段临时使用）
    /// </summary>
    protected static readonly Guid TestTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// 当前租户ID（使用ABP的CurrentTenant服务）
    /// MVP阶段：如果没有提供租户ID，使用测试租户ID
    /// </summary>
    protected Guid CurrentTenantId => CurrentTenant.Id ?? TestTenantId;

    /// <summary>
    /// 当前用户ID（使用ABP的CurrentUser服务）
    /// </summary>
    protected Guid? CurrentUserId => CurrentUser.Id;

    /// <summary>
    /// 记录操作日志
    /// </summary>
    /// <param name="action">操作名称</param>
    /// <param name="message">日志消息</param>
    /// <param name="parameters">参数（可选）</param>
    protected void LogOperation(string action, string message, object? parameters = null)
    {
        Logger.LogInformation(
            "操作日志 | 租户: {TenantId} | 用户: {UserId} | 操作: {Action} | 消息: {Message} | 参数: {@Parameters}",
            CurrentTenantId,
            CurrentUserId,
            action,
            message,
            parameters
        );
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    /// <param name="action">操作名称</param>
    /// <param name="exception">异常</param>
    /// <param name="parameters">参数（可选）</param>
    protected void LogError(string action, Exception exception, object? parameters = null)
    {
        Logger.LogError(
            exception,
            "操作失败 | 租户: {TenantId} | 用户: {UserId} | 操作: {Action} | 参数: {@Parameters}",
            CurrentTenantId,
            CurrentUserId,
            action,
            parameters
        );
    }
}

