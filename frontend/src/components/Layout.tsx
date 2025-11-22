import React from 'react';
import { Layout as AntLayout, Menu } from 'antd';
import { FileTextOutlined, ApartmentOutlined, CloudUploadOutlined } from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';

const { Header, Sider, Content } = AntLayout;

export const Layout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const menuItems = [
    {
      key: '/',
      icon: <FileTextOutlined />,
      label: '流程定义',
    },
    {
      key: '/instances',
      icon: <ApartmentOutlined />,
      label: '流程实例',
    },
    {
      key: '/file-upload',
      icon: <CloudUploadOutlined />,
      label: '文件上传',
    },
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
      <Header style={{ display: 'flex', alignItems: 'center', padding: '0 24px' }}>
        <div style={{ color: 'white', fontSize: '20px', fontWeight: 'bold' }}>
          WorkFlowCore 审批流系统
        </div>
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

