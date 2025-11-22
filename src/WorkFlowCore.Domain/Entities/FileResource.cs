using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 文件资源实体，保存统一文件元数据
/// </summary>
public class FileResource : Entity<Guid>, ITenantEntity, ISoftDelete
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// 上传人
    /// </summary>
    public Guid? UploadedBy { get; set; }

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 文件哈希（SHA256）
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// 存储提供器
    /// </summary>
    public string StorageProvider { get; set; } = string.Empty;

    /// <summary>
    /// 存储Key（Provider内部唯一标识）
    /// </summary>
    public string StorageKey { get; set; } = string.Empty;

    /// <summary>
    /// 附加元数据（JSON）
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

