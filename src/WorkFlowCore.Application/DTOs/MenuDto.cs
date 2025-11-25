namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 菜单DTO
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; }

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
    /// 路由参数
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    public string IsCache { get; set; } = "0";

    /// <summary>
    /// 菜单名称国际化key
    /// </summary>
    public string? MenuNameKey { get; set; }

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
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 子菜单
    /// </summary>
    public List<MenuDto>? Children { get; set; }
}

/// <summary>
/// 路由元数据
/// </summary>
public class RouterMetaDto
{
    /// <summary>
    /// 路由标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 路由图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 是否不缓存
    /// </summary>
    public bool NoCache { get; set; }

    /// <summary>
    /// 外链地址
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// 国际化标题key
    /// </summary>
    public string? TitleKey { get; set; }

    /// <summary>
    /// 是否新功能（7天内新增）
    /// </summary>
    public int IsNew { get; set; }

    /// <summary>
    /// 图标颜色
    /// </summary>
    public string? IconColor { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Permi { get; set; }
}

/// <summary>
/// 路由DTO
/// </summary>
public class RouterDto
{
    /// <summary>
    /// 路由名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 重定向地址
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 总是显示根路由
    /// </summary>
    public bool? AlwaysShow { get; set; }

    /// <summary>
    /// 路由参数（查询字符串）
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// 路由元信息
    /// </summary>
    public RouterMetaDto? Meta { get; set; }

    /// <summary>
    /// 子路由
    /// </summary>
    public List<RouterDto>? Children { get; set; }
}

