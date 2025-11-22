using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 文件访问服务接口
/// </summary>
public interface IFileAccessService
{
    /// <summary>
    /// 根据ID获取附件信息
    /// </summary>
    Task<FileAttachmentDto?> GetAttachmentAsync(long attachmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据业务类型和业务ID获取附件列表
    /// </summary>
    Task<List<FileAttachmentDto>> GetAttachmentsByBusinessAsync(string businessType, string businessId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载文件
    /// </summary>
    Task<DownloadFileResponse> DownloadFileAsync(DownloadFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    Task<GenerateAccessTokenResponse> GenerateAccessTokenAsync(long attachmentId, int expireMinutes = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证访问令牌
    /// </summary>
    Task<bool> ValidateAccessTokenAsync(long attachmentId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除附件
    /// </summary>
    Task DeleteAttachmentAsync(long attachmentId, CancellationToken cancellationToken = default);
}

