using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BlobStoring;
using WorkFlowCore.Domain.Storage;

namespace WorkFlowCore.Infrastructure.Storage;

/// <summary>
/// Azure Blob存储提供者（基于 ABP BlobStoring.Azure）
/// 配置在 AbpBlobStoringOptions 中，支持动态切换
/// </summary>
public class AzureBlobProvider : IStorageProvider
{
    private readonly IBlobContainer<FileStorageBlobContainer> _blobContainer;
    private readonly ILogger<AzureBlobProvider> _logger;

    public AzureBlobProvider(
        IBlobContainer<FileStorageBlobContainer> blobContainer,
        ILogger<AzureBlobProvider> logger)
    {
        _blobContainer = blobContainer;
        _logger = logger;
    }

    public Task<string> UploadChunkAsync(Stream stream, string path, CancellationToken cancellationToken = default)
    {
        return _blobContainer.SaveAsync(path, stream, overrideExisting: true, cancellationToken)
            .ContinueWith(_ => path, cancellationToken);
    }

    public async Task<string> MergeChunksAsync(string[] chunkPaths, string finalPath, CancellationToken cancellationToken = default)
    {
        using var finalStream = new MemoryStream();
        
        foreach (var chunkPath in chunkPaths)
        {
            var chunkStream = await _blobContainer.GetAsync(chunkPath, cancellationToken);
            await chunkStream.CopyToAsync(finalStream, cancellationToken);
            await chunkStream.DisposeAsync();
        }

        finalStream.Position = 0;
        await _blobContainer.SaveAsync(finalPath, finalStream, overrideExisting: true, cancellationToken);

        foreach (var chunkPath in chunkPaths)
        {
            await _blobContainer.DeleteAsync(chunkPath, cancellationToken);
        }

        return finalPath;
    }

    public Task<string> GeneratePresignedUrlAsync(string path, int expireMinutes = 30, CancellationToken cancellationToken = default)
    {
        // ABP BlobStoring 的 Azure 提供者支持预签名 URL，这里简化返回
        return Task.FromResult($"/api/files/download/{Uri.EscapeDataString(path)}");
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        return _blobContainer.DeleteAsync(path, cancellationToken);
    }

    public Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        return _blobContainer.GetAsync(path, cancellationToken);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return _blobContainer.ExistsAsync(path, cancellationToken);
    }

    public async Task<long> GetFileSizeAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = await _blobContainer.GetAsync(path, cancellationToken);
        return stream.Length;
    }
}

