import type { ReactNode } from 'react';
import { usePermission } from '../hooks/usePermission';

/**
 * 权限包装组件Props
 */
interface AuthorizedProps {
  /**
   * 需要的权限（满足任意一个即可）
   */
  permission?: string | string[];

  /**
   * 需要的角色（满足任意一个即可）
   */
  role?: string | string[];

  /**
   * 是否需要所有权限（默认false）
   */
  requireAll?: boolean;

  /**
   * 无权限时显示的内容（默认不显示）
   */
  fallback?: ReactNode;

  /**
   * 子组件
   */
  children: ReactNode;
}

/**
 * 权限控制组件
 * 
 * @example
 * // 单个权限
 * <Authorized permission="system:user:add">
 *   <Button>新增用户</Button>
 * </Authorized>
 * 
 * // 多个权限（满足任意一个）
 * <Authorized permission={["system:user:add", "system:user:edit"]}>
 *   <Button>操作</Button>
 * </Authorized>
 * 
 * // 需要所有权限
 * <Authorized permission={["system:user:add", "system:user:edit"]} requireAll>
 *   <Button>高级操作</Button>
 * </Authorized>
 * 
 * // 角色控制
 * <Authorized role="admin">
 *   <Button>管理员功能</Button>
 * </Authorized>
 * 
 * // 无权限时显示替代内容
 * <Authorized permission="system:user:delete" fallback={<span>无权限</span>}>
 *   <Button danger>删除</Button>
 * </Authorized>
 */
export function Authorized({
  permission,
  role,
  requireAll = false,
  fallback = null,
  children
}: AuthorizedProps) {
  const { checkPermission, checkAllPermissions, checkRole, checkAllRoles } = usePermission();

  let hasAuth = true;

  // 检查权限
  if (permission) {
    if (requireAll && Array.isArray(permission)) {
      hasAuth = checkAllPermissions(permission);
    } else {
      hasAuth = checkPermission(permission);
    }
  }

  // 检查角色
  if (hasAuth && role) {
    if (requireAll && Array.isArray(role)) {
      hasAuth = checkAllRoles(role);
    } else {
      hasAuth = checkRole(role);
    }
  }

  return hasAuth ? <>{children}</> : <>{fallback}</>;
}

/**
 * HOC版本 - 权限控制高阶组件
 * 
 * @example
 * const ProtectedButton = withAuthorized(Button, { permission: "system:user:add" });
 */
export function withAuthorized<P extends object>(
  Component: React.ComponentType<P>,
  authProps: Omit<AuthorizedProps, 'children'>
) {
  return (props: P) => (
    <Authorized {...authProps}>
      <Component {...props} />
    </Authorized>
  );
}

