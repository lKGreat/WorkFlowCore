using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 菜单类型 (M=目录, C=菜单, F=按钮)
    /// </summary>
    public string MenuType { get; set; } = "C";

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 权限代码
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 是否外链
    /// </summary>
    public bool IsFrame { get; set; } = false;

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 角色菜单关系集合
    /// </summary>
    public virtual ICollection<RoleMenu>? RoleMenus { get; set; }

    protected Menu()
    {
    }

    public Menu(long id, string menuName, string menuType) : base(id)
    {
        MenuName = menuName;
        MenuType = menuType;
    }
}

