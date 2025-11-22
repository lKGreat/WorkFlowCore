using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 文件上传下载控制器
/// </summary>
[Authorize]
public class FilesController : BaseController
{
    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// 初始化分片上传
    /// </summary>
    [HttpPost("uploads/init")]
    public async Task<ActionResult<ApiResponse<FileUploadInitResponseDto>>> InitializeUploadAsync(
        [FromBody] FileUploadInitRequestDto request,
        CancellationToken cancellationToken)
    {
        if (CurrentTenantId is null)
        {
            return ApiResponse<FileUploadInitResponseDto>.Fail("缺少租户上下文");
        }

        var result = await _fileStorageService.InitializeUploadAsync(
            request,
            CurrentTenantId.Value,
            CurrentUserId,
            cancellationToken);

        return ApiResponse<FileUploadInitResponseDto>.Ok(result);
    }

    /// <summary>
    /// 上传分片
    /// </summary>
    [HttpPost("uploads/{uploadId:guid}/chunks")]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<ActionResult<ApiResponse>> UploadChunkAsync(
        Guid uploadId,
        [FromForm] IFormFile chunk,
        [FromForm] int chunkNumber,
        CancellationToken cancellationToken)
    {
        if (CurrentTenantId is null)
        {
            return ApiResponse.Fail("缺少租户上下文");
        }

        if (chunk == null || chunk.Length == 0)
        {
            return ApiResponse.Fail("分片内容不能为空");
        }

        await using var chunkStream = chunk.OpenReadStream();
        var dto = new FileChunkUploadDto
        {
            UploadId = uploadId,
            ChunkNumber = chunkNumber,
            Content = chunkStream,
            Length = chunk.Length
        };

        await _fileStorageService.UploadChunkAsync(CurrentTenantId.Value, dto, cancellationToken);

        return ApiResponse.Ok("分片上传成功");
    }

    /// <summary>
    /// 完成分片上传
    /// </summary>
    [HttpPost("uploads/{uploadId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<FileUploadCompleteResponseDto>>> CompleteUploadAsync(
        Guid uploadId,
        [FromBody] FileUploadCompleteRequestDto request,
        CancellationToken cancellationToken)
    {
        if (CurrentTenantId is null)
        {
            return ApiResponse<FileUploadCompleteResponseDto>.Fail("缺少租户上下文");
        }

        request.UploadId = uploadId;

        var result = await _fileStorageService.CompleteUploadAsync(
            request,
            CurrentTenantId.Value,
            CurrentUserId,
            cancellationToken);

        return ApiResponse<FileUploadCompleteResponseDto>.Ok(result);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    [HttpGet("{fileId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadAsync(Guid fileId, CancellationToken cancellationToken)
    {
        if (CurrentTenantId is null)
        {
            return BadRequest("缺少租户上下文");
        }

        var download = await _fileStorageService.DownloadAsync(fileId, CurrentTenantId.Value, cancellationToken);
        if (download == null)
        {
            return NotFound();
        }

        return File(download.Content, download.ContentType, download.FileName);
    }

    /// <summary>
    /// 下载指定分片
    /// </summary>
    [HttpGet("{fileId:guid}/chunks")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadChunkAsync(
        Guid fileId,
        [FromQuery] int chunkNumber,
        [FromQuery] int chunkSize,
        CancellationToken cancellationToken)
    {
        if (CurrentTenantId is null)
        {
            return BadRequest("缺少租户上下文");
        }

        var chunk = await _fileStorageService.DownloadChunkAsync(
            fileId,
            CurrentTenantId.Value,
            chunkNumber,
            chunkSize,
            cancellationToken);

        if (chunk == null)
        {
            return NotFound();
        }

        Response.Headers["X-Chunk-Number"] = chunk.ChunkNumber.ToString();
        Response.Headers["X-Chunk-Size"] = chunk.ChunkSize.ToString();
        Response.Headers["X-Chunk-IsLast"] = chunk.IsLastChunk ? "true" : "false";
        Response.Headers["X-Chunk-TotalSize"] = chunk.TotalSize.ToString();

        return File(chunk.Content, chunk.ContentType);
    }
}

