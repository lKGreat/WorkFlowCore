using Volo.Abp.DependencyInjection;

namespace WorkFlowCore.Application.Services.Sms;

/// <summary>
/// 短信提供商接口
/// </summary>
public interface ISmsProvider : ITransientDependency
{
    /// <summary>
    /// 提供商名称
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="templateCode">模板代码</param>
    /// <param name="templateParams">模板参数</param>
    /// <returns>是否成功</returns>
    Task<(bool Success, string Message)> SendAsync(string phoneNumber, string templateCode, Dictionary<string, string> templateParams);
}

