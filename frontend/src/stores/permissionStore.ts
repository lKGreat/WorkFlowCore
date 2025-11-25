import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import type { RouteObject } from 'react-router-dom';

/**
 * 路由元数据
 */
export interface RouterMeta {
  title: string;
  icon?: string;
  noCache?: boolean;
  link?: string;
  titleKey?: string;
  isNew?: number;
  iconColor?: string;
  permi?: string;
}

/**
 * 后端路由数据结构
 */
export interface BackendRoute {
  name: string;
  path: string;
  hidden?: boolean;
  redirect?: string;
  component: string;
  alwaysShow?: boolean;
  query?: string;
  meta?: RouterMeta;
  children?: BackendRoute[];
}

/**
 * 权限状态
 */
interface PermissionState {
  // 路由是否已加载
  routesLoaded: boolean;
  // 动态路由列表
  dynamicRoutes: RouteObject[];
  // 后端原始路由数据
  backendRoutes: BackendRoute[];
  // 用户权限列表
  permissions: string[];
  // 用户角色列表
  roles: string[];

  // Actions
  setRoutesLoaded: (loaded: boolean) => void;
  setDynamicRoutes: (routes: RouteObject[]) => void;
  setBackendRoutes: (routes: BackendRoute[]) => void;
  setPermissions: (permissions: string[]) => void;
  setRoles: (roles: string[]) => void;
  clearPermissions: () => void;

  // 权限检查
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasRole: (role: string) => boolean;
  hasAnyRole: (roles: string[]) => boolean;
}

/**
 * 权限Store
 */
export const usePermissionStore = create<PermissionState>()(
  persist(
    (set, get) => ({
      routesLoaded: false,
      dynamicRoutes: [],
      backendRoutes: [],
      permissions: [],
      roles: [],

      setRoutesLoaded: (loaded) => set({ routesLoaded: loaded }),
      
      setDynamicRoutes: (routes) => set({ dynamicRoutes: routes }),
      
      setBackendRoutes: (routes) => set({ backendRoutes: routes }),
      
      setPermissions: (permissions) => set({ permissions }),
      
      setRoles: (roles) => set({ roles }),
      
      clearPermissions: () => set({
        routesLoaded: false,
        dynamicRoutes: [],
        backendRoutes: [],
        permissions: [],
        roles: []
      }),

      hasPermission: (permission) => {
        const { permissions } = get();
        // 超级权限
        if (permissions.includes('*:*:*')) {
          return true;
        }
        return permissions.includes(permission);
      },

      hasAnyPermission: (perms) => {
        const { permissions } = get();
        // 超级权限
        if (permissions.includes('*:*:*')) {
          return true;
        }
        return perms.some(p => permissions.includes(p));
      },

      hasRole: (role) => {
        const { roles } = get();
        // 管理员角色
        if (roles.includes('admin')) {
          return true;
        }
        return roles.includes(role);
      },

      hasAnyRole: (roleList) => {
        const { roles } = get();
        // 管理员角色
        if (roles.includes('admin')) {
          return true;
        }
        return roleList.some(r => roles.includes(r));
      }
    }),
    {
      name: 'permission-storage',
      storage: createJSONStorage(() => sessionStorage), // 使用sessionStorage，刷新清除
      partialize: (state) => ({
        permissions: state.permissions,
        roles: state.roles,
        backendRoutes: state.backendRoutes
      })
    }
  )
);

