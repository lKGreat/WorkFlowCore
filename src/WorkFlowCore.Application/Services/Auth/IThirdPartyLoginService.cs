using Volo.Abp.DependencyInjection;
using WorkFlowCore.Application.DTOs.Auth;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 第三方登录服务接口
/// </summary>
public interface IThirdPartyLoginService : ITransientDependency
{
    /// <summary>
    /// 获取授权URL
    /// </summary>
    Task<string> GetAuthorizationUrlAsync(string provider, string redirectUrl);

    /// <summary>
    /// 处理回调
    /// </summary>
    Task<ThirdPartyLoginResult> CallbackAsync(string provider, string code, string state);

    /// <summary>
    /// 绑定账号
    /// </summary>
    Task<bool> BindAccountAsync(Guid userId, string provider, string tempToken);

    /// <summary>
    /// 解绑账号
    /// </summary>
    Task<bool> UnbindAccountAsync(Guid userId, string provider);
}

