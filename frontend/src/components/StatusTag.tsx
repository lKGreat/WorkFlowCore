import { Tag } from 'antd';
import type { PresetColorType, PresetStatusColorType } from 'antd/es/_util/colors';

type StatusConfig = {
  text: string;
  color: PresetColorType | PresetStatusColorType;
};

type StatusTagProps = {
  status: string | number | boolean;
  config?: Record<string, StatusConfig>;
};

const defaultStatusConfig: Record<string, StatusConfig> = {
  // 布尔值
  true: { text: '是', color: 'success' },
  false: { text: '否', color: 'default' },

  // 启用状态
  enabled: { text: '启用', color: 'success' },
  disabled: { text: '禁用', color: 'error' },

  // 任务状态
  pending: { text: '待处理', color: 'warning' },
  processing: { text: '处理中', color: 'processing' },
  completed: { text: '已完成', color: 'success' },
  rejected: { text: '已拒绝', color: 'error' },
  cancelled: { text: '已取消', color: 'default' },

  // 审批状态
  approved: { text: '已通过', color: 'success' },
  waiting: { text: '等待中', color: 'warning' },

  // 流程状态
  running: { text: '运行中', color: 'processing' },
  suspended: { text: '已暂停', color: 'warning' },
  terminated: { text: '已终止', color: 'error' }
};

/**
 * 状态标签组件
 */
export function StatusTag({ status, config }: StatusTagProps) {
  const statusKey = String(status).toLowerCase();
  const finalConfig = { ...defaultStatusConfig, ...config };
  const statusConfig = finalConfig[statusKey];

  if (!statusConfig) {
    return <Tag>{status}</Tag>;
  }

  return <Tag color={statusConfig.color}>{statusConfig.text}</Tag>;
}

