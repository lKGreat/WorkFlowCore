import { Typography, Space, Breadcrumb } from 'antd';
import { HomeOutlined } from '@ant-design/icons';
import type { ReactNode } from 'react';

const { Title } = Typography;

type BreadcrumbItem = {
  title: string;
  path?: string;
};

type PageHeaderProps = {
  title: string;
  subtitle?: string;
  breadcrumbs?: BreadcrumbItem[];
  extra?: ReactNode;
};

/**
 * 页面头部组件
 */
export function PageHeader({ title, subtitle, breadcrumbs, extra }: PageHeaderProps) {
  return (
    <div style={{ marginBottom: 24 }}>
      {breadcrumbs && breadcrumbs.length > 0 && (
        <Breadcrumb style={{ marginBottom: 16 }}>
          <Breadcrumb.Item href="/">
            <HomeOutlined />
          </Breadcrumb.Item>
          {breadcrumbs.map((item, index) => (
            <Breadcrumb.Item key={index} href={item.path}>
              {item.title}
            </Breadcrumb.Item>
          ))}
        </Breadcrumb>
      )}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Space direction="vertical" size={0}>
          <Title level={2} style={{ margin: 0 }}>
            {title}
          </Title>
          {subtitle && (
            <Typography.Text type="secondary">{subtitle}</Typography.Text>
          )}
        </Space>
        {extra && <Space>{extra}</Space>}
      </div>
    </div>
  );
}

