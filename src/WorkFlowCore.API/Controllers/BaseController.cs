using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 基础控制器
/// </summary>
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 测试租户ID（MVP阶段临时使用）
    /// </summary>
    protected static readonly Guid TestTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// 当前租户ID（从请求头或认证信息中获取）
    /// MVP阶段：如果没有提供租户ID，使用测试租户ID
    /// </summary>
    protected Guid CurrentTenantId
    {
        get
        {
            if (HttpContext == null)
            {
                return TestTenantId;
            }

            if (HttpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdValues) &&
                Guid.TryParse(tenantIdValues.FirstOrDefault(), out var tenantIdFromHeader))
            {
                return tenantIdFromHeader;
            }

            var tenantClaim = HttpContext.User?.FindFirst("tenant_id")?.Value ??
                              HttpContext.User?.FindFirst("tenantId")?.Value;

            if (Guid.TryParse(tenantClaim, out var tenantIdFromClaim))
            {
                return tenantIdFromClaim;
            }

            // MVP阶段：返回测试租户ID
            return TestTenantId;
        }
    }

    /// <summary>
    /// 当前用户ID（从认证信息中获取）
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            var user = HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                              user.FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}

