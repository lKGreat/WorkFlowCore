using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Volo.Abp.Caching;
using WorkFlowCore.Application.Services.Sms;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 短信验证码服务实现
/// </summary>
public class SmsCodeService : ISmsCodeService
{
    private readonly IDistributedCache<SmsCodeCacheItem> _cache;
    private readonly SmsService _smsService;
    private readonly ILogger<SmsCodeService> _logger;

    public SmsCodeService(
        IDistributedCache<SmsCodeCacheItem> cache,
        SmsService smsService,
        ILogger<SmsCodeService> logger)
    {
        _cache = cache;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    public async Task<bool> SendAsync(string phoneNumber, SmsCodeType type)
    {
        // 生成6位验证码
        var code = Random.Shared.Next(100000, 999999).ToString();

        // 调用短信服务发送验证码
        var typeName = type switch
        {
            SmsCodeType.Login => "登录",
            SmsCodeType.Register => "注册",
            SmsCodeType.ResetPassword => "重置密码",
            SmsCodeType.BindPhone => "绑定手机",
            _ => "登录"
        };

        var success = await _smsService.SendVerificationCodeAsync(phoneNumber, code, typeName);
        
        if (!success)
        {
            _logger.LogError($"短信发送失败: {phoneNumber}");
            return false;
        }

        // 缓存5分钟
        await _cache.SetAsync(
            phoneNumber,
            new SmsCodeCacheItem { Code = code, Type = type },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        _logger.LogInformation($"短信验证码已发送并缓存: {phoneNumber}");
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
