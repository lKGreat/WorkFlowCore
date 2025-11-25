import { Typography } from 'antd';
import type { ReactNode } from 'react';

const { Title } = Typography;

type PageHeaderProps = {
  title: string;
  subTitle?: string;
  extra?: ReactNode;
};

export function PageHeader({ title, subTitle, extra }: PageHeaderProps) {
  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 24,
      }}
    >
      <div>
        <Title level={3} style={{ margin: 0 }}>
          {title}
        </Title>
        {subTitle && (
          <div style={{ color: '#8c8c8c', marginTop: 8 }}>{subTitle}</div>
        )}
      </div>
      {extra && <div>{extra}</div>}
    </div>
  );
}
