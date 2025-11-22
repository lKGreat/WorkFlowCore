using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 文件分片记录实体
/// </summary>
public class FileChunk : FullAuditedAggregateRoot<long>
{
    /// <summary>
    /// 附件ID
    /// </summary>
    public long AttachmentId { get; set; }

    /// <summary>
    /// 分片序号（从0开始）
    /// </summary>
    public int ChunkIndex { get; set; }

    /// <summary>
    /// 分片大小（字节）
    /// </summary>
    public long ChunkSize { get; set; }

    /// <summary>
    /// 分片哈希值
    /// </summary>
    public string ChunkHash { get; set; } = string.Empty;

    /// <summary>
    /// 上传状态
    /// </summary>
    public ChunkUploadStatus UploadStatus { get; set; } = ChunkUploadStatus.Pending;

    /// <summary>
    /// 存储路径
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// 附件导航属性
    /// </summary>
    public virtual FileAttachment? Attachment { get; set; }

    protected FileChunk() { }

    public FileChunk(long id, long attachmentId, int chunkIndex, long chunkSize)
        : base(id)
    {
        AttachmentId = attachmentId;
        ChunkIndex = chunkIndex;
        ChunkSize = chunkSize;
    }
}

/// <summary>
/// 分片上传状态枚举
/// </summary>
public enum ChunkUploadStatus
{
    /// <summary>
    /// 等待上传
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 上传中
    /// </summary>
    Uploading = 1,

    /// <summary>
    /// 上传完成
    /// </summary>
    Completed = 2,

    /// <summary>
    /// 上传失败
    /// </summary>
    Failed = 3
}

