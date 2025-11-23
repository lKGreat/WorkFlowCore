import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './components/Layout';
import { ProcessDesigner } from './components/ProcessDesigner';
import { ProcessDefinitionList } from './pages/ProcessDefinitionList';
import { VersionHistory } from './pages/VersionHistory';
import { ProcessInstanceList } from './pages/ProcessInstanceList';
import { FileUploadDemo } from './pages/FileUploadDemo';
import LoginPage from './pages/Auth/Login';
import ThirdPartyBind from './pages/Auth/ThirdPartyBind';
import AuthGuard from './components/AuthGuard';
import UserManagement from './pages/System/User';
import RoleManagement from './pages/System/Role';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <AuthGuard>
        <Routes>
          {/* 登录相关路由（无布局） */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/auth/bind" element={<ThirdPartyBind />} />
          
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
          
          {/* 404重定向 */}
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </AuthGuard>
    </BrowserRouter>
  );
}

export default App;
