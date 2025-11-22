using System;
using System.Collections.Generic;
using System.IO;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 初始化分片上传请求
/// </summary>
public class FileUploadInitRequestDto
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/octet-stream";

    public long TotalSize { get; set; }

    public int ChunkSize { get; set; }

    public string? StorageProvider { get; set; }

    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// 初始化分片上传响应
/// </summary>
public class FileUploadInitResponseDto
{
    public Guid UploadId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int ChunkSize { get; set; }

    public int TotalChunks { get; set; }

    public long TotalSize { get; set; }

    public string StorageProvider { get; set; } = string.Empty;
}

/// <summary>
/// 单个分片上传数据
/// </summary>
public class FileChunkUploadDto
{
    public Guid UploadId { get; set; }

    public int ChunkNumber { get; set; }

    public Stream Content { get; set; } = Stream.Null;

    public long Length { get; set; }

    public string? Checksum { get; set; }
}

/// <summary>
/// 完成上传请求
/// </summary>
public class FileUploadCompleteRequestDto
{
    public Guid UploadId { get; set; }

    public string? Hash { get; set; }

    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// 上传完成响应
/// </summary>
public class FileUploadCompleteResponseDto
{
    public Guid FileId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string StorageProvider { get; set; } = string.Empty;

    public string StorageKey { get; set; } = string.Empty;

    public long Size { get; set; }

    public string Hash { get; set; } = string.Empty;

    public int TotalChunks { get; set; }
}

/// <summary>
/// 文件元数据
/// </summary>
public class FileMetadataDto
{
    public Guid FileId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/octet-stream";

    public long Size { get; set; }

    public string StorageProvider { get; set; } = string.Empty;

    public string StorageKey { get; set; } = string.Empty;

    public string Hash { get; set; } = string.Empty;

    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// 文件下载结果
/// </summary>
public class FileDownloadResultDto
{
    public Stream Content { get; set; } = Stream.Null;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/octet-stream";

    public long Size { get; set; }

    public string StorageProvider { get; set; } = string.Empty;
}

/// <summary>
/// 文件分片下载结果
/// </summary>
public class FileChunkDownloadResultDto
{
    public Stream Content { get; set; } = Stream.Null;

    public int ChunkNumber { get; set; }

    public int ChunkSize { get; set; }

    public long TotalSize { get; set; }

    public bool IsLastChunk { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/octet-stream";
}

