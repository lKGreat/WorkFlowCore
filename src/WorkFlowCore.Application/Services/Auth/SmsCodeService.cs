using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Volo.Abp.Caching;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 短信验证码服务实现
/// </summary>
public class SmsCodeService : ISmsCodeService
{
    private readonly IDistributedCache<SmsCodeCacheItem> _cache;
    private readonly ILogger<SmsCodeService> _logger;

    public SmsCodeService(
        IDistributedCache<SmsCodeCacheItem> cache,
        ILogger<SmsCodeService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    public async Task<bool> SendAsync(string phoneNumber, SmsCodeType type)
    {
        // 生成6位验证码
        var code = Random.Shared.Next(100000, 999999).ToString();

        // TODO: 调用短信服务商API发送验证码
        _logger.LogInformation($"发送验证码到 {phoneNumber}: {code}");

        // 缓存5分钟
        await _cache.SetAsync(
            phoneNumber,
            new SmsCodeCacheItem { Code = code, Type = type },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return true;
    }

    /// <summary>
    /// 验证短信验证码
    /// </summary>
    public async Task<bool> ValidateAsync(string phoneNumber, string code)
    {
        var cached = await _cache.GetAsync(phoneNumber);
        if (cached == null)
        {
            return false;
        }

        // 验证后删除
        await _cache.RemoveAsync(phoneNumber);

        return cached.Code == code;
    }
}
