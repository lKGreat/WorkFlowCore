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
}

