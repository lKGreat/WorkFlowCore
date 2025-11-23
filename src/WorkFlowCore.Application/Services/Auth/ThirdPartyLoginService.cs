using Microsoft.Extensions.Configuration;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs.Auth;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.Application.Services.Auth;

/// <summary>
/// 第三方登录服务实现
/// </summary>
public class ThirdPartyLoginService : IThirdPartyLoginService
{
    private readonly IRepository<UserThirdPartyAccount, long> _thirdPartyAccountRepository;
    private readonly IRepository<AppUser, Guid> _userRepository;
    private readonly IConfiguration _configuration;

    public ThirdPartyLoginService(
        IRepository<UserThirdPartyAccount, long> thirdPartyAccountRepository,
        IRepository<AppUser, Guid> userRepository,
        IConfiguration configuration)
    {
        _thirdPartyAccountRepository = thirdPartyAccountRepository;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// 获取授权URL
    /// </summary>
    public async Task<string> GetAuthorizationUrlAsync(string provider, string redirectUrl)
    {
        await Task.CompletedTask;

        // TODO: 根据provider生成对应的OAuth授权URL
        var clientId = _configuration[$"Authentication:{provider}:ClientId"];
        var authUrl = provider.ToLower() switch
        {
            "wechat" => $"https://open.weixin.qq.com/connect/qrconnect?appid={clientId}&redirect_uri={redirectUrl}",
            "qq" => $"https://graph.qq.com/oauth2.0/authorize?client_id={clientId}&redirect_uri={redirectUrl}",
            "alipay" => $"https://openauth.alipay.com/oauth2/publicAppAuthorize.htm?app_id={clientId}&redirect_uri={redirectUrl}",
            "apple" => $"https://appleid.apple.com/auth/authorize?client_id={clientId}&redirect_uri={redirectUrl}",
            _ => throw new NotSupportedException($"不支持的提供商: {provider}")
        };

        return authUrl;
    }

    /// <summary>
    /// 处理回调
    /// </summary>
    public async Task<ThirdPartyLoginResult> CallbackAsync(string provider, string code, string state)
    {
        // TODO: 1. 用code换取access_token
        // TODO: 2. 获取第三方用户信息
        var thirdPartyUser = await GetThirdPartyUserInfoAsync(provider, code);

        // 3. 查找是否已绑定系统账号
        var thirdPartyAccount = await _thirdPartyAccountRepository.FirstOrDefaultAsync(
            x => x.Provider == provider && x.OpenId == thirdPartyUser.OpenId);

        if (thirdPartyAccount != null)
        {
            // 已绑定,直接登录
            var user = await _userRepository.GetAsync(thirdPartyAccount.UserId);
            return new ThirdPartyLoginResult
            {
                IsSuccess = true,
                User = user,
                NeedBind = false
            };
        }
        else
        {
            // 未绑定,返回第三方用户信息
            return new ThirdPartyLoginResult
            {
                IsSuccess = false,
                NeedBind = true,
                ThirdPartyUser = thirdPartyUser,
                TempToken = GenerateTempToken(thirdPartyUser)
            };
        }
    }

    /// <summary>
    /// 绑定账号
    /// </summary>
    public async Task<bool> BindAccountAsync(Guid userId, string provider, string tempToken)
    {
        // 验证临时token
        var thirdPartyUser = ValidateTempToken(tempToken);
        if (thirdPartyUser == null)
        {
            return false;
        }

        // 创建绑定关系
        var account = new UserThirdPartyAccount(
            SnowflakeIdGenerator.NextId(),
            userId,
            provider,
            thirdPartyUser.OpenId)
        {
            UnionId = thirdPartyUser.UnionId,
            NickName = thirdPartyUser.NickName,
            Avatar = thirdPartyUser.Avatar,
            AccessToken = thirdPartyUser.AccessToken,
            RefreshToken = thirdPartyUser.RefreshToken,
            TokenExpireTime = thirdPartyUser.TokenExpireTime
        };

        await _thirdPartyAccountRepository.InsertAsync(account);
        return true;
    }

    /// <summary>
    /// 解绑账号
    /// </summary>
    public async Task<bool> UnbindAccountAsync(Guid userId, string provider)
    {
        await _thirdPartyAccountRepository.DeleteAsync(
            x => x.UserId == userId && x.Provider == provider);
        return true;
    }

    /// <summary>
    /// 获取第三方用户信息
    /// </summary>
    private async Task<ThirdPartyUserInfo> GetThirdPartyUserInfoAsync(string provider, string code)
    {
        // TODO: 实际调用第三方API获取用户信息
        await Task.CompletedTask;

        return new ThirdPartyUserInfo
        {
            Provider = provider,
            OpenId = $"mock_openid_{code}",
            NickName = "测试用户",
            Avatar = "https://example.com/avatar.jpg"
        };
    }

    /// <summary>
    /// 生成临时Token
    /// </summary>
    private string GenerateTempToken(ThirdPartyUserInfo userInfo)
    {
        // TODO: 使用JWT或其他方式生成临时token
        var data = $"{userInfo.Provider}|{userInfo.OpenId}|{DateTime.Now.Ticks}";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
    }

    /// <summary>
    /// 验证临时Token
    /// </summary>
    private ThirdPartyUserInfo? ValidateTempToken(string tempToken)
    {
        try
        {
            // TODO: 实际验证JWT token
            var data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(tempToken));
            var parts = data.Split('|');

            return new ThirdPartyUserInfo
            {
                Provider = parts[0],
                OpenId = parts[1]
            };
        }
        catch
        {
            return null;
        }
    }
}

