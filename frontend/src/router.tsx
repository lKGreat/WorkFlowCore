import { lazy } from 'react';
import { Navigate } from 'react-router-dom';
import type { RouteObject } from 'react-router-dom';
import AuthGuard from './components/AuthGuard';
import { Layout } from './components/Layout';

// 静态导入（首屏必需）
import LoginPage from './features/auth/pages/Login';

// 懒加载认证模块
const ThirdPartyBind = lazy(() => import('./features/auth/pages/ThirdPartyBind'));

// 懒加载工作流模块
const ProcessDesigner = lazy(() => 
  import('./features/workflow/components/ProcessDesigner').then(m => ({ default: m.ProcessDesigner }))
);
const ProcessDefinitionList = lazy(() => 
  import('./features/workflow/pages/ProcessDefinitionList').then(m => ({ default: m.ProcessDefinitionList }))
);
const ProcessInstanceList = lazy(() => 
  import('./features/workflow/pages/ProcessInstanceList').then(m => ({ default: m.ProcessInstanceList }))
);
const VersionHistory = lazy(() => 
  import('./features/workflow/pages/VersionHistory').then(m => ({ default: m.VersionHistory }))
);

// 懒加载系统管理模块
const UserManagement = lazy(() => import('./features/system/pages/User'));
const RoleManagement = lazy(() => import('./features/system/pages/Role'));
const DepartmentManagement = lazy(() => import('./features/system/pages/Department'));
const PostManagement = lazy(() => import('./features/system/pages/Post'));
const MenuManagement = lazy(() => import('./features/system/pages/Menu'));
const DictManagement = lazy(() => import('./features/system/pages/Dict'));
const ConfigManagement = lazy(() => import('./features/system/pages/Config'));
const NoticeManagement = lazy(() => import('./features/system/pages/Notice'));

// 懒加载监控管理模块
const LoginLogManagement = lazy(() => import('./features/monitor/pages/LoginLog'));
const OperationLogManagement = lazy(() => import('./features/monitor/pages/OperationLog'));
const OnlineUserManagement = lazy(() => import('./features/monitor/pages/OnlineUser'));
const ServerMonitor = lazy(() => import('./features/monitor/pages/Server'));
const TaskManagement = lazy(() => import('./features/monitor/pages/Task'));

// 懒加载文件管理模块
const FileUploadDemo = lazy(() => 
  import('./features/file/pages/FileUploadDemo').then(m => ({ default: m.FileUploadDemo }))
);

/**
 * 路由配置
 */
export const routes: RouteObject[] = [
  // 登录相关路由（无布局）
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/auth/bind',
    element: <ThirdPartyBind />
  },

  // 受保护路由
  {
    element: <AuthGuard />,
    children: [
      // 带布局的路由（受保护）
      {
        path: '/',
        element: <Layout />,
        children: [
          {
            index: true,
            element: <ProcessDefinitionList />
          },
          {
            path: 'instances',
            element: <ProcessInstanceList />
          },
          {
            path: 'versions/:key',
            element: <VersionHistory />
          },
          {
            path: 'file-upload',
            element: <FileUploadDemo />
          },
          // 系统管理
          {
            path: 'system/user',
            element: <UserManagement />
          },
          {
            path: 'system/role',
            element: <RoleManagement />
          },
          {
            path: 'system/dept',
            element: <DepartmentManagement />
          },
          {
            path: 'system/post',
            element: <PostManagement />
          },
          {
            path: 'system/menu',
            element: <MenuManagement />
          },
          {
            path: 'system/dict',
            element: <DictManagement />
          },
          {
            path: 'system/config',
            element: <ConfigManagement />
          },
          {
            path: 'system/notice',
            element: <NoticeManagement />
          },
          // 监控管理
          {
            path: 'monitor/logininfor',
            element: <LoginLogManagement />
          },
          {
            path: 'monitor/operlog',
            element: <OperationLogManagement />
          },
          {
            path: 'monitor/online',
            element: <OnlineUserManagement />
          },
          {
            path: 'monitor/server',
            element: <ServerMonitor />
          },
          {
            path: 'monitor/job',
            element: <TaskManagement />
          }
        ]
      },

      // 全屏流程设计器（无布局，受保护）
      {
        path: 'designer',
        element: <ProcessDesigner mode="create" />
      },
      {
        path: 'designer/:id',
        element: <ProcessDesigner mode="edit" />
      }
    ]
  },

  // 404 重定向
  {
    path: '*',
    element: <Navigate to="/login" replace />
  }
];

