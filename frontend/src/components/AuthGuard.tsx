import React, { useEffect, useState, useRef } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuthStore } from '../stores/authStore';
import { useRouterStore } from '../stores/routerStore';
import { authService } from '../features/auth/services/authService';

/**
 * 路由守卫组件
 */
const AuthGuard: React.FC = () => {
  const location = useLocation();
  const [loading, setLoading] = useState(true);
  const [authenticated, setAuthenticated] = useState(false);
  const isCheckingRef = useRef(false);
  
  const { token, roles, setUserInfo, setRoles, setPermissions } = useAuthStore();
  const { setRoutes } = useRouterStore();

  // 白名单路由(不需要登录)
  const whiteList = ['/login', '/auth/bind', '/register'];

  useEffect(() => {
    checkAuth();
  }, [token]); // 仅依赖 token 变化

  const checkAuth = async () => {
    // 防止重复请求
    if (isCheckingRef.current) {
      return;
    }

    // 无Token
    if (!token) {
      setAuthenticated(false);
      setLoading(false);
      return;
    }

    // 有Token且已加载用户信息
    if (roles.length > 0) {
      setAuthenticated(true);
      setLoading(false);
      return;
    }

    // 有Token但未加载用户信息
    isCheckingRef.current = true;
    try {
      // 获取用户信息
      const userInfo = await authService.getCurrentUser();
      // 转换为 authStore 需要的格式
      setUserInfo({
        userId: String(userInfo.id),
        userName: userInfo.username,
        nickName: userInfo.fullName,
        email: userInfo.email,
        status: '0', // TODO: 从后端获取状态
      });
      setRoles(userInfo.roles);  // 从响应中获取角色
      setPermissions(userInfo.permissions);  // 从响应中获取权限

      // TODO: 获取动态路由
      // const routers = await authService.getRouters();
      // setRoutes(routers);
      setRoutes([]);

      setAuthenticated(true);
    } catch (error) {
      console.error('获取用户信息失败:', error);
      setAuthenticated(false);
    } finally {
      setLoading(false);
      isCheckingRef.current = false;
    }
  };

  if (loading) {
    return (
      <div
        style={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh',
        }}
      >
        <Spin size="large" tip="加载中..." spinning>
          <div style={{ minHeight: 80 }} />
        </Spin>
      </div>
    );
  }

  // 已登录访问登录页,重定向到首页
  if (authenticated && location.pathname === '/login') {
    return <Navigate to="/" replace />;
  }

  // 未登录访问受保护页面,重定向到登录页
  if (!authenticated && !whiteList.includes(location.pathname)) {
    return <Navigate to={`/login?redirect=${encodeURIComponent(location.pathname)}`} replace />;
  }

  return <Outlet />;
};

export default AuthGuard;

