import { Outlet } from 'react-router-dom';

/**
 * ParentView - 多级菜单占位组件
 * 用于多级菜单结构中的父级菜单，仅渲染子路由
 */
export default function ParentView() {
  return <Outlet />;
}

