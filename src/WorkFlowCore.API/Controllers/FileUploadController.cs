using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 文件上传控制器
/// </summary>
[ApiController]
[Route("api/files")]
[Authorize]
public class FileUploadController : AbpControllerBase
{
    private readonly IFileUploadService _uploadService;

    public FileUploadController(IFileUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    /// <summary>
    /// 初始化上传
    /// </summary>
    [HttpPost("initiate-upload")]
    public Task<InitiateUploadResponse> InitiateUpload([FromBody] InitiateUploadRequest request, CancellationToken cancellationToken)
    {
        return _uploadService.InitiateUploadAsync(request, cancellationToken);
    }

    /// <summary>
    /// 上传分片
    /// </summary>
    [HttpPost("upload-chunk")]
    [RequestSizeLimit(10485760)] // 10MB
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    public async Task<UploadChunkResponse> UploadChunk(
        [FromForm] long uploadId,
        [FromForm] int chunkIndex,
        [FromForm] string chunkHash,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required");
        }

        var request = new UploadChunkRequest
        {
            UploadId = uploadId,
            ChunkIndex = chunkIndex,
            ChunkHash = chunkHash
        };

        await using var stream = file.OpenReadStream();
        return await _uploadService.UploadChunkAsync(request, stream, cancellationToken);
    }

    /// <summary>
    /// 完成上传
    /// </summary>
    [HttpPost("complete-upload")]
    public Task<CompleteUploadResponse> CompleteUpload([FromBody] CompleteUploadRequest request, CancellationToken cancellationToken)
    {
        return _uploadService.CompleteUploadAsync(request, cancellationToken);
    }

    /// <summary>
    /// 查询上传进度
    /// </summary>
    [HttpGet("upload-progress/{uploadId}")]
    public Task<UploadProgressResponse> GetUploadProgress(long uploadId, CancellationToken cancellationToken)
    {
        return _uploadService.GetUploadProgressAsync(uploadId, cancellationToken);
    }

    /// <summary>
    /// 取消上传
    /// </summary>
    [HttpPost("cancel-upload/{uploadId}")]
    public Task CancelUpload(long uploadId, CancellationToken cancellationToken)
    {
        return _uploadService.CancelUploadAsync(uploadId, cancellationToken);
    }
}

