namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 数据权限范围类型
/// </summary>
public enum DataScopeType
{
    /// <summary>
    /// 全部数据权限
    /// </summary>
    All = 1,

    /// <summary>
    /// 自定义数据权限
    /// </summary>
    Custom = 2,

    /// <summary>
    /// 本部门数据权限
    /// </summary>
    Dept = 3,

    /// <summary>
    /// 本部门及以下数据权限
    /// </summary>
    DeptAndChild = 4,

    /// <summary>
    /// 项目数据权限
    /// </summary>
    Project = 5,

    /// <summary>
    /// 仅本人数据权限
    /// </summary>
    Self = 6
}

/// <summary>
/// 数据权限过滤接口
/// </summary>
public interface IDataScopeEntity
{
    /// <summary>
    /// 创建人ID（用于本人权限）
    /// </summary>
    Guid? CreatorId { get; }

    /// <summary>
    /// 部门ID（用于部门权限）
    /// </summary>
    long? DepartmentId { get; }
}

