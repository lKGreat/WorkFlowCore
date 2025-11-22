using System;
using Volo.Abp.Identity;

namespace WorkFlowCore.Domain.Identity;

/// <summary>
/// 扩展的应用用户实体
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>
    /// 昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 所属部门ID
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 直属上级ID
    /// </summary>
    public long? ManagerId { get; set; }

    /// <summary>
    /// 登录失败次数
    /// </summary>
    public int LoginFailCount { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 用户状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 部门导航属性
    /// </summary>
    public virtual Entities.Department? Department { get; set; }

    /// <summary>
    /// 第三方账号集合
    /// </summary>
    public virtual ICollection<Entities.UserThirdPartyAccount>? ThirdPartyAccounts { get; set; }

    protected AppUser()
    {
    }

    public AppUser(Guid id, string userName, string? email = null, Guid? tenantId = null) 
        : base(id, userName, email, tenantId)
    {
    }
}

