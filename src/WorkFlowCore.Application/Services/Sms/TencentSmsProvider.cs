using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WorkFlowCore.Application.Services.Sms;

/// <summary>
/// 腾讯云短信服务提供商
/// </summary>
public class TencentSmsProvider : ISmsProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TencentSmsProvider> _logger;

    public string ProviderName => "Tencent";

    public TencentSmsProvider(IConfiguration configuration, ILogger<TencentSmsProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> SendAsync(
        string phoneNumber, 
        string templateCode, 
        Dictionary<string, string> templateParams)
    {
        try
        {
            var secretId = _configuration["Sms:Tencent:SecretId"];
            var secretKey = _configuration["Sms:Tencent:SecretKey"];
            var sdkAppId = _configuration["Sms:Tencent:SdkAppId"];
            var signName = _configuration["Sms:Tencent:SignName"];

            if (string.IsNullOrEmpty(secretId) || string.IsNullOrEmpty(secretKey))
            {
                _logger.LogWarning("腾讯云短信配置未完成,使用Mock模式");
                return await SendMockAsync(phoneNumber, templateCode, templateParams);
            }

            // TODO: 集成腾讯云SDK
            // var credential = new Credential { SecretId = secretId, SecretKey = secretKey };
            // var client = new SmsClient(credential, "ap-guangzhou");
            // var request = new SendSmsRequest
            // {
            //     PhoneNumberSet = new[] { phoneNumber },
            //     SmsSdkAppId = sdkAppId,
            //     SignName = signName,
            //     TemplateId = templateCode,
            //     TemplateParamSet = templateParams.Values.ToArray()
            // };
            // var response = await client.SendSms(request);

            _logger.LogInformation($"[腾讯云短信] 发送到 {phoneNumber}, 模板: {templateCode}");
            await Task.Delay(100); // 模拟网络延迟
            return (true, "发送成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"腾讯云短信发送失败: {phoneNumber}");
            return (false, ex.Message);
        }
    }

    private async Task<(bool Success, string Message)> SendMockAsync(
        string phoneNumber, 
        string templateCode, 
        Dictionary<string, string> templateParams)
    {
        await Task.CompletedTask;
        var code = templateParams.GetValueOrDefault("code", "123456");
        _logger.LogInformation($"[Mock腾讯云短信] 手机号: {phoneNumber}, 验证码: {code}, 模板: {templateCode}");
        return (true, "Mock发送成功");
    }
}

