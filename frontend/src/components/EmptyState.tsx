import { Empty, Button } from 'antd';
import type { ReactNode } from 'react';

type EmptyStateProps = {
  description?: string;
  image?: ReactNode;
  action?: ReactNode;
  style?: React.CSSProperties;
};

/**
 * 空状态占位组件
 */
export function EmptyState({
  description = '暂无数据',
  image,
  action,
  style
}: EmptyStateProps) {
  return (
    <div style={{ padding: '50px 0', textAlign: 'center', ...style }}>
      <Empty
        image={image || Empty.PRESENTED_IMAGE_SIMPLE}
        description={description}
      >
        {action}
      </Empty>
    </div>
  );
}

/**
 * 空状态 - 带创建按钮
 */
export function EmptyStateWithAction({
  description = '暂无数据',
  buttonText = '创建',
  onClick
}: {
  description?: string;
  buttonText?: string;
  onClick: () => void;
}) {
  return (
    <EmptyState
      description={description}
      action={
        <Button type="primary" onClick={onClick}>
          {buttonText}
        </Button>
      }
    />
  );
}

