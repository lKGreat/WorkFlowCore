using Microsoft.Extensions.Caching.Distributed;
using SkiaSharp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using WorkFlowCore.Application.DTOs.Auth;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 图形验证码服务实现
/// </summary>
public class CaptchaService : ICaptchaService, ITransientDependency
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
    /// 生成验证码图片 (使用SkiaSharp)
    /// </summary>
    private string GenerateImage(string code)
    {
        using var surface = SkiaSharp.SKSurface.Create(new SkiaSharp.SKImageInfo(120, 40));
        var canvas = surface.Canvas;
        canvas.Clear(SkiaSharp.SKColors.White);

        // 绘制背景噪点
        var random = new Random();
        using var noisePaint = new SkiaSharp.SKPaint { Color = SkiaSharp.SKColors.LightGray };
        for (int i = 0; i < 50; i++)
        {
            canvas.DrawCircle(random.Next(120), random.Next(40), 1, noisePaint);
        }

        // 绘制验证码文字 (使用新API)
        using var font = new SkiaSharp.SKFont(SkiaSharp.SKTypeface.FromFamilyName("Arial", SkiaSharp.SKFontStyle.Bold), 28);
        using var textPaint = new SkiaSharp.SKPaint
        {
            Color = SkiaSharp.SKColors.Black,
            IsAntialias = true
        };

        float x = 10;
        for (int i = 0; i < code.Length; i++)
        {
            // 随机倾斜和颜色
            canvas.Save();
            canvas.Translate(x, 30);
            canvas.RotateDegrees(random.Next(-15, 15));
            
            var colors = new[] { SkiaSharp.SKColors.Blue, SkiaSharp.SKColors.Red, SkiaSharp.SKColors.Green, SkiaSharp.SKColors.Orange };
            textPaint.Color = colors[random.Next(colors.Length)];
            
            canvas.DrawText(code[i].ToString(), 0, 0, font, textPaint);
            canvas.Restore();
            
            x += 25;
        }

        // 绘制干扰线
        using var linePaint = new SkiaSharp.SKPaint { Color = SkiaSharp.SKColors.Gray, StrokeWidth = 1 };
        for (int i = 0; i < 3; i++)
        {
            canvas.DrawLine(random.Next(120), random.Next(40), random.Next(120), random.Next(40), linePaint);
        }

        // 转换为Base64
        using var image = surface.Snapshot();
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        var bytes = data.ToArray();
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
