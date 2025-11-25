using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 通知公告控制器
/// </summary>
[Authorize]
[Route("api/system/notice")]
public class NoticeController : BaseController
{
    private readonly INoticeService _noticeService;

    public NoticeController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    /// <summary>
    /// 获取通知公告列表
    /// </summary>
    [HttpGet("list")]
    [ActionPermissionFilter(Permission = "system:notice:list")]
    [OperationLog("查询通知公告", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<NoticeDto>>>> GetList([FromQuery] NoticeQueryDto query)
    {
        var result = await _noticeService.GetPagedListAsync(query);
        return ApiResponse<PagedResponse<NoticeDto>>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取通知公告详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NoticeDto>>> GetById(long id)
    {
        var notice = await _noticeService.GetByIdAsync(id);
        if (notice == null)
        {
            return ApiResponse<NoticeDto>.Fail("通知公告不存在").ToActionResult();
        }
        return ApiResponse<NoticeDto>.Ok(notice).ToActionResult();
    }

    /// <summary>
    /// 新增通知公告
    /// </summary>
    [HttpPost]
    [ActionPermissionFilter(Permission = "system:notice:add")]
    [OperationLog("新增通知公告", "INSERT")]
    public async Task<ActionResult<ApiResponse<NoticeDto>>> Create([FromBody] NoticeDto dto)
    {
        // 设置发布人
        dto.Publisher = CurrentUser.UserName;
        
        var result = await _noticeService.CreateAsync(dto);
        return ApiResponse<NoticeDto>.Ok(result, "创建成功").ToActionResult();
    }

    /// <summary>
    /// 修改通知公告
    /// </summary>
    [HttpPut]
    [ActionPermissionFilter(Permission = "system:notice:edit")]
    [OperationLog("修改通知公告", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update([FromBody] NoticeDto dto)
    {
        await _noticeService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    /// <summary>
    /// 删除通知公告
    /// </summary>
    [HttpDelete("{noticeIds}")]
    [ActionPermissionFilter(Permission = "system:notice:remove")]
    [OperationLog("删除通知公告", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string noticeIds)
    {
        var ids = noticeIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        await _noticeService.DeleteAsync(ids);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    /// <summary>
    /// 获取弹出公告
    /// </summary>
    [HttpGet("popup")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<NoticeDto>>>> GetPopupNotices()
    {
        var notices = await _noticeService.GetPopupNoticesAsync();
        return ApiResponse<List<NoticeDto>>.Ok(notices).ToActionResult();
    }
}

