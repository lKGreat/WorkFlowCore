import { usePermissionStore } from '../stores/permissionStore';

/**
 * 权限检查Hook
 */
export function usePermission() {
  const {
    permissions,
    roles,
    hasPermission,
    hasAnyPermission,
    hasRole,
    hasAnyRole
  } = usePermissionStore();

  /**
   * 检查是否有指定权限
   * @param permission 权限代码，如 "system:user:add"
   */
  const checkPermission = (permission: string | string[]): boolean => {
    if (Array.isArray(permission)) {
      return hasAnyPermission(permission);
    }
    return hasPermission(permission);
  };

  /**
   * 检查是否有所有指定权限
   * @param permissions 权限代码数组
   */
  const checkAllPermissions = (perms: string[]): boolean => {
    return perms.every(p => hasPermission(p));
  };

  /**
   * 检查是否有指定角色
   * @param role 角色标识，如 "admin"
   */
  const checkRole = (role: string | string[]): boolean => {
    if (Array.isArray(role)) {
      return hasAnyRole(role);
    }
    return hasRole(role);
  };

  /**
   * 检查是否有所有指定角色
   * @param roles 角色标识数组
   */
  const checkAllRoles = (roleList: string[]): boolean => {
    return roleList.every(r => hasRole(r));
  };

  return {
    permissions,
    roles,
    checkPermission,
    checkAllPermissions,
    checkRole,
    checkAllRoles,
    // 便捷方法
    hasPermission: checkPermission,
    hasRole: checkRole
  };
}
