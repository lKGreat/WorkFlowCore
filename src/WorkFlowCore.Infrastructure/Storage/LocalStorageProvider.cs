using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BlobStoring;
using WorkFlowCore.Domain.Storage;

namespace WorkFlowCore.Infrastructure.Storage;

/// <summary>
/// 基于 ABP BlobStoring 的存储提供者
/// </summary>
public class LocalStorageProvider : IStorageProvider
{
    private readonly IBlobContainer<FileStorageBlobContainer> _blobContainer;
    private readonly ILogger<LocalStorageProvider> _logger;

    public LocalStorageProvider(
        IBlobContainer<FileStorageBlobContainer> blobContainer,
        ILogger<LocalStorageProvider> logger)
    {
        _blobContainer = blobContainer;
        _logger = logger;
    }

    public async Task<string> UploadChunkAsync(Stream stream, string path, CancellationToken cancellationToken = default)
    {
        await _blobContainer.SaveAsync(path, stream, overrideExisting: true, cancellationToken);
        
        _logger.LogInformation("Uploaded chunk to blob storage: {Path}", path);
        
        return path;
    }

    public async Task<string> MergeChunksAsync(string[] chunkPaths, string finalPath, CancellationToken cancellationToken = default)
    {
        using var finalStream = new MemoryStream();
        
        foreach (var chunkPath in chunkPaths)
        {
            var chunkExists = await _blobContainer.ExistsAsync(chunkPath, cancellationToken);
            if (!chunkExists)
            {
                throw new FileNotFoundException($"Chunk file not found: {chunkPath}");
            }

            var chunkStream = await _blobContainer.GetAsync(chunkPath, cancellationToken);
            await chunkStream.CopyToAsync(finalStream, cancellationToken);
            await chunkStream.DisposeAsync();
        }

        // 保存合并后的文件
        finalStream.Position = 0;
        await _blobContainer.SaveAsync(finalPath, finalStream, overrideExisting: true, cancellationToken);

        // 删除分片文件
        foreach (var chunkPath in chunkPaths)
        {
            try
            {
                await _blobContainer.DeleteAsync(chunkPath, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete chunk file: {Path}", chunkPath);
            }
        }

        _logger.LogInformation("Merged {Count} chunks to: {Path}", chunkPaths.Length, finalPath);
        
        return finalPath;
    }

    public Task<string> GeneratePresignedUrlAsync(string path, int expireMinutes = 30, CancellationToken cancellationToken = default)
    {
        // 本地存储不支持真正的预签名URL，返回下载API路径
        return Task.FromResult($"/api/files/download/{Uri.EscapeDataString(path)}");
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        await _blobContainer.DeleteAsync(path, cancellationToken);
        _logger.LogInformation("Deleted file: {Path}", path);
    }

    public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        var exists = await _blobContainer.ExistsAsync(path, cancellationToken);
        if (!exists)
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        return await _blobContainer.GetAsync(path, cancellationToken);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return _blobContainer.ExistsAsync(path, cancellationToken);
    }

    public async Task<long> GetFileSizeAsync(string path, CancellationToken cancellationToken = default)
    {
        var exists = await _blobContainer.ExistsAsync(path, cancellationToken);
        if (!exists)
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        // ABP BlobStoring 没有直接获取文件大小的方法，需要读取流
        await using var stream = await _blobContainer.GetAsync(path, cancellationToken);
        return stream.Length;
    }
}

