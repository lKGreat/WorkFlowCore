using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Common;

/// <summary>
/// 数据权限帮助类
/// </summary>
public static class DataPermissionHelper
{
    /// <summary>
    /// 检查用户是否有数据权限
    /// </summary>
    /// <param name="dataScope">数据权限范围</param>
    /// <param name="userId">当前用户ID</param>
    /// <param name="userDeptId">当前用户部门ID</param>
    /// <param name="creatorId">数据创建人ID</param>
    /// <param name="dataDeptId">数据所属部门ID</param>
    /// <param name="deptAncestors">部门祖级列表（用于本部门及以下判断）</param>
    /// <returns>是否有权限</returns>
    public static bool HasDataPermission(
        DataScopeType dataScope,
        Guid userId,
        long? userDeptId,
        Guid? creatorId,
        long? dataDeptId,
        string? deptAncestors = null)
    {
        return dataScope switch
        {
            DataScopeType.All => true, // 全部数据权限
            DataScopeType.Self => creatorId == userId, // 仅本人数据
            DataScopeType.Dept => dataDeptId.HasValue && userDeptId.HasValue && dataDeptId == userDeptId, // 本部门数据
            DataScopeType.DeptAndChild => CheckDeptAndChild(userDeptId, dataDeptId, deptAncestors), // 本部门及以下
            DataScopeType.Custom => true, // TODO: 自定义权限需要查询角色部门关联表
            DataScopeType.Project => true, // TODO: 项目权限需要查询用户项目关联表
            _ => false
        };
    }

    /// <summary>
    /// 检查是否是本部门及以下
    /// </summary>
    private static bool CheckDeptAndChild(long? userDeptId, long? dataDeptId, string? deptAncestors)
    {
        if (!userDeptId.HasValue || !dataDeptId.HasValue)
        {
            return false;
        }

        if (userDeptId == dataDeptId)
        {
            return true; // 同一个部门
        }

        if (!string.IsNullOrEmpty(deptAncestors))
        {
            // 检查用户部门ID是否在数据部门的祖级列表中
            var ancestorIds = deptAncestors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToList();
            return ancestorIds.Contains(userDeptId.Value);
        }

        return false;
    }
}

