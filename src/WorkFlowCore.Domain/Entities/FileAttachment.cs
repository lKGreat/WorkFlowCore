using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 文件附件实体
/// </summary>
public class FileAttachment : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 文件名（存储时的文件名）
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 内容类型（MIME Type）
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// 存储提供者ID
    /// </summary>
    public long StorageProviderId { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件MD5哈希值
    /// </summary>
    public string Md5Hash { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型（如"入库单"、"请假"、"出库"等）
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// 业务单据ID
    /// </summary>
    public string BusinessId { get; set; } = string.Empty;

    /// <summary>
    /// 上传状态
    /// </summary>
    public UploadStatus UploadStatus { get; set; } = UploadStatus.Uploading;

    /// <summary>
    /// 总分片数
    /// </summary>
    public int TotalChunks { get; set; }

    /// <summary>
    /// 已上传分片数
    /// </summary>
    public int UploadedChunks { get; set; }

    /// <summary>
    /// 访问令牌（用于临时访问控制）
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 令牌过期时间
    /// </summary>
    public DateTime? TokenExpireAt { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    /// <summary>
    /// 存储提供者导航属性
    /// </summary>
    public virtual FileStorageProvider? StorageProvider { get; set; }

    protected FileAttachment() { }

    public FileAttachment(
        long id,
        Guid? tenantId,
        string originalFileName,
        long fileSize,
        string contentType,
        string md5Hash,
        string businessType,
        string businessId,
        long storageProviderId)
        : base(id)
    {
        TenantId = tenantId;
        OriginalFileName = originalFileName;
        FileSize = fileSize;
        ContentType = contentType;
        Md5Hash = md5Hash;
        BusinessType = businessType;
        BusinessId = businessId;
        StorageProviderId = storageProviderId;

        // 设置文件扩展名
        FileExtension = System.IO.Path.GetExtension(originalFileName).ToLowerInvariant();
    }
}

/// <summary>
/// 上传状态枚举
/// </summary>
public enum UploadStatus
{
    /// <summary>
    /// 上传中
    /// </summary>
    Uploading = 0,

    /// <summary>
    /// 上传完成
    /// </summary>
    Completed = 1,

    /// <summary>
    /// 上传失败
    /// </summary>
    Failed = 2,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 3
}

