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
            // TODO: 从请求头或 JWT Token 中获取租户ID
            // 目前返回固定值用于测试
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }

    /// <summary>
    /// 当前用户ID（从认证信息中获取）
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            // TODO: 从 JWT Token 中获取用户ID
            return null;
        }
    }
}

