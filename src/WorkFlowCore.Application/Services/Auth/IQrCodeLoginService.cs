using Volo.Abp.DependencyInjection;
using WorkFlowCore.Application.DTOs.Auth;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 二维码登录服务接口
/// </summary>
public interface IQrCodeLoginService : ITransientDependency
{
    /// <summary>
    /// 生成二维码
    /// </summary>
    Task<QrCodeInfo> GenerateQrCodeAsync(string deviceId);

    /// <summary>
    /// 扫描二维码
    /// </summary>
    Task<bool> ScanAsync(string uuid, Guid userId);

    /// <summary>
    /// 确认登录
    /// </summary>
    Task<bool> ConfirmAsync(string uuid, Guid userId);

    /// <summary>
    /// 轮询二维码状态
    /// </summary>
    Task<QrCodeLoginResult> PollStatusAsync(string uuid);
}

