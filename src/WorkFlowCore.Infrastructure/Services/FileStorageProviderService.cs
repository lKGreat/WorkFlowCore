using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Storage;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 文件存储提供者服务实现
/// </summary>
public class FileStorageProviderService : ApplicationService, IFileStorageProviderService
{
    private readonly IRepository<FileStorageProvider, long> _providerRepository;
    private readonly IRepository<FileAttachment, long> _attachmentRepository;
    private readonly StorageProviderFactory _providerFactory;

    public FileStorageProviderService(
        IRepository<FileStorageProvider, long> providerRepository,
        IRepository<FileAttachment, long> attachmentRepository,
        StorageProviderFactory providerFactory)
    {
        _providerRepository = providerRepository;
        _attachmentRepository = attachmentRepository;
        _providerFactory = providerFactory;
    }

    public async Task<List<FileStorageProviderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetListAsync(cancellationToken: cancellationToken);
        var sortedProviders = providers.OrderBy(p => p.Priority).ToList();
        return ObjectMapper.Map<List<FileStorageProvider>, List<FileStorageProviderDto>>(sortedProviders);
    }

    public async Task<List<FileStorageProviderDto>> GetEnabledAsync(CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetListAsync(p => p.IsEnabled, cancellationToken: cancellationToken);
        var sortedProviders = providers.OrderBy(p => p.Priority).ToList();
        return ObjectMapper.Map<List<FileStorageProvider>, List<FileStorageProviderDto>>(sortedProviders);
    }

    public async Task<FileStorageProviderDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.FindAsync(id, cancellationToken: cancellationToken);
        return provider == null ? null : ObjectMapper.Map<FileStorageProvider, FileStorageProviderDto>(provider);
    }

    public async Task<FileStorageProviderDto> CreateAsync(CreateFileStorageProviderRequest request, CancellationToken cancellationToken = default)
    {
        // 生成雪花ID（这里简化处理，实际应使用雪花算法生成器）
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        var provider = new FileStorageProvider(id, CurrentTenant.Id, request.ProviderName, request.ProviderType)
        {
            Configuration = request.Configuration,
            IsEnabled = request.IsEnabled,
            Priority = request.Priority
        };

        provider = await _providerRepository.InsertAsync(provider, cancellationToken: cancellationToken);

        Logger.LogInformation("Created file storage provider: {ProviderName} (ID: {Id})", provider.ProviderName, provider.Id);

        return ObjectMapper.Map<FileStorageProvider, FileStorageProviderDto>(provider);
    }

    public async Task<FileStorageProviderDto> UpdateAsync(long id, UpdateFileStorageProviderRequest request, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.GetAsync(id, cancellationToken: cancellationToken);

        if (request.ProviderName != null)
            provider.ProviderName = request.ProviderName;
        
        if (request.Configuration != null)
            provider.Configuration = request.Configuration;
        
        if (request.IsEnabled.HasValue)
            provider.IsEnabled = request.IsEnabled.Value;
        
        if (request.Priority.HasValue)
            provider.Priority = request.Priority.Value;

        provider = await _providerRepository.UpdateAsync(provider, cancellationToken: cancellationToken);

        Logger.LogInformation("Updated file storage provider: {Id}", id);

        return ObjectMapper.Map<FileStorageProvider, FileStorageProviderDto>(provider);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.GetAsync(id, cancellationToken: cancellationToken);

        // 检查是否有附件在使用此提供者
        var hasAttachments = await _attachmentRepository.AnyAsync(a => a.StorageProviderId == id, cancellationToken);

        if (hasAttachments)
        {
            throw new BusinessException("Cannot delete storage provider that has attachments");
        }

        await _providerRepository.DeleteAsync(provider, cancellationToken: cancellationToken);

        Logger.LogInformation("Deleted file storage provider: {Id}", id);
    }

    public async Task<bool> TestConnectionAsync(long id, CancellationToken cancellationToken = default)
    {
        var provider = await _providerRepository.GetAsync(id, cancellationToken: cancellationToken);

        try
        {
            var storageProvider = _providerFactory.CreateProvider(provider);
            
            // 简单测试：尝试上传一个小文件并删除
            var testPath = $"test/{Guid.NewGuid()}.txt";
            using var testStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("test"));
            
            await storageProvider.UploadChunkAsync(testStream, testPath, cancellationToken);
            await storageProvider.DeleteAsync(testPath, cancellationToken);
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Storage provider connection test failed: {Id}", id);
            return false;
        }
    }

    public async Task<FileStorageProviderDto> GetDefaultProviderAsync(CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetListAsync(p => p.IsEnabled, cancellationToken: cancellationToken);
        var provider = providers.OrderBy(p => p.Priority).FirstOrDefault();

        if (provider == null)
        {
            throw new BusinessException("No enabled storage provider found");
        }

        return ObjectMapper.Map<FileStorageProvider, FileStorageProviderDto>(provider);
    }
}

