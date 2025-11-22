using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 文件访问控制器
/// </summary>
[ApiController]
[Route("api/files")]
[Authorize]
public class FileAccessController : AbpControllerBase
{
    private readonly IFileAccessService _accessService;

    public FileAccessController(IFileAccessService accessService)
    {
        _accessService = accessService;
    }

    /// <summary>
    /// 获取附件信息
    /// </summary>
    [HttpGet("{attachmentId}")]
    public Task<FileAttachmentDto?> GetAttachment(long attachmentId, CancellationToken cancellationToken)
    {
        return _accessService.GetAttachmentAsync(attachmentId, cancellationToken);
    }

    /// <summary>
    /// 根据业务类型和ID获取附件列表
    /// </summary>
    [HttpGet("by-business")]
    public Task<List<FileAttachmentDto>> GetAttachmentsByBusiness(
        [FromQuery] string businessType,
        [FromQuery] string businessId,
        CancellationToken cancellationToken)
    {
        return _accessService.GetAttachmentsByBusinessAsync(businessType, businessId, cancellationToken);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    [HttpGet("{attachmentId}/download")]
    public async Task<IActionResult> DownloadFile(
        long attachmentId,
        [FromQuery] bool directUrl = false,
        [FromQuery] int expireMinutes = 30,
        CancellationToken cancellationToken = default)
    {
        var request = new DownloadFileRequest
        {
            AttachmentId = attachmentId,
            DirectUrl = directUrl,
            ExpireMinutes = expireMinutes
        };

        var response = await _accessService.DownloadFileAsync(request, cancellationToken);

        if (directUrl && !string.IsNullOrEmpty(response.Url))
        {
            return Ok(new { url = response.Url });
        }

        if (response.FileStream == null)
        {
            return NotFound();
        }

        return File(response.FileStream, response.ContentType, response.FileName);
    }

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    [HttpGet("{attachmentId}/access-token")]
    public Task<GenerateAccessTokenResponse> GenerateAccessToken(
        long attachmentId,
        [FromQuery] int expireMinutes = 30,
        CancellationToken cancellationToken = default)
    {
        return _accessService.GenerateAccessTokenAsync(attachmentId, expireMinutes, cancellationToken);
    }

    /// <summary>
    /// 删除附件
    /// </summary>
    [HttpDelete("{attachmentId}")]
    public Task DeleteAttachment(long attachmentId, CancellationToken cancellationToken)
    {
        return _accessService.DeleteAttachmentAsync(attachmentId, cancellationToken);
    }
}

