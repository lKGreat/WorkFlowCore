using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 分片上传会话
/// </summary>
public class FileUploadSession : Entity<Guid>, ITenantEntity, ISoftDelete
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";

    /// <summary>
    /// 总大小
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// 单个分片大小
    /// </summary>
    public int ChunkSize { get; set; }

    /// <summary>
    /// 总分片数
    /// </summary>
    public int TotalChunks { get; set; }

    /// <summary>
    /// 已上传分片数
    /// </summary>
    public int UploadedChunks { get; set; }

    /// <summary>
    /// 目标存储提供器
    /// </summary>
    public string StorageProvider { get; set; } = string.Empty;

    /// <summary>
    /// 附加元数据（JSON）
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// 临时存储目录
    /// </summary>
    public string TempDirectory { get; set; } = string.Empty;

    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 最终文件ID
    /// </summary>
    public Guid? FileResourceId { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

