using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Domain.Storage;

namespace WorkFlowCore.Infrastructure.Storage;

/// <summary>
/// 存储提供者工厂（基于 ABP BlobStoring）
/// </summary>
public class StorageProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StorageProviderFactory> _logger;

    public StorageProviderFactory(
        IServiceProvider serviceProvider,
        ILogger<StorageProviderFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 根据配置创建存储提供者实例
    /// </summary>
    public IStorageProvider CreateProvider(FileStorageProvider providerEntity)
    {
        if (!providerEntity.IsEnabled)
        {
            throw new InvalidOperationException($"Storage provider '{providerEntity.ProviderName}' is not enabled");
        }

        // 所有类型都使用统一的 LocalStorageProvider（基于 ABP BlobStoring）
        // ABP BlobStoring 会根据配置自动选择正确的提供者
        return _serviceProvider.GetRequiredService<LocalStorageProvider>();
    }
}

