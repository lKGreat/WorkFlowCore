import { Modal } from 'antd';
import { ExclamationCircleOutlined } from '@ant-design/icons';

/**
 * 显示删除确认弹窗
 */
export function showDeleteConfirm(
  onConfirm: () => void | Promise<void>,
  itemName?: string
) {
  Modal.confirm({
    title: '确认删除',
    icon: <ExclamationCircleOutlined />,
    content: itemName ? `确定要删除 ${itemName} 吗？` : '确定要删除吗？',
    okText: '确定',
    okType: 'danger',
    cancelText: '取消',
    onOk: async () => {
      await onConfirm();
    },
  });
}

/**
 * 显示通用确认弹窗
 */
export function showConfirm(
  title: string,
  content: string,
  onConfirm: () => void | Promise<void>
) {
  Modal.confirm({
    title,
    icon: <ExclamationCircleOutlined />,
    content,
    okText: '确定',
    cancelText: '取消',
    onOk: async () => {
      await onConfirm();
    },
  });
}
