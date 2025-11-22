using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 文件存储提供者配置实体
/// </summary>
public class FileStorageProvider : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 提供者名称
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// 提供者类型
    /// </summary>
    public StorageProviderType ProviderType { get; set; }

    /// <summary>
    /// 配置信息（JSON格式）
    /// </summary>
    public string Configuration { get; set; } = "{}";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 优先级（数字越小优先级越高）
    /// </summary>
    public int Priority { get; set; } = 999;

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected FileStorageProvider() { }

    public FileStorageProvider(long id, Guid? tenantId, string providerName, StorageProviderType providerType)
        : base(id)
    {
        TenantId = tenantId;
        ProviderName = providerName;
        ProviderType = providerType;
    }
}

/// <summary>
/// 存储提供者类型枚举
/// </summary>
public enum StorageProviderType
{
    /// <summary>
    /// 本地文件系统
    /// </summary>
    Local = 0,

    /// <summary>
    /// 阿里云 OSS
    /// </summary>
    AliyunOss = 1,

    /// <summary>
    /// AWS S3
    /// </summary>
    AwsS3 = 2,

    /// <summary>
    /// Azure Blob Storage
    /// </summary>
    AzureBlob = 3
}

