using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace WorkFlowCore.Application.Services.Sms;

/// <summary>
/// 短信服务(统一入口)
/// </summary>
public class SmsService : ITransientDependency
{
    private readonly IEnumerable<ISmsProvider> _providers;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        IEnumerable<ISmsProvider> providers,
        IConfiguration configuration,
        ILogger<SmsService> logger)
    {
        _providers = providers;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 发送验证码短信
    /// </summary>
    public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code, string type = "登录")
    {
        var provider = GetProvider();
        var templateCode = GetTemplateCode(type);
        var templateParams = new Dictionary<string, string>
        {
            { "code", code }
        };

        var (success, message) = await provider.SendAsync(phoneNumber, templateCode, templateParams);
        
        if (success)
        {
            _logger.LogInformation($"短信发送成功: {phoneNumber}, 类型: {type}");
        }
        else
        {
            _logger.LogError($"短信发送失败: {phoneNumber}, 错误: {message}");
        }

        return success;
    }

    /// <summary>
    /// 获取当前使用的短信提供商
    /// </summary>
    private ISmsProvider GetProvider()
    {
        var providerName = _configuration["Sms:Provider"] ?? "Aliyun";
        var provider = _providers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        
        if (provider == null)
        {
            _logger.LogWarning($"未找到短信提供商: {providerName}, 使用默认(Aliyun)");
            provider = _providers.First(p => p.ProviderName == "Aliyun");
        }

        return provider;
    }

    /// <summary>
    /// 根据类型获取模板代码
    /// </summary>
    private string GetTemplateCode(string type)
    {
        return type switch
        {
            "登录" => _configuration["Sms:Templates:Login"] ?? "SMS_LOGIN",
            "注册" => _configuration["Sms:Templates:Register"] ?? "SMS_REGISTER",
            "重置密码" => _configuration["Sms:Templates:ResetPassword"] ?? "SMS_RESET_PWD",
            "绑定手机" => _configuration["Sms:Templates:BindPhone"] ?? "SMS_BIND_PHONE",
            _ => "SMS_DEFAULT"
        };
    }
}

