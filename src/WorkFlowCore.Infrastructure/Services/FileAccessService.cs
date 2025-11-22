using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Infrastructure.Storage;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 文件访问服务实现
/// </summary>
public class FileAccessService : ApplicationService, IFileAccessService
{
    private readonly IRepository<FileAttachment, long> _attachmentRepository;
    private readonly IRepository<FileStorageProvider, long> _providerRepository;
    private readonly StorageProviderFactory _providerFactory;
    private readonly IConfiguration _configuration;

    public FileAccessService(
        IRepository<FileAttachment, long> attachmentRepository,
        IRepository<FileStorageProvider, long> providerRepository,
        StorageProviderFactory providerFactory,
        IConfiguration configuration)
    {
        _attachmentRepository = attachmentRepository;
        _providerRepository = providerRepository;
        _providerFactory = providerFactory;
        _configuration = configuration;
    }

    public async Task<FileAttachmentDto?> GetAttachmentAsync(long attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.FindAsync(attachmentId, cancellationToken: cancellationToken);
        return attachment == null ? null : ObjectMapper.Map<FileAttachment, FileAttachmentDto>(attachment);
    }

    public async Task<List<FileAttachmentDto>> GetAttachmentsByBusinessAsync(string businessType, string businessId, CancellationToken cancellationToken = default)
    {
        var attachments = await _attachmentRepository.GetListAsync(
            a => a.BusinessType == businessType && a.BusinessId == businessId && a.UploadStatus == UploadStatus.Completed,
            cancellationToken: cancellationToken);

        return ObjectMapper.Map<List<FileAttachment>, List<FileAttachmentDto>>(attachments);
    }

    public async Task<DownloadFileResponse> DownloadFileAsync(DownloadFileRequest request, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(request.AttachmentId, cancellationToken: cancellationToken);

        if (attachment.UploadStatus != UploadStatus.Completed)
        {
            throw new BusinessException("File upload is not completed");
        }

        var provider = await _providerRepository.GetAsync(attachment.StorageProviderId, cancellationToken: cancellationToken);
        var storageProvider = _providerFactory.CreateProvider(provider);

        if (request.DirectUrl)
        {
            // 返回预签名URL
            var url = await storageProvider.GeneratePresignedUrlAsync(
                attachment.StoragePath,
                request.ExpireMinutes,
                cancellationToken);

            return new DownloadFileResponse
            {
                Url = url,
                FileName = attachment.OriginalFileName,
                ContentType = attachment.ContentType
            };
        }
        else
        {
            // 代理下载
            var stream = await storageProvider.GetAsync(attachment.StoragePath, cancellationToken);

            return new DownloadFileResponse
            {
                FileStream = stream,
                FileName = attachment.OriginalFileName,
                ContentType = attachment.ContentType
            };
        }
    }

    public async Task<GenerateAccessTokenResponse> GenerateAccessTokenAsync(long attachmentId, int expireMinutes = 30, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(attachmentId, cancellationToken: cancellationToken);

        if (attachment.UploadStatus != UploadStatus.Completed)
        {
            throw new BusinessException("File upload is not completed");
        }

        var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new BusinessException("JWT SecretKey not configured");
        var expireAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        // 生成JWT令牌
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("attachmentId", attachmentId.ToString()),
                new Claim("tenantId", attachment.TenantId?.ToString() ?? ""),
                new Claim("userId", CurrentUser.Id?.ToString() ?? "")
            }),
            Expires = expireAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // 更新附件的访问令牌
        attachment.AccessToken = tokenString;
        attachment.TokenExpireAt = expireAt;
        await _attachmentRepository.UpdateAsync(attachment, cancellationToken: cancellationToken);

        Logger.LogInformation("Generated access token for attachment: {AttachmentId}", attachmentId);

        return new GenerateAccessTokenResponse
        {
            AccessToken = tokenString,
            ExpireAt = expireAt,
            AccessUrl = $"/api/files/{attachmentId}/download?token={tokenString}"
        };
    }

    public async Task<bool> ValidateAccessTokenAsync(long attachmentId, string token, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.FindAsync(attachmentId, cancellationToken: cancellationToken);
        
        if (attachment == null || attachment.AccessToken != token)
        {
            return false;
        }

        if (attachment.TokenExpireAt.HasValue && attachment.TokenExpireAt.Value < DateTime.UtcNow)
        {
            return false;
        }

        try
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new BusinessException("JWT SecretKey not configured");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task DeleteAttachmentAsync(long attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.GetAsync(attachmentId, cancellationToken: cancellationToken);

        // 删除存储的文件
        if (attachment.UploadStatus == UploadStatus.Completed)
        {
            try
            {
                var provider = await _providerRepository.GetAsync(attachment.StorageProviderId, cancellationToken: cancellationToken);
                var storageProvider = _providerFactory.CreateProvider(provider);
                await storageProvider.DeleteAsync(attachment.StoragePath, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to delete file from storage: {StoragePath}", attachment.StoragePath);
            }
        }

        await _attachmentRepository.DeleteAsync(attachment, cancellationToken: cancellationToken);

        Logger.LogInformation("Deleted attachment: {AttachmentId}", attachmentId);
    }
}

