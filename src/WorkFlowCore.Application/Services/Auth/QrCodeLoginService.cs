using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using WorkFlowCore.Application.DTOs.Auth;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 二维码登录服务实现
/// </summary>
public class QrCodeLoginService : IQrCodeLoginService
{
    private readonly IDistributedCache<QrCodeCacheItem> _cache;

    public QrCodeLoginService(IDistributedCache<QrCodeCacheItem> cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    public async Task<QrCodeInfo> GenerateQrCodeAsync(string deviceId)
    {
        var uuid = Guid.NewGuid().ToString();
        var state = Guid.NewGuid().ToString();

        // 缓存5分钟
        await _cache.SetAsync(
            uuid,
            new QrCodeCacheItem
            {
                State = state,
                Status = QrCodeStatus.WaitScan,
                DeviceId = deviceId
            },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        // 生成二维码内容
        var qrContent = JsonSerializer.Serialize(new
        {
            uuid,
            state,
            deviceId,
            type = "login"
        });

        return new QrCodeInfo
        {
            Uuid = uuid,
            State = state,
            QrContent = qrContent,
            ExpireTime = DateTime.Now.AddMinutes(5)
        };
    }

    /// <summary>
    /// 扫描二维码
    /// </summary>
    public async Task<bool> ScanAsync(string uuid, Guid userId)
    {
        var cached = await _cache.GetAsync(uuid);
        if (cached == null || cached.Status != QrCodeStatus.WaitScan)
        {
            return false;
        }

        // 更新为已扫描状态
        cached.Status = QrCodeStatus.Scanned;
        cached.UserId = userId;
        await _cache.SetAsync(uuid, cached);

        return true;
    }

    /// <summary>
    /// 确认登录
    /// </summary>
    public async Task<bool> ConfirmAsync(string uuid, Guid userId)
    {
        var cached = await _cache.GetAsync(uuid);
        if (cached == null || cached.UserId != userId)
        {
            return false;
        }

        // 更新为已确认状态
        cached.Status = QrCodeStatus.Confirmed;
        await _cache.SetAsync(uuid, cached);

        return true;
    }

    /// <summary>
    /// 轮询二维码状态
    /// </summary>
    public async Task<QrCodeLoginResult> PollStatusAsync(string uuid)
    {
        var cached = await _cache.GetAsync(uuid);
        if (cached == null)
        {
            return new QrCodeLoginResult { Status = QrCodeStatus.Expired };
        }

        var result = new QrCodeLoginResult
        {
            Status = cached.Status,
            UserId = cached.UserId
        };

        // 如果已确认,删除缓存(Token由Controller生成)
        if (cached.Status == QrCodeStatus.Confirmed)
        {
            await _cache.RemoveAsync(uuid);
        }

        return result;
    }
}

