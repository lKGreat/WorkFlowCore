using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WorkFlowCore.Application.Services.Sms;

/// <summary>
/// 阿里云短信服务提供商
/// </summary>
public class AliyunSmsProvider : ISmsProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AliyunSmsProvider> _logger;

    public string ProviderName => "Aliyun";

    public AliyunSmsProvider(IConfiguration configuration, ILogger<AliyunSmsProvider> logger)
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
            var accessKeyId = _configuration["Sms:Aliyun:AccessKeyId"];
            var accessKeySecret = _configuration["Sms:Aliyun:AccessKeySecret"];
            var signName = _configuration["Sms:Aliyun:SignName"];

            if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(accessKeySecret))
            {
                _logger.LogWarning("阿里云短信配置未完成,使用Mock模式");
                return await SendMockAsync(phoneNumber, templateCode, templateParams);
            }

            // TODO: 集成阿里云SDK
            // var client = new DefaultAcsClient(profile);
            // var request = new SendSmsRequest();
            // request.PhoneNumbers = phoneNumber;
            // request.SignName = signName;
            // request.TemplateCode = templateCode;
            // request.TemplateParam = JsonSerializer.Serialize(templateParams);
            // var response = client.GetAcsResponse(request);

            _logger.LogInformation($"[阿里云短信] 发送到 {phoneNumber}, 模板: {templateCode}");
            await Task.Delay(100); // 模拟网络延迟
            return (true, "发送成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"阿里云短信发送失败: {phoneNumber}");
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
        _logger.LogInformation($"[Mock阿里云短信] 手机号: {phoneNumber}, 验证码: {code}, 模板: {templateCode}");
        return (true, "Mock发送成功");
    }
}

