using System;
using System.IO;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Infrastructure.FileStorage;

/// <summary>
/// 文件存储配置
/// </summary>
public class FileStorageOptions
{
    /// <summary>
    /// 默认存储提供器
    /// </summary>
    public string DefaultProvider { get; set; } = FileStorageProviders.Local;

    /// <summary>
    /// 分片临时目录
    /// </summary>
    public string TempDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "file-chunks");

    public ChunkOptions Chunk { get; set; } = new();

    public LocalStorageOptions Local { get; set; } = new();

    public AliyunStorageOptions Aliyun { get; set; } = new();

    public AwsStorageOptions Aws { get; set; } = new();
}

/// <summary>
/// 分片配置
/// </summary>
public class ChunkOptions
{
    public int MinChunkSizeInBytes { get; set; } = 256 * 1024;

    public int MaxChunkSizeInBytes { get; set; } = 8 * 1024 * 1024;

    public int ExpirationMinutes { get; set; } = 120;
}

/// <summary>
/// 本地存储配置
/// </summary>
public class LocalStorageOptions
{
    public string RootPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "files");

    public bool UseDateFolders { get; set; } = true;
}

/// <summary>
/// 阿里云 OSS 配置
/// </summary>
public class AliyunStorageOptions
{
    public string Endpoint { get; set; } = string.Empty;

    public string AccessKeyId { get; set; } = string.Empty;

    public string AccessKeySecret { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;
}

/// <summary>
/// AWS S3 配置
/// </summary>
public class AwsStorageOptions
{
    public string Region { get; set; } = string.Empty;

    public string AccessKeyId { get; set; } = string.Empty;

    public string AccessKeySecret { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;
}

