using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BlobStoring;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Data;
using WorkFlowCore.Infrastructure.FileStorage;
using WorkFlowCore.Infrastructure.FileStorage.Containers;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 文件存储服务实现
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly WorkFlowDbContext _dbContext;
    private readonly IBlobContainer<LocalFileBlobContainer> _localContainer;
    private readonly IBlobContainer<AliyunFileBlobContainer> _aliyunContainer;
    private readonly IBlobContainer<AwsFileBlobContainer> _awsContainer;
    private readonly FileStorageOptions _options;
    private readonly ILogger<FileStorageService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public FileStorageService(
        WorkFlowDbContext dbContext,
        IBlobContainer<LocalFileBlobContainer> localContainer,
        IBlobContainer<AliyunFileBlobContainer> aliyunContainer,
        IBlobContainer<AwsFileBlobContainer> awsContainer,
        IOptions<FileStorageOptions> options,
        ILogger<FileStorageService> logger)
    {
        _dbContext = dbContext;
        _localContainer = localContainer;
        _aliyunContainer = aliyunContainer;
        _awsContainer = awsContainer;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<FileUploadInitResponseDto> InitializeUploadAsync(
        FileUploadInitRequestDto request,
        Guid tenantId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("租户ID不能为空", nameof(tenantId));
        }

        if (request.TotalSize <= 0)
        {
            throw new ArgumentException("文件大小必须大于0", nameof(request.TotalSize));
        }

        var provider = NormalizeProvider(request.StorageProvider);
        ValidateProvider(provider);

        var chunkSize = NormalizeChunkSize(request, (int)Math.Min(request.TotalSize, int.MaxValue));
        var totalChunks = (int)Math.Ceiling((double)request.TotalSize / chunkSize);
        var tempDirectory = BuildTempDirectory(tenantId);
        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? $"upload-{Guid.NewGuid():N}"
            : request.FileName.Trim();
        var contentType = string.IsNullOrWhiteSpace(request.ContentType)
            ? "application/octet-stream"
            : request.ContentType;

        Directory.CreateDirectory(tempDirectory);

        var session = new FileUploadSession
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CreatedBy = userId,
            FileName = fileName,
            ContentType = contentType,
            TotalSize = request.TotalSize,
            ChunkSize = chunkSize,
            TotalChunks = totalChunks,
            UploadedChunks = 0,
            StorageProvider = provider,
            TempDirectory = tempDirectory,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_options.Chunk.ExpirationMinutes),
            IsCompleted = false,
            Metadata = SerializeMetadata(request.Metadata)
        };

        await _dbContext.FileUploadSessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new FileUploadInitResponseDto
        {
            UploadId = session.Id,
            FileName = session.FileName,
            ChunkSize = session.ChunkSize,
            TotalChunks = session.TotalChunks,
            TotalSize = session.TotalSize,
            StorageProvider = session.StorageProvider
        };
    }

    public async Task UploadChunkAsync(Guid tenantId, FileChunkUploadDto chunk, CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.FileUploadSessions
            .FirstOrDefaultAsync(x => x.Id == chunk.UploadId && x.TenantId == tenantId, cancellationToken);

        if (session == null)
        {
            throw new InvalidOperationException("上传会话不存在");
        }

        ValidateSessionState(session);
        ValidateChunkNumber(session, chunk.ChunkNumber);

        if (chunk.Length <= 0)
        {
            throw new ArgumentException("分片内容为空", nameof(chunk.Length));
        }

        var expectedLength = CalculateExpectedChunkSize(session, chunk.ChunkNumber);
        if (chunk.Length > expectedLength)
        {
            throw new InvalidOperationException("分片大小超出限制");
        }

        Directory.CreateDirectory(session.TempDirectory);
        var chunkPath = GetChunkPath(session.TempDirectory, chunk.ChunkNumber);
        var isNewChunk = !File.Exists(chunkPath);

        await using (var fileStream = File.Open(chunkPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await chunk.Content.CopyToAsync(fileStream, cancellationToken);
        }

        if (isNewChunk)
        {
            session.UploadedChunks = Math.Min(session.UploadedChunks + 1, session.TotalChunks);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<FileUploadCompleteResponseDto> CompleteUploadAsync(
        FileUploadCompleteRequestDto request,
        Guid tenantId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.FileUploadSessions
            .FirstOrDefaultAsync(x => x.Id == request.UploadId && x.TenantId == tenantId, cancellationToken);

        if (session == null)
        {
            throw new InvalidOperationException("上传会话不存在");
        }

        ValidateSessionState(session);

        if (session.UploadedChunks < session.TotalChunks)
        {
            throw new InvalidOperationException("分片未全部上传");
        }

        var mergedFilePath = await MergeChunksAsync(session, cancellationToken);
        await using var mergedStream = File.OpenRead(mergedFilePath);
        var hash = await ComputeHashAsync(mergedStream, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Hash) &&
            !hash.Equals(request.Hash, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("文件哈希校验失败");
        }

        mergedStream.Position = 0;
        var finalSize = mergedStream.Length;
        var providerType = FileStorageProviders.ToEnum(session.StorageProvider);
        var container = ResolveContainer(providerType);
        var storageKey = BuildStorageKey(tenantId, session.FileName);

        await container.SaveAsync(storageKey, mergedStream, true, cancellationToken);

        var combinedMetadata = MergeMetadata(session.Metadata, request.Metadata);

        var resource = new FileResource
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UploadedBy = userId,
            FileName = session.FileName,
            ContentType = session.ContentType,
            Size = finalSize,
            StorageProvider = session.StorageProvider,
            StorageKey = storageKey,
            Hash = hash,
            Metadata = combinedMetadata
        };

        session.IsCompleted = true;
        session.FileResourceId = resource.Id;
        session.Metadata = combinedMetadata;
        session.TotalSize = finalSize;

        await _dbContext.FileResources.AddAsync(resource, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        CleanupTempDirectory(session);

        return new FileUploadCompleteResponseDto
        {
            FileId = resource.Id,
            FileName = resource.FileName,
            StorageProvider = resource.StorageProvider,
            StorageKey = resource.StorageKey,
            Size = resource.Size,
            Hash = resource.Hash,
            TotalChunks = session.TotalChunks
        };
    }

    public async Task<FileDownloadResultDto?> DownloadAsync(
        Guid fileId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.FileResources
            .FirstOrDefaultAsync(x => x.Id == fileId && x.TenantId == tenantId, cancellationToken);

        if (file == null)
        {
            return null;
        }

        var providerType = FileStorageProviders.ToEnum(file.StorageProvider);
        var container = ResolveContainer(providerType);
        var stream = await container.GetAsync(file.StorageKey, cancellationToken);

        return new FileDownloadResultDto
        {
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Size,
            StorageProvider = file.StorageProvider
        };
    }

    public async Task<FileChunkDownloadResultDto?> DownloadChunkAsync(
        Guid fileId,
        Guid tenantId,
        int chunkNumber,
        int chunkSize,
        CancellationToken cancellationToken = default)
    {
        if (chunkSize <= 0)
        {
            throw new ArgumentException("chunkSize 必须大于0", nameof(chunkSize));
        }

        var file = await _dbContext.FileResources
            .FirstOrDefaultAsync(x => x.Id == fileId && x.TenantId == tenantId, cancellationToken);

        if (file == null)
        {
            return null;
        }

        var providerType = FileStorageProviders.ToEnum(file.StorageProvider);
        var container = ResolveContainer(providerType);
        var totalChunks = (int)Math.Ceiling((double)file.Size / chunkSize);

        if (chunkNumber < 0 || chunkNumber >= totalChunks)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkNumber), "分片索引超出范围");
        }

        var offset = (long)chunkNumber * chunkSize;
        var stream = await container.GetAsync(file.StorageKey, cancellationToken);
        await SkipAsync(stream, offset, cancellationToken);

        var memory = new MemoryStream();
        var buffer = new byte[Math.Min(chunkSize, 1024 * 64)];
        var remaining = chunkSize;

        while (remaining > 0)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, remaining)), cancellationToken);
            if (read == 0)
            {
                break;
            }

            await memory.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            remaining -= read;
        }

        memory.Position = 0;
        await stream.DisposeAsync();

        return new FileChunkDownloadResultDto
        {
            Content = memory,
            ChunkNumber = chunkNumber,
            ChunkSize = (int)memory.Length,
            TotalSize = file.Size,
            IsLastChunk = offset + memory.Length >= file.Size,
            FileName = file.FileName,
            ContentType = file.ContentType
        };
    }

    private string NormalizeProvider(string? requestedProvider)
    {
        var provider = string.IsNullOrWhiteSpace(requestedProvider)
            ? _options.DefaultProvider
            : requestedProvider.Trim();

        if (provider.Equals(FileStorageProviders.Aliyun, StringComparison.OrdinalIgnoreCase))
        {
            return FileStorageProviders.Aliyun;
        }

        if (provider.Equals(FileStorageProviders.Aws, StringComparison.OrdinalIgnoreCase))
        {
            return FileStorageProviders.Aws;
        }

        if (provider.Equals(FileStorageProviders.Local, StringComparison.OrdinalIgnoreCase))
        {
            return FileStorageProviders.Local;
        }

        return provider;
    }

    private void ValidateProvider(string provider)
    {
        if (!FileStorageProviders.IsValid(provider))
        {
            throw new ArgumentOutOfRangeException(nameof(provider), provider, "不支持的存储提供器");
        }
    }

    private int NormalizeChunkSize(FileUploadInitRequestDto request, int maxPossible)
    {
        var chunkSize = request.ChunkSize > 0 ? request.ChunkSize : _options.Chunk.MaxChunkSizeInBytes;
        chunkSize = Math.Clamp(chunkSize, _options.Chunk.MinChunkSizeInBytes, _options.Chunk.MaxChunkSizeInBytes);
        chunkSize = Math.Min(chunkSize, maxPossible);
        return chunkSize;
    }

    private string BuildTempDirectory(Guid tenantId)
    {
        var basePath = string.IsNullOrWhiteSpace(_options.TempDirectory)
            ? Path.Combine(Path.GetTempPath(), "workflow-chunks")
            : Path.IsPathRooted(_options.TempDirectory)
                ? _options.TempDirectory
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _options.TempDirectory));

        return Path.Combine(basePath, tenantId.ToString("N"), Guid.NewGuid().ToString("N"));
    }

    private static string GetChunkPath(string tempDirectory, int chunkNumber)
    {
        return Path.Combine(tempDirectory, $"{chunkNumber:D6}.chunk");
    }

    private static long CalculateExpectedChunkSize(FileUploadSession session, int chunkNumber)
    {
        if (chunkNumber < session.TotalChunks - 1)
        {
            return session.ChunkSize;
        }

        var lastChunkSize = session.TotalSize - (long)session.ChunkSize * (session.TotalChunks - 1);
        return lastChunkSize == 0 ? session.ChunkSize : lastChunkSize;
    }

    private void ValidateSessionState(FileUploadSession session)
    {
        if (session.IsCompleted)
        {
            throw new InvalidOperationException("上传会话已完成");
        }

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("上传会话已过期");
        }
    }

    private static void ValidateChunkNumber(FileUploadSession session, int chunkNumber)
    {
        if (chunkNumber < 0 || chunkNumber >= session.TotalChunks)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkNumber), "分片索引超出范围");
        }
    }

    private async Task<string> MergeChunksAsync(FileUploadSession session, CancellationToken cancellationToken)
    {
        var mergedFilePath = Path.Combine(session.TempDirectory, $"merged-{Guid.NewGuid():N}.tmp");
        Directory.CreateDirectory(session.TempDirectory);

        var chunkFiles = Directory.Exists(session.TempDirectory)
            ? Directory.EnumerateFiles(session.TempDirectory, "*.chunk")
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList()
            : new List<string>();

        if (chunkFiles.Count != session.TotalChunks)
        {
            throw new InvalidOperationException("分片数量与期望不一致");
        }

        await using var output = File.Open(mergedFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

        foreach (var chunkFile in chunkFiles)
        {
            await using var input = File.OpenRead(chunkFile);
            await input.CopyToAsync(output, cancellationToken);
        }

        await output.FlushAsync(cancellationToken);
        return mergedFilePath;
    }

    private static async Task<string> ComputeHashAsync(Stream stream, CancellationToken cancellationToken)
    {
        stream.Position = 0;
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }

    private IBlobContainer ResolveContainer(FileStorageProviderType providerType)
    {
        return providerType switch
        {
            FileStorageProviderType.Local => _localContainer,
            FileStorageProviderType.Aliyun => _aliyunContainer,
            FileStorageProviderType.Aws => _awsContainer,
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, "未知的存储提供器")
        };
    }

    private static string BuildStorageKey(Guid tenantId, string fileName)
    {
        var sanitizedName = string.IsNullOrWhiteSpace(fileName) ? "file.bin" : fileName.Trim();
        var safeName = sanitizedName.Replace(" ", "_");
        return $"{tenantId:D}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}-{safeName}";
    }

    private string? SerializeMetadata(Dictionary<string, string>? metadata)
    {
        if (metadata == null || metadata.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(metadata, _jsonOptions);
    }

    private string? MergeMetadata(string? originalJson, Dictionary<string, string>? appendMetadata)
    {
        Dictionary<string, string> result;

        if (string.IsNullOrWhiteSpace(originalJson))
        {
            result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            result = JsonSerializer.Deserialize<Dictionary<string, string>>(originalJson, _jsonOptions)
                     ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        if (appendMetadata != null)
        {
            foreach (var (key, value) in appendMetadata)
            {
                result[key] = value;
            }
        }

        return result.Count == 0 ? null : JsonSerializer.Serialize(result, _jsonOptions);
    }

    private static async Task SkipAsync(Stream stream, long bytes, CancellationToken cancellationToken)
    {
        if (bytes <= 0)
        {
            return;
        }

        if (stream.CanSeek)
        {
            stream.Seek(bytes, SeekOrigin.Begin);
            return;
        }

        var buffer = new byte[81920];
        long remaining = bytes;

        while (remaining > 0)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, remaining)), cancellationToken);
            if (read == 0)
            {
                break;
            }

            remaining -= read;
        }
    }

    private void CleanupTempDirectory(FileUploadSession session)
    {
        try
        {
            if (Directory.Exists(session.TempDirectory))
            {
                Directory.Delete(session.TempDirectory, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "清理分片临时目录失败：{Directory}", session.TempDirectory);
        }
    }
}

