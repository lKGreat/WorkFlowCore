using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using WorkFlowCore.Application.DTOs.Auth;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 图形验证码服务实现
/// </summary>
public class CaptchaService : ICaptchaService
{
    private readonly IDistributedCache<CaptchaCacheItem> _cache;

    public CaptchaService(IDistributedCache<CaptchaCacheItem> cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    public async Task<CaptchaInfo> GenerateAsync()
    {
        var uuid = Guid.NewGuid().ToString();
        var code = GenerateRandomCode(4);

        // 缓存2分钟
        await _cache.SetAsync(
            uuid,
            new CaptchaCacheItem { Code = code },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

        // 生成Base64图片
        var imageBase64 = GenerateImage(code);

        return new CaptchaInfo
        {
            Uuid = uuid,
            ImageBase64 = imageBase64,
            ExpireTime = DateTime.Now.AddMinutes(2)
        };
    }

    /// <summary>
    /// 验证验证码
    /// </summary>
    public async Task<bool> ValidateAsync(string uuid, string code)
    {
        var cached = await _cache.GetAsync(uuid);
        if (cached == null)
        {
            return false;
        }

        // 验证后删除
        await _cache.RemoveAsync(uuid);

        return cached.Code.Equals(code, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 生成随机验证码
    /// </summary>
    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }

    /// <summary>
    /// 生成验证码图片 (简化版,使用Base64编码)
    /// TODO: 使用 SkiaSharp 或其他图形库生成真实图片
    /// </summary>
    private string GenerateImage(string code)
    {
        // 简化实现:直接返回文本形式的Base64
        // 生产环境应使用图形库生成真实的验证码图片
        var bytes = System.Text.Encoding.UTF8.GetBytes($"CAPTCHA:{code}");
        return $"data:text/plain;base64,{Convert.ToBase64String(bytes)}";
    }
}
