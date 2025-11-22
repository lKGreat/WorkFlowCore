using System;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 文件存储提供者DTO
/// </summary>
public class FileStorageProviderDto
{
    public long Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public StorageProviderType ProviderType { get; set; }
    public string Configuration { get; set; } = "{}";
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建存储提供者请求
/// </summary>
public class CreateFileStorageProviderRequest
{
    public string ProviderName { get; set; } = string.Empty;
    public StorageProviderType ProviderType { get; set; }
    public string Configuration { get; set; } = "{}";
    public bool IsEnabled { get; set; } = true;
    public int Priority { get; set; } = 999;
}

/// <summary>
/// 更新存储提供者请求
/// </summary>
public class UpdateFileStorageProviderRequest
{
    public string? ProviderName { get; set; }
    public string? Configuration { get; set; }
    public bool? IsEnabled { get; set; }
    public int? Priority { get; set; }
}

