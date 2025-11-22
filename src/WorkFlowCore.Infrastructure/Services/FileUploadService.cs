using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Storage;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 文件上传服务实现
/// </summary>
public class FileUploadService : ApplicationService, IFileUploadService
{
    private readonly IRepository<FileAttachment, long> _attachmentRepository;
    private readonly IRepository<FileChunk, long> _chunkRepository;
    private readonly IRepository<FileStorageProvider, long> _providerRepository;
    private readonly StorageProviderFactory _providerFactory;
    private readonly IConfiguration _configuration;

    private long ChunkSize => _configuration.GetValue<long>("FileUpload:ChunkSize", 5242880); // 5MB
    private long MaxFileSize => _configuration.GetValue<long>("FileUpload:MaxFileSize", 5368709120); // 5GB

    public FileUploadService(
        IRepository<FileAttachment, long> attachmentRepository,
        IRepository<FileChunk, long> chunkRepository,
        IRepository<FileStorageProvider, long> providerRepository,
        StorageProviderFactory providerFactory,
        IConfiguration configuration)
    {
        _attachmentRepository = attachmentRepository;
        _chunkRepository = chunkRepository;
        _providerRepository = providerRepository;
        _providerFactory = providerFactory;
        _configuration = configuration;
    }

    public async Task<InitiateUploadResponse> InitiateUploadAsync(InitiateUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FileSize > MaxFileSize)
        {
            throw new BusinessException($"File size exceeds maximum allowed size: {MaxFileSize} bytes");
        }

        // 检查是否已存在相同MD5的文件（秒传）
        var existingFile = await _attachmentRepository.FindAsync(
            a => a.Md5Hash == request.Md5Hash && a.UploadStatus == UploadStatus.Completed,
            cancellationToken: cancellationToken);

        if (existingFile != null)
        {
            Logger.LogInformation("File with MD5 {Md5Hash} already exists, instant upload", request.Md5Hash);
            return new InitiateUploadResponse
            {
                UploadId = existingFile.Id,
                ChunkSize = ChunkSize,
                TotalChunks = 0,
                CanInstantUpload = true
            };
        }

        // 获取存储提供者
        long providerId;
        if (request.StorageProviderId.HasValue)
        {
            providerId = request.StorageProviderId.Value;
        }
        else
        {
            var defaultProvider = await _providerRepository.FirstOrDefaultAsync(
                p => p.IsEnabled,
                cancellationToken: cancellationToken);
            
            if (defaultProvider == null)
            {
                throw new BusinessException("No enabled storage provider found");
            }
            providerId = defaultProvider.Id;
        }

        // 生成雪花ID
        var attachmentId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // 创建附件记录
        var attachment = new FileAttachment(
            attachmentId,
            CurrentTenant.Id,
            request.OriginalFileName,
            request.FileSize,
            request.ContentType,
            request.Md5Hash,
            request.BusinessType,
            request.BusinessId,
            providerId)
        {
            FileName = $"{Guid.NewGuid()}{Path.GetExtension(request.OriginalFileName)}",
            StoragePath = GenerateStoragePath(request.BusinessType, request.OriginalFileName),
            UploadStatus = UploadStatus.Uploading
        };

        // 计算分片信息
        var totalChunks = (int)Math.Ceiling((double)request.FileSize / ChunkSize);
        attachment.TotalChunks = totalChunks;
        attachment.UploadedChunks = 0;

        await _attachmentRepository.InsertAsync(attachment, cancellationToken: cancellationToken);

        // 创建分片记录
        for (int i = 0; i < totalChunks; i++)
        {
            var chunkId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + i;
            var chunkSize = i == totalChunks - 1
                ? request.FileSize - (i * ChunkSize)
                : ChunkSize;

            var chunk = new FileChunk(chunkId, attachmentId, i, chunkSize);
            await _chunkRepository.InsertAsync(chunk, autoSave: false, cancellationToken: cancellationToken);
        }

