using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using WorkFlowCore.Domain.Identity;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 用户第三方账号绑定实体
/// </summary>
public class UserThirdPartyAccount : Entity<long>, IMultiTenant
{
    /// <summary>
    /// 用户ID (ABP Identity User 使用 Guid)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 第三方提供商 (WeChat, QQ, Alipay, Apple)
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// OpenId
    /// </summary>
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// UnionId (微信)
    /// </summary>
    public string? UnionId { get; set; }

    /// <summary>
    /// 第三方昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 第三方头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// AccessToken
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// RefreshToken
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Token过期时间
    /// </summary>
    public DateTime? TokenExpireTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 用户导航属性
    /// </summary>
    public virtual AppUser? User { get; set; }

    protected UserThirdPartyAccount()
    {
    }

    public UserThirdPartyAccount(
        long id,
        Guid userId,
        string provider,
        string openId)
    {
        Id = id;
        UserId = userId;
        Provider = provider;
        OpenId = openId;
        CreatedTime = DateTime.Now;
    }
}

