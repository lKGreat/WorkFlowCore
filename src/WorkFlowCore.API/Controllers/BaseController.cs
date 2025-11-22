using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 基础控制器
/// </summary>
[Route("api/[controller]")]
public abstract class BaseController : AbpController
{
}
