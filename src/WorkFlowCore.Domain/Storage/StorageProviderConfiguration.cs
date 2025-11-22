namespace WorkFlowCore.Domain.Storage;

/// <summary>
/// 存储提供者配置基类
/// </summary>
public abstract class StorageProviderConfiguration
{
    /// <summary>
    /// 提供者类型
    /// </summary>
    public abstract string ProviderType { get; }
}

/// <summary>
/// 本地存储配置
/// </summary>
public class LocalStorageConfiguration : StorageProviderConfiguration
{
    public override string ProviderType => "Local";

    /// <summary>
    /// 本地存储根路径
    /// </summary>
    public string RootPath { get; set; } = "FileStorage/Local";

    /// <summary>
    /// 访问URL前缀
    /// </summary>
    public string? UrlPrefix { get; set; }
}

/// <summary>
/// 阿里云OSS配置
/// </summary>
public class AliyunOssConfiguration : StorageProviderConfiguration
{
    public override string ProviderType => "AliyunOss";

    /// <summary>
    /// AccessKey ID
    /// </summary>
    public string AccessKeyId { get; set; } = string.Empty;

    /// <summary>
    /// AccessKey Secret
    /// </summary>
    public string AccessKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// Bucket名称
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// 终端节点（如：oss-cn-hangzhou.aliyuncs.com）
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 自定义域名（可选）
    /// </summary>
    public string? CustomDomain { get; set; }
}

/// <summary>
/// AWS S3配置
/// </summary>
public class AwsS3Configuration : StorageProviderConfiguration
{
    public override string ProviderType => "AwsS3";

    /// <summary>
    /// AccessKey
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// SecretKey
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Bucket名称
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// 区域（如：us-east-1）
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 自定义终端节点（可选）
    /// </summary>
    public string? ServiceUrl { get; set; }
}

/// <summary>
/// Azure Blob存储配置
/// </summary>
public class AzureBlobConfiguration : StorageProviderConfiguration
{
    public override string ProviderType => "AzureBlob";

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 容器名称
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;
}

