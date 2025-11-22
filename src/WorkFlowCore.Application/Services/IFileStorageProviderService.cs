using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 文件存储提供者服务接口
/// </summary>
public interface IFileStorageProviderService
{
    /// <summary>
    /// 获取所有存储提供者
    /// </summary>
    Task<List<FileStorageProviderDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已启用的存储提供者
    /// </summary>
    Task<List<FileStorageProviderDto>> GetEnabledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取存储提供者
    /// </summary>
    Task<FileStorageProviderDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建存储提供者
    /// </summary>
    Task<FileStorageProviderDto> CreateAsync(CreateFileStorageProviderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新存储提供者
    /// </summary>
    Task<FileStorageProviderDto> UpdateAsync(long id, UpdateFileStorageProviderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除存储提供者
    /// </summary>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试存储提供者连接
    /// </summary>
    Task<bool> TestConnectionAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取默认存储提供者
    /// </summary>
    Task<FileStorageProviderDto> GetDefaultProviderAsync(CancellationToken cancellationToken = default);
}

