using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 基础控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 当前租户ID（从请求头或认证信息中获取）
    /// </summary>
    protected Guid? CurrentTenantId
    {
        get
        {
            if (HttpContext == null)
            {
                return null;
            }

            if (HttpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdValues) &&
                Guid.TryParse(tenantIdValues.FirstOrDefault(), out var tenantIdFromHeader))
            {
                return tenantIdFromHeader;
            }

            var tenantClaim = HttpContext.User?.FindFirst("tenant_id")?.Value ??
                              HttpContext.User?.FindFirst("tenantId")?.Value;

            return Guid.TryParse(tenantClaim, out var tenantIdFromClaim) ? tenantIdFromClaim : null;
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

