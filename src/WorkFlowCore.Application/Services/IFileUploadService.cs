using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 文件上传服务接口
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// 初始化上传（创建上传任务）
    /// </summary>
    Task<InitiateUploadResponse> InitiateUploadAsync(InitiateUploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传文件分片
    /// </summary>
    Task<UploadChunkResponse> UploadChunkAsync(UploadChunkRequest request, Stream chunkStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成上传（合并分片）
    /// </summary>
    Task<CompleteUploadResponse> CompleteUploadAsync(CompleteUploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询上传进度
    /// </summary>
    Task<UploadProgressResponse> GetUploadProgressAsync(long uploadId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消上传
    /// </summary>
    Task CancelUploadAsync(long uploadId, CancellationToken cancellationToken = default);
}

