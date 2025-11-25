import React from 'react';
import { Layout as AntLayout, Menu, Dropdown, Avatar, Space, message } from 'antd';
import {
  FileTextOutlined,
  ApartmentOutlined,
  CloudUploadOutlined,
  UserOutlined,
  LogoutOutlined,
  SettingOutlined,
  TeamOutlined,
  SafetyOutlined,
  DeploymentUnitOutlined,
  IdcardOutlined,
  MenuOutlined,
  BookOutlined,
  ToolOutlined,
  BellOutlined,
  LoginOutlined,
  FileSearchOutlined,
  GlobalOutlined,
  DesktopOutlined,
  ScheduleOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import type { MenuProps } from 'antd';
import { useAuthStore } from '../stores/authStore';
import { usePermissionStore } from '../stores/permissionStore';
import { useRouterStore } from '../stores/routerStore';
import { authService } from '../features/auth/services/authService';
import { removeToken } from '../utils/auth';

const { Header, Sider, Content } = AntLayout;

type RouteConfig = {
  path: string;
  hidden?: boolean;
  meta?: {
    title?: string;
    icon?: string;
  };
  children?: RouteConfig[];
};

type MenuItem = Required<MenuProps>['items'][number];

export const Layout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { userInfo, logout: logoutStore } = useAuthStore();
  const { clearPermissions } = usePermissionStore();
  const { routes, clearRoutes } = useRouterStore();

  // 动态菜单（从路由配置生成）
  const buildMenuItems = (routerConfigs: RouteConfig[]): MenuItem[] => {
    return routerConfigs
      .filter(r => !r.hidden && r.meta?.title)
      .map(route => {
        const item: MenuItem = {
          key: route.path,
          label: route.meta?.title,
          icon: getMenuIcon(route.meta?.icon),
          children: route.children && route.children.length > 0 
            ? buildMenuItems(route.children) 
            : undefined
        };
        return item;
      });
  };

  const getMenuIcon = (iconName?: string): React.ReactNode => {
    const iconMap: Record<string, React.ReactNode> = {
      'process': <FileTextOutlined />,
      'instance': <ApartmentOutlined />,
      'upload': <CloudUploadOutlined />,
      'user': <UserOutlined />,
      'team': <TeamOutlined />,
      'safety': <SafetyOutlined />,
      'deployment': <DeploymentUnitOutlined />,
      'idcard': <IdcardOutlined />,
      'menu': <MenuOutlined />,
      'book': <BookOutlined />,
      'tool': <ToolOutlined />,
      'bell': <BellOutlined />,
      'login': <LoginOutlined />,
      'search': <FileSearchOutlined />,
      'global': <GlobalOutlined />,
      'desktop': <DesktopOutlined />,
      'schedule': <ScheduleOutlined />,
      'setting': <SettingOutlined />
    };
    return iconMap[iconName || ''] || <FileTextOutlined />;
  };

  // 使用动态路由或默认菜单
  const menuItems: MenuItem[] = routes && routes.length > 0 
    ? buildMenuItems(routes as RouteConfig[])
    : [
        {
          key: 'workflow',
          icon: <ApartmentOutlined />,
          label: '流程管理',
          children: [
            { key: '/', icon: <FileTextOutlined />, label: '流程定义' },
            { key: '/instances', icon: <ApartmentOutlined />, label: '流程实例' },
          ],
        },
        {
          key: 'system',
          icon: <SettingOutlined />,
          label: '系统管理',
          children: [
            { key: '/system/user', icon: <UserOutlined />, label: '用户管理' },
            { key: '/system/role', icon: <SafetyOutlined />, label: '角色管理' },
            { key: '/system/dept', icon: <DeploymentUnitOutlined />, label: '部门管理' },
            { key: '/system/post', icon: <IdcardOutlined />, label: '岗位管理' },
            { key: '/system/menu', icon: <MenuOutlined />, label: '菜单管理' },
            { key: '/system/dict', icon: <BookOutlined />, label: '字典管理' },
            { key: '/system/config', icon: <ToolOutlined />, label: '参数设置' },
            { key: '/system/notice', icon: <BellOutlined />, label: '通知公告' },
          ],
        },
        {
          key: 'monitor',
          icon: <DesktopOutlined />,
          label: '系统监控',
          children: [
            { key: '/monitor/logininfor', icon: <LoginOutlined />, label: '登录日志' },
            { key: '/monitor/operlog', icon: <FileSearchOutlined />, label: '操作日志' },
            { key: '/monitor/online', icon: <GlobalOutlined />, label: '在线用户' },
            { key: '/monitor/server', icon: <DesktopOutlined />, label: '服务监控' },
            { key: '/monitor/job', icon: <ScheduleOutlined />, label: '定时任务' },
          ],
        },
        { key: '/file-upload', icon: <CloudUploadOutlined />, label: '文件上传' },
      ];

  const handleLogout = async () => {
    try {
      await authService.logout();
      removeToken();
      logoutStore();
      clearPermissions();
      clearRoutes();
      message.success('已退出登录');
      navigate('/login');
    } catch (error) {
      console.error('退出登录失败:', error);
      // 即使失败也清除本地状态
      removeToken();
      logoutStore();
      clearPermissions();
      clearRoutes();
      navigate('/login');
    }
  };

  const userMenuItems: MenuProps['items'] = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: '个人中心',
      onClick: () => navigate('/user/profile')
    },
    {
      key: 'settings',
      icon: <SettingOutlined />,
      label: '账号设置',
      onClick: () => navigate('/user/settings')
    },
    {
      type: 'divider'
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: '退出登录',
      onClick: handleLogout
    }
  ];

  const handleMenuClick = (key: string) => {
    navigate(key);
  };

  // 获取当前选中的菜单项
  const getSelectedKey = () => {
    if (location.pathname.startsWith('/instances')) {
      return '/instances';
    }
    if (location.pathname.startsWith('/file-upload')) {
      return '/file-upload';
    }
    if (location.pathname.startsWith('/designer') || location.pathname.startsWith('/versions')) {
      return '/';
    }
    return location.pathname;
  };

  return (
    <AntLayout style={{ minHeight: '100vh' }}>
      <Header style={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'space-between',
        padding: '0 24px' 
      }}>
        <div style={{ color: 'white', fontSize: '20px', fontWeight: 'bold' }}>
          WorkFlowCore 审批流系统
        </div>
        
        <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
          <Space style={{ cursor: 'pointer', color: 'white' }}>
            <Avatar 
              size="small" 
              src={userInfo?.avatar} 
              icon={<UserOutlined />}
            />
            <span>{userInfo?.nickName || userInfo?.userName || '用户'}</span>
          </Space>
        </Dropdown>
      </Header>
      <AntLayout>
        <Sider width={200} style={{ background: '#fff' }}>
          <Menu
            mode="inline"
            selectedKeys={[getSelectedKey()]}
            style={{ height: '100%', borderRight: 0 }}
            items={menuItems}
            onClick={({ key }) => handleMenuClick(key)}
          />
        </Sider>
        <AntLayout style={{ padding: '0' }}>
          <Content
            style={{
              background: '#fff',
              margin: 0,
              minHeight: 280,
            }}
          >
            <Outlet />
          </Content>
        </AntLayout>
      </AntLayout>
    </AntLayout>
  );
};
