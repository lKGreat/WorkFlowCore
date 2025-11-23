using Volo.Abp.Domain.Entities;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 角色菜单关系实体
/// </summary>
public class RoleMenu : Entity<long>
{
    /// <summary>
    /// 角色ID (ABP Identity Role 使用 Guid)
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; }

    /// <summary>
    /// 菜单导航属性
    /// </summary>
    public virtual Menu? Menu { get; set; }

    protected RoleMenu()
    {
    }

    public RoleMenu(long id, Guid roleId, long menuId) : base(id)
    {
        RoleId = roleId;
        MenuId = menuId;
    }
}

