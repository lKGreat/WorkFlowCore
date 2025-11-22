using System;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 初始化上传请求
/// </summary>
public class InitiateUploadRequest
{
    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 内容类型
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件MD5哈希值
    /// </summary>
    public string Md5Hash { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// 业务单据ID
    /// </summary>
    public string BusinessId { get; set; } = string.Empty;

    /// <summary>
    /// 存储提供者ID（可选，不指定则使用默认）
    /// </summary>
    public long? StorageProviderId { get; set; }
}

/// <summary>
/// 初始化上传响应
/// </summary>
public class InitiateUploadResponse
{
    /// <summary>
    /// 上传ID（附件ID）
    /// </summary>
    public long UploadId { get; set; }

    /// <summary>
    /// 分片大小（字节）
    /// </summary>
    public long ChunkSize { get; set; }

    /// <summary>
    /// 总分片数
    /// </summary>
    public int TotalChunks { get; set; }

    /// <summary>
    /// 已上传的分片索引列表
    /// </summary>
    public int[] UploadedChunkIndexes { get; set; } = Array.Empty<int>();

    /// <summary>
    /// 是否可以秒传（文件已存在）
    /// </summary>
    public bool CanInstantUpload { get; set; }
}

/// <summary>
/// 上传分片请求
/// </summary>
public class UploadChunkRequest
{
    /// <summary>
    /// 上传ID（附件ID）
    /// </summary>
    public long UploadId { get; set; }

    /// <summary>
    /// 分片序号（从0开始）
    /// </summary>
    public int ChunkIndex { get; set; }

    /// <summary>
    /// 分片哈希值
    /// </summary>
    public string ChunkHash { get; set; } = string.Empty;
}

/// <summary>
/// 上传分片响应
/// </summary>
public class UploadChunkResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 分片序号
    /// </summary>
    public int ChunkIndex { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// 完成上传请求
/// </summary>
public class CompleteUploadRequest
{
    /// <summary>
    /// 上传ID（附件ID）
    /// </summary>
    public long UploadId { get; set; }
}

/// <summary>
/// 完成上传响应
/// </summary>
public class CompleteUploadResponse
{
    /// <summary>
    /// 附件ID
    /// </summary>
    public long AttachmentId { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 访问URL（如果支持直接访问）
    /// </summary>
    public string? AccessUrl { get; set; }
}

/// <summary>
/// 上传进度响应
/// </summary>
public class UploadProgressResponse
{
    /// <summary>
    /// 上传ID
    /// </summary>
    public long UploadId { get; set; }

    /// <summary>
    /// 总分片数
    /// </summary>
    public int TotalChunks { get; set; }

    /// <summary>
    /// 已上传分片数
    /// </summary>
    public int UploadedChunks { get; set; }

    /// <summary>
    /// 上传进度百分比
    /// </summary>
    public double ProgressPercentage { get; set; }

    /// <summary>
    /// 上传状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 已上传的分片索引列表
    /// </summary>
    public int[] UploadedChunkIndexes { get; set; } = Array.Empty<int>();
}

/// <summary>
/// 生成访问令牌响应
/// </summary>
public class GenerateAccessTokenResponse
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime ExpireAt { get; set; }

    /// <summary>
    /// 访问URL
    /// </summary>
    public string AccessUrl { get; set; } = string.Empty;
}

/// <summary>
/// 下载文件请求
/// </summary>
public class DownloadFileRequest
{
    /// <summary>
    /// 附件ID
    /// </summary>
    public long AttachmentId { get; set; }

    /// <summary>
    /// 是否直接返回URL（false则代理下载）
    /// </summary>
    public bool DirectUrl { get; set; } = false;

    /// <summary>
    /// URL过期时间（分钟，仅在DirectUrl=true时有效）
    /// </summary>
    public int ExpireMinutes { get; set; } = 30;
}

/// <summary>
/// 下载文件响应
/// </summary>
public class DownloadFileResponse
{
    /// <summary>
    /// 下载URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 文件流（仅在代理模式下）
    /// </summary>
    public Stream? FileStream { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
}

