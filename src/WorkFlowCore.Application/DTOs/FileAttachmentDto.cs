using System;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 文件附件DTO
/// </summary>
public class FileAttachmentDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long StorageProviderId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string Md5Hash { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string BusinessId { get; set; } = string.Empty;
    public UploadStatus UploadStatus { get; set; }
    public int TotalChunks { get; set; }
    public int UploadedChunks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 上传进度百分比
    /// </summary>
    public double ProgressPercentage => TotalChunks > 0 ? (double)UploadedChunks / TotalChunks * 100 : 0;
}

