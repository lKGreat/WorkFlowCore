using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Volo.Abp.DependencyInjection;
using WorkFlowCore.Application.DTOs.Auth;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 短信验证码服务接口
/// </summary>
public interface ISmsCodeService : ITransientDependency
{
    /// <summary>
    /// 发送短信验证码
    /// </summary>
    Task<bool> SendAsync(string phoneNumber, SmsCodeType type);

    /// <summary>
    /// 验证短信验证码
    /// </summary>
    Task<bool> ValidateAsync(string phoneNumber, string code);
}

/// <summary>
/// 短信验证码服务实现
/// </summary>
public class SmsCodeService : ISmsCodeService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<SmsCodeService> _logger;

    public SmsCodeService(IDistributedCache cache, ILogger<SmsCodeService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    public async Task<bool> SendAsync(string phoneNumber, SmsCodeType type)
    {
        // 生成6位随机验证码
        var code = Random.Shared.Next(100000, 999999).ToString();

        // TODO: 调用短信服务商API发送验证码（阿里云、腾讯云等）
        _logger.LogInformation($"发送验证码到 {phoneNumber}: {code} (类型: {type})");

        // 缓存验证码（5分钟有效期）
        var cacheItem = new SmsCodeCacheItem
        {
            Code = code,
            Type = type,
            CreatedTime = DateTime.Now
        };

        var cacheKey = $"sms_code:{phoneNumber}";
        var cacheValue = JsonSerializer.Serialize(cacheItem);

        await _cache.SetStringAsync(
            cacheKey,
            cacheValue,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }
        );

        return true;
    }

    /// <summary>
    /// 验证短信验证码
    /// </summary>
    public async Task<bool> ValidateAsync(string phoneNumber, string code)
    {
        var cacheKey = $"sms_code:{phoneNumber}";
        var cacheValue = await _cache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(cacheValue))
        {
            return false;
        }

        // 验证后删除缓存（防止重复使用）
        await _cache.RemoveAsync(cacheKey);

        var cached = JsonSerializer.Deserialize<SmsCodeCacheItem>(cacheValue);
        if (cached == null)
        {
            return false;
        }

        return cached.Code == code;
    }
}

