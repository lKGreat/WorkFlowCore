using Lazy.Captcha.Core;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using WorkFlowCore.Application.DTOs.Auth;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 图形验证码服务接口
/// </summary>
public interface ICaptchaService : ITransientDependency
{
    /// <summary>
    /// 生成验证码
    /// </summary>
    Task<CaptchaInfo> GenerateAsync();

    /// <summary>
    /// 验证验证码
    /// </summary>
    Task<bool> ValidateAsync(string uuid, string code);
}

/// <summary>
/// 图形验证码服务实现
/// </summary>
public class CaptchaService : ICaptchaService
{
    private readonly ICaptcha _captcha;
    private readonly IDistributedCache _cache;

    public CaptchaService(ICaptcha captcha, IDistributedCache cache)
    {
        _captcha = captcha;
        _cache = cache;
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    public async Task<CaptchaInfo> GenerateAsync()
    {
        var uuid = Guid.NewGuid().ToString();
        var info = _captcha.Generate(uuid, 120); // 120秒有效期

        // 缓存验证码（2分钟）
        var cacheItem = new CaptchaCacheItem { Code = info.Code };
        var cacheValue = JsonSerializer.Serialize(cacheItem);
        
        await _cache.SetStringAsync(
            $"captcha:{uuid}",
            cacheValue,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
            }
        );

        return new CaptchaInfo
        {
            Uuid = uuid,
            ImageBase64 = info.Base64,
            ExpireTime = DateTime.Now.AddSeconds(120)
        };
    }

    /// <summary>
    /// 验证验证码
    /// </summary>
    public async Task<bool> ValidateAsync(string uuid, string code)
    {
        var cacheKey = $"captcha:{uuid}";
        var cacheValue = await _cache.GetStringAsync(cacheKey);
        
        if (string.IsNullOrEmpty(cacheValue))
        {
            return false;
        }

        // 验证后删除缓存（防止重复使用）
        await _cache.RemoveAsync(cacheKey);

        var cached = JsonSerializer.Deserialize<CaptchaCacheItem>(cacheValue);
        if (cached == null)
        {
            return false;
        }

        return cached.Code.Equals(code, StringComparison.OrdinalIgnoreCase);
    }
}

