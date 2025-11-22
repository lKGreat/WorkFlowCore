using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 文件上传下载应用服务
/// </summary>
public interface IFileStorageService
{
    Task<FileUploadInitResponseDto> InitializeUploadAsync(
        FileUploadInitRequestDto request,
        Guid tenantId,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task UploadChunkAsync(
        Guid tenantId,
        FileChunkUploadDto chunk,
        CancellationToken cancellationToken = default);

    Task<FileUploadCompleteResponseDto> CompleteUploadAsync(
        FileUploadCompleteRequestDto request,
        Guid tenantId,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<FileDownloadResultDto?> DownloadAsync(
        Guid fileId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<FileChunkDownloadResultDto?> DownloadChunkAsync(
        Guid fileId,
        Guid tenantId,
        int chunkNumber,
        int chunkSize,
        CancellationToken cancellationToken = default);
}

