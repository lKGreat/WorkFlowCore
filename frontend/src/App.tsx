import { lazy, Suspense } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Spin } from 'antd';
import AuthGuard from './components/AuthGuard';
import { ErrorBoundary } from './components/ErrorBoundary';
import './App.css';

// 静态导入（首屏必需）
import { Layout } from './components/Layout';
import LoginPage from './pages/Auth/Login';

// 懒加载（按需加载）
const ThirdPartyBind = lazy(() => import('./pages/Auth/ThirdPartyBind'));
const ProcessDesigner = lazy(() => import('./components/ProcessDesigner').then(m => ({ default: m.ProcessDesigner })));
const ProcessDefinitionList = lazy(() => import('./pages/ProcessDefinitionList').then(m => ({ default: m.ProcessDefinitionList })));
const ProcessInstanceList = lazy(() => import('./pages/ProcessInstanceList').then(m => ({ default: m.ProcessInstanceList })));
const VersionHistory = lazy(() => import('./pages/VersionHistory').then(m => ({ default: m.VersionHistory })));
const FileUploadDemo = lazy(() => import('./pages/FileUploadDemo').then(m => ({ default: m.FileUploadDemo })));
const UserManagement = lazy(() => import('./pages/System/User'));
const RoleManagement = lazy(() => import('./pages/System/Role'));

// 全局加载占位符
const PageLoading = () => (
  <div style={{ 
    display: 'flex', 
    justifyContent: 'center', 
    alignItems: 'center', 
    height: '100vh' 
  }}>
    <Spin size="large" tip="加载中..." />
  </div>
);

function App() {
  return (
    <ErrorBoundary>
      <BrowserRouter>
        <Suspense fallback={<PageLoading />}>
          <Routes>
            {/* 登录相关路由（无布局） */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/auth/bind" element={<ThirdPartyBind />} />

            {/* 受保护路由 */}
            <Route element={<AuthGuard />}>
              {/* 带布局的路由（受保护） */}
              <Route path="/" element={<Layout />}>
                <Route index element={<ProcessDefinitionList />} />
                <Route path="instances" element={<ProcessInstanceList />} />
                <Route path="versions/:key" element={<VersionHistory />} />
                <Route path="file-upload" element={<FileUploadDemo />} />
                <Route path="system/user" element={<UserManagement />} />
                <Route path="system/role" element={<RoleManagement />} />
              </Route>

              {/* 全屏流程设计器（无布局，受保护） */}
              <Route path="designer" element={<ProcessDesigner mode="create" />} />
              <Route path="designer/:id" element={<ProcessDesigner mode="edit" />} />
            </Route>

            {/* 404重定向 */}
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </Suspense>
      </BrowserRouter>
    </ErrorBoundary>
  );
}

export default App;
