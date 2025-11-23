import React from 'react';
import { Layout as AntLayout, Menu, Dropdown, Avatar, Space, message } from 'antd';
import { FileTextOutlined, ApartmentOutlined, CloudUploadOutlined, UserOutlined, LogoutOutlined, SettingOutlined } from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { useRouterStore } from '../stores/routerStore';
import { logout } from '../services/authService';
import { removeToken } from '../utils/auth';

const { Header, Sider, Content } = AntLayout;

export const Layout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { userInfo, logout: logoutStore } = useAuthStore();
  const { routes, clearRoutes } = useRouterStore();

  // 动态菜单（从路由配置生成）
  const buildMenuItems = (routerConfigs: any[]): any[] => {
    return routerConfigs
      .filter(r => !r.hidden && r.meta?.title)
      .map(route => {
        const item: any = {
          key: route.path,
          label: route.meta?.title,
          icon: getMenuIcon(route.meta?.icon)
        };

        if (route.children && route.children.length > 0) {
          item.children = buildMenuItems(route.children);
        }

        return item;
      });
  };

  const getMenuIcon = (iconName?: string) => {
    const iconMap: any = {
      'process': <FileTextOutlined />,
      'instance': <ApartmentOutlined />,
      'upload': <CloudUploadOutlined />,
      'user': <UserOutlined />,
      'setting': <SettingOutlined />
    };
    return iconMap[iconName || ''] || <FileTextOutlined />;
  };

  // 使用动态路由或默认菜单
  const menuItems = routes && routes.length > 0 
    ? buildMenuItems(routes)
    : [
        { key: '/', icon: <FileTextOutlined />, label: '流程定义' },
        { key: '/instances', icon: <ApartmentOutlined />, label: '流程实例' },
        { key: '/file-upload', icon: <CloudUploadOutlined />, label: '文件上传' },
        { key: '/system/user', icon: <UserOutlined />, label: '用户管理' },
      ];

  const handleLogout = async () => {
    try {
      await logout();
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

  const userMenuItems = [
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
      type: 'divider' as const
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: '退出登录',
      onClick: handleLogout
    }
  ] as any;

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