        await UnitOfWorkManager.Current!.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Initiated upload for file: {FileName}, UploadId: {UploadId}", 
            request.OriginalFileName, attachmentId);

        return new InitiateUploadResponse
        {
            UploadId = attachmentId,
            ChunkSize = ChunkSize,
            TotalChunks = totalChunks,
            UploadedChunkIndexes = Array.Empty<int>(),
            CanInstantUpload = false
        };
    }

    public async Task<UploadChunkResponse> UploadChunkAsync(UploadChunkRequest request, Stream chunkStream, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(request.UploadId, cancellationToken: cancellationToken);
        
        if (attachment.UploadStatus != UploadStatus.Uploading)
        {
            throw new BusinessException($"Upload is not in uploading status: {attachment.UploadStatus}");
        }

        var chunk = await _chunkRepository.FirstOrDefaultAsync(
            c => c.AttachmentId == request.UploadId && c.ChunkIndex == request.ChunkIndex,
            cancellationToken: cancellationToken);

        if (chunk == null)
        {
            throw new BusinessException($"Chunk not found: {request.ChunkIndex}");
        }

        if (chunk.UploadStatus == ChunkUploadStatus.Completed)
        {
            return new UploadChunkResponse
            {
                Success = true,
                ChunkIndex = request.ChunkIndex,
                Message = "Chunk already uploaded"
            };
        }

        // 获取存储提供者
        var provider = await _providerRepository.GetAsync(attachment.StorageProviderId, cancellationToken: cancellationToken);
        var storageProvider = _providerFactory.CreateProvider(provider);

        // 上传分片
        var chunkPath = $"{attachment.StoragePath}.chunk{request.ChunkIndex}";
        chunk.StoragePath = await storageProvider.UploadChunkAsync(chunkStream, chunkPath, cancellationToken);
        chunk.ChunkHash = request.ChunkHash;
        chunk.UploadStatus = ChunkUploadStatus.Completed;

        await _chunkRepository.UpdateAsync(chunk, cancellationToken: cancellationToken);

        // 更新附件上传进度
        attachment.UploadedChunks++;
        await _attachmentRepository.UpdateAsync(attachment, cancellationToken: cancellationToken);

        Logger.LogInformation("Uploaded chunk {ChunkIndex}/{TotalChunks} for UploadId: {UploadId}",
            request.ChunkIndex, attachment.TotalChunks, request.UploadId);

        return new UploadChunkResponse
        {
            Success = true,
            ChunkIndex = request.ChunkIndex,
            Message = "Chunk uploaded successfully"
        };
    }

    public async Task<CompleteUploadResponse> CompleteUploadAsync(CompleteUploadRequest request, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(request.UploadId, cancellationToken: cancellationToken);

        if (attachment.UploadStatus != UploadStatus.Uploading)
        {
            throw new BusinessException($"Upload is not in uploading status: {attachment.UploadStatus}");
        }

        // 检查所有分片是否已上传
        var chunks = await _chunkRepository.GetListAsync(
            c => c.AttachmentId == request.UploadId,
            cancellationToken: cancellationToken);

        var allCompleted = chunks.All(c => c.UploadStatus == ChunkUploadStatus.Completed);
        if (!allCompleted)
        {
            throw new BusinessException("Not all chunks have been uploaded");
        }

        // 获取存储提供者并合并分片
        var provider = await _providerRepository.GetAsync(attachment.StorageProviderId, cancellationToken: cancellationToken);
        var storageProvider = _providerFactory.CreateProvider(provider);

        var chunkPaths = chunks.OrderBy(c => c.ChunkIndex)
                               .Select(c => c.StoragePath)
                               .ToArray();

        attachment.StoragePath = await storageProvider.MergeChunksAsync(chunkPaths, attachment.StoragePath, cancellationToken);
        attachment.UploadStatus = UploadStatus.Completed;

        await _attachmentRepository.UpdateAsync(attachment, cancellationToken: cancellationToken);

        Logger.LogInformation("Completed upload for UploadId: {UploadId}, File: {FileName}",
            request.UploadId, attachment.OriginalFileName);

        // 生成访问URL（如果支持）
        string? accessUrl = null;
        try
        {
            accessUrl = await storageProvider.GeneratePresignedUrlAsync(attachment.StoragePath, 30, cancellationToken);
        }
        catch
        {
            // 忽略错误，某些提供者可能不支持预签名URL
        }

        return new CompleteUploadResponse
        {
            AttachmentId = attachment.Id,
            FileName = attachment.OriginalFileName,
            FileSize = attachment.FileSize,
            AccessUrl = accessUrl
        };
    }

    public async Task<UploadProgressResponse> GetUploadProgressAsync(long uploadId, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(uploadId, cancellationToken: cancellationToken);

        var chunks = await _chunkRepository.GetListAsync(
            c => c.AttachmentId == uploadId && c.UploadStatus == ChunkUploadStatus.Completed,
            cancellationToken: cancellationToken);

        var uploadedIndexes = chunks.Select(c => c.ChunkIndex).ToArray();

        return new UploadProgressResponse
        {
            UploadId = uploadId,
            TotalChunks = attachment.TotalChunks,
            UploadedChunks = attachment.UploadedChunks,
            ProgressPercentage = attachment.TotalChunks > 0 
                ? (double)attachment.UploadedChunks / attachment.TotalChunks * 100 
                : 0,
            Status = attachment.UploadStatus.ToString(),
            UploadedChunkIndexes = uploadedIndexes
        };
    }

    public async Task CancelUploadAsync(long uploadId, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(uploadId, cancellationToken: cancellationToken);

        if (attachment.UploadStatus != UploadStatus.Uploading)
        {
            return;
        }

        attachment.UploadStatus = UploadStatus.Cancelled;
        await _attachmentRepository.UpdateAsync(attachment, cancellationToken: cancellationToken);

        // 删除已上传的分片
        var chunks = await _chunkRepository.GetListAsync(
            c => c.AttachmentId == uploadId && c.UploadStatus == ChunkUploadStatus.Completed,
            cancellationToken: cancellationToken);

        if (chunks.Any())
        {
            var provider = await _providerRepository.GetAsync(attachment.StorageProviderId, cancellationToken: cancellationToken);
            var storageProvider = _providerFactory.CreateProvider(provider);

            foreach (var chunk in chunks)
            {
                try
                {
                    await storageProvider.DeleteAsync(chunk.StoragePath, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to delete chunk: {ChunkPath}", chunk.StoragePath);
                }
            }
        }

        Logger.LogInformation("Cancelled upload: {UploadId}", uploadId);
    }

    private string GenerateStoragePath(string businessType, string originalFileName)
    {
        var tenantId = CurrentTenant.Id?.ToString() ?? "default";
        var date = DateTime.UtcNow;
        var extension = Path.GetExtension(originalFileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        
        return $"{tenantId}/{businessType}/{date:yyyy-MM}/{fileName}";
    }
}

