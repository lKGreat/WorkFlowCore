import { useAuthStore } from '../stores/authStore';

/**
 * 权限控制Hook
 */
export const usePermission = () => {
  const { permissions } = useAuthStore();

  const hasPermission = (permissionCode: string): boolean => {
    if (!permissionCode) return true;
    if (permissions.includes('*:*:*')) return true; // 超级管理员
    return permissions.some(p => 
      p === permissionCode || 
      p === permissionCode.split(':').slice(0, 2).join(':') + ':*' ||
      p.endsWith(':*')
    );
  };

  const hasAnyPermission = (permissionCodes: string[]): boolean => {
    return permissionCodes.some(code => hasPermission(code));
  };

  const hasAllPermissions = (permissionCodes: string[]): boolean => {
    return permissionCodes.every(code => hasPermission(code));
  };

  return {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    permissions
  };
};

