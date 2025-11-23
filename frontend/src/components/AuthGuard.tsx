import React, { useEffect, useState } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuthStore } from '../stores/authStore';
import { useRouterStore } from '../stores/routerStore';
import { getToken } from '../utils/auth';
import { getInfo, getRouters } from '../services/authService';

// #region agent log
const agentDebugLog = (
  hypothesisId: string,
  location: string,
  message: string,
  data?: Record<string, unknown>,
) => {
  fetch('http://127.0.0.1:7242/ingest/1d9ea195-40d1-47e5-a8cf-0285be79d950', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      sessionId: 'debug-session',
      runId: 'run1',
      hypothesisId,
      location,
      message,
      data,
      timestamp: Date.now(),
    }),
  }).catch(() => {});
};
// #endregion

/**
 * 路由守卫组件
 */
const AuthGuard: React.FC = () => {
  const location = useLocation();
  const [loading, setLoading] = useState(true);
  const [authenticated, setAuthenticated] = useState(false);
  
  const { token, roles, setUserInfo, setRoles, setPermissions } = useAuthStore();
  const { setRoutes } = useRouterStore();

  // 白名单路由(不需要登录)
  const whiteList = ['/login', '/auth/bind', '/register'];

  useEffect(() => {
    checkAuth();
  }, [location.pathname]);

  const checkAuth = async () => {
    const currentToken = token || getToken();
    agentDebugLog('H6', 'AuthGuard.checkAuth', 'auth check triggered', {
      pathname: location.pathname,
      hasToken: Boolean(currentToken),
      rolesCount: roles.length,
    });

    // 无Token且不在白名单
    if (!currentToken) {
      if (whiteList.includes(location.pathname)) {
        setAuthenticated(true);
        setLoading(false);
      } else {
        setAuthenticated(false);
        setLoading(false);
      }
      return;
    }

    // 有Token但未加载用户信息
    if (roles.length === 0) {
      try {
        // 获取用户信息
        const userInfo = await getInfo();
        setUserInfo(userInfo.user);
        setRoles(userInfo.roles);
        setPermissions(userInfo.permissions);

        // 获取动态路由
        const routers = await getRouters();
        setRoutes(routers);

        setAuthenticated(true);
        agentDebugLog('H6', 'AuthGuard.checkAuth', 'user info & routes loaded', {
          roles: userInfo.roles?.length ?? 0,
          permissions: userInfo.permissions?.length ?? 0,
        });
      } catch (error) {
        console.error('获取用户信息失败:', error);
        setAuthenticated(false);
        agentDebugLog('H6', 'AuthGuard.checkAuth', 'failed to load user info', {
          error: (error as Error).message,
        });
      } finally {
        setLoading(false);
      }
    } else {
      setAuthenticated(true);
      setLoading(false);
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

