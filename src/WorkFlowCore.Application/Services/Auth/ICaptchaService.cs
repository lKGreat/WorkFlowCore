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

