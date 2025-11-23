using Volo.Abp.DependencyInjection;
using WorkFlowCore.Domain.Common;

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

