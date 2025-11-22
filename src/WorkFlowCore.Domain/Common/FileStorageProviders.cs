using System;

namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 文件存储提供器常量
/// </summary>
public static class FileStorageProviders
{
    public const string Local = "Local";
    public const string Aliyun = "Aliyun";
    public const string Aws = "Aws";

    public static bool IsValid(string provider)
    {
        return provider switch
        {
            Local => true,
            Aliyun => true,
            Aws => true,
            _ => false
        };
    }

    public static FileStorageProviderType ToEnum(string provider)
    {
        return provider switch
        {
            Local => FileStorageProviderType.Local,
            Aliyun => FileStorageProviderType.Aliyun,
            Aws => FileStorageProviderType.Aws,
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, "未知的存储提供器")
        };
    }

    public static string FromEnum(FileStorageProviderType providerType)
    {
        return providerType switch
        {
            FileStorageProviderType.Local => Local,
            FileStorageProviderType.Aliyun => Aliyun,
            FileStorageProviderType.Aws => Aws,
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, "未知的存储提供器")
        };
    }
}

/// <summary>
/// 文件存储提供器类型
/// </summary>
public enum FileStorageProviderType
{
    Local = 0,
    Aliyun = 1,
    Aws = 2
}

