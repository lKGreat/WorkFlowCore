import { lazy, createElement } from 'react';
import type { RouteObject } from 'react-router-dom';
import { Layout } from '../components/Layout';
import type { BackendRoute } from '../stores/permissionStore';

/**
 * 路由组件缓存
 */
const componentCache = new Map<string, React.LazyExoticComponent<React.ComponentType<any>>>();

/**
 * 懒加载组件
 * @param componentPath 组件路径（如 "system/user/index"）
 */
function loadComponent(componentPath: string) {
  // 检查缓存
  if (componentCache.has(componentPath)) {
    return componentCache.get(componentPath)!;
  }

  // 特殊组件处理
  if (componentPath === 'Layout') {
    return Layout;
  }

  if (componentPath === 'ParentView') {
    // ParentView 是一个占位组件，用于多级菜单
    return lazy(() => import('../components/ParentView'));
  }

  if (componentPath === 'InnerLink') {
    // 内链组件，用于在iframe中显示外部链接
    return lazy(() => import('../components/InnerLink'));
  }

  // 动态加载业务组件
  try {
    const component = lazy(() => import(`../features/${componentPath}.tsx`));
    componentCache.set(componentPath, component);
    return component;
  } catch (error) {
    console.error(`Failed to load component: ${componentPath}`, error);
    // 返回404组件
    return lazy(() => import('../components/NotFound'));
  }
}

/**
 * 转换后端路由数据为前端路由格式
 * @param backendRoutes 后端路由数据
 * @returns React Router 路由配置
 */
export function transformRoutes(backendRoutes: BackendRoute[]): RouteObject[] {
  return backendRoutes.map(route => transformRoute(route)).filter(Boolean) as RouteObject[];
}

/**
 * 转换单个路由
 */
function transformRoute(route: BackendRoute): RouteObject | null {
  if (!route || route.hidden) {
    return null;
  }

  const routeConfig: RouteObject = {
    path: route.path,
    element: undefined,
    children: undefined
  };

  // 设置组件
  if (route.component) {
    const Component = loadComponent(route.component);
    routeConfig.element = createElement(Component);
  }

  // 处理子路由
  if (route.children && route.children.length > 0) {
    const children = route.children
      .map(child => transformRoute(child))
      .filter(Boolean) as RouteObject[];
    
    if (children.length > 0) {
      routeConfig.children = children;
    }
  }

  // 处理重定向
  if (route.redirect && route.redirect !== 'noRedirect') {
    routeConfig.index = false;
    // React Router v6+ 使用 Navigate 组件进行重定向
    // 这里我们只存储重定向路径，实际重定向在路由配置中处理
  }

  // 存储元数据（用于面包屑、标题等）
  if (route.meta) {
    // @ts-ignore - 添加自定义属性
    routeConfig.meta = route.meta;
  }

  return routeConfig;
}

/**
 * 扁平化路由（用于权限匹配）
 * @param routes 路由配置
 * @param parentPath 父路径
 */
export function flattenRoutes(routes: BackendRoute[], parentPath: string = ''): BackendRoute[] {
  const result: BackendRoute[] = [];
  
  routes.forEach(route => {
    const fullPath = parentPath ? `${parentPath}/${route.path}` : route.path;
    const flatRoute = { ...route, path: fullPath };
    result.push(flatRoute);
    
    if (route.children && route.children.length > 0) {
      result.push(...flattenRoutes(route.children, fullPath));
    }
  });
  
  return result;
}

/**
 * 根据权限过滤路由
 * @param routes 路由列表
 * @param permissions 用户权限列表
 */
export function filterRoutesByPermission(routes: BackendRoute[], permissions: string[]): BackendRoute[] {
  // 如果有超级权限，返回所有路由
  if (permissions.includes('*:*:*')) {
    return routes;
  }

  return routes.filter(route => {
    // 检查路由是否需要权限
    if (route.meta?.permi) {
      const hasPermission = permissions.includes(route.meta.permi);
      if (!hasPermission) {
        return false;
      }
    }

    // 递归过滤子路由
    if (route.children && route.children.length > 0) {
      route.children = filterRoutesByPermission(route.children, permissions);
    }

    return true;
  });
}

/**
 * 生成面包屑路径
 * @param path 当前路径
 * @param routes 所有路由
 */
export function generateBreadcrumb(path: string, routes: BackendRoute[]): BackendRoute[] {
  const breadcrumb: BackendRoute[] = [];
  const flatRoutes = flattenRoutes(routes);
  
  // 查找当前路由
  const currentRoute = flatRoutes.find(r => r.path === path);
  if (!currentRoute) {
    return breadcrumb;
  }

  // 递归查找父路由
  const findParents = (route: BackendRoute) => {
    breadcrumb.unshift(route);
    const parentPath = route.path.substring(0, route.path.lastIndexOf('/'));
    if (parentPath) {
      const parent = flatRoutes.find(r => r.path === parentPath);
      if (parent) {
        findParents(parent);
      }
    }
  };

  findParents(currentRoute);
  return breadcrumb;
}

