import React from 'react';
import { Layout as AntLayout, Menu, Dropdown, Avatar, Space, message } from 'antd';
import { FileTextOutlined, ApartmentOutlined, CloudUploadOutlined, UserOutlined, LogoutOutlined, SettingOutlined } from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import type { MenuProps } from 'antd';
import { useAuthStore } from '../stores/authStore';
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
      'setting': <SettingOutlined />
    };
    return iconMap[iconName || ''] || <FileTextOutlined />;
  };

  // 使用动态路由或默认菜单
  const menuItems: MenuItem[] = routes && routes.length > 0 
    ? buildMenuItems(routes as RouteConfig[])
    : [
        { key: '/', icon: <FileTextOutlined />, label: '流程定义' },
        { key: '/instances', icon: <ApartmentOutlined />, label: '流程实例' },
        { key: '/file-upload', icon: <CloudUploadOutlined />, label: '文件上传' },
        { key: '/system/user', icon: <UserOutlined />, label: '用户管理' },
      ];

  const handleLogout = async () => {
    try {
      await authService.logout();
      removeToken();
      logoutStore();
      clearRoutes();
      message.success('已退出登录');
      navigate('/login');
    } catch (error) {
      console.error('退出登录失败:', error);
      // 即使失败也清除本地状态
      removeToken();
      logoutStore();
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
