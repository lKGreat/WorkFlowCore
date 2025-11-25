import { Modal, ModalFuncProps } from 'antd';
import { ExclamationCircleOutlined } from '@ant-design/icons';

type ConfirmOptions = {
  title: string;
  content?: string;
  onOk: () => void | Promise<void>;
  onCancel?: () => void;
  okText?: string;
  cancelText?: string;
  okType?: 'primary' | 'danger' | 'dashed' | 'link' | 'text' | 'default';
  type?: 'confirm' | 'info' | 'success' | 'error' | 'warning';
};

/**
 * 确认弹窗工具函数
 */
export const showConfirm = ({
  title,
  content,
  onOk,
  onCancel,
  okText = '确定',
  cancelText = '取消',
  okType = 'primary',
  type = 'confirm'
}: ConfirmOptions) => {
  const config: ModalFuncProps = {
    title,
    content,
    okText,
    cancelText,
    okType,
    icon: <ExclamationCircleOutlined />,
    onOk: async () => {
      await onOk();
    },
    onCancel
  };

  switch (type) {
    case 'info':
      return Modal.info(config);
    case 'success':
      return Modal.success(config);
    case 'error':
      return Modal.error(config);
    case 'warning':
      return Modal.warning(config);
    default:
      return Modal.confirm(config);
  }
};

/**
 * 删除确认弹窗
 */
export const showDeleteConfirm = (onOk: () => void | Promise<void>, itemName = '该项') => {
  return showConfirm({
    title: '确认删除',
    content: `确定要删除${itemName}吗？此操作不可恢复。`,
    onOk,
    okType: 'danger',
    type: 'warning'
  });
};

/**
 * 启用/禁用确认弹窗
 */
export const showEnableConfirm = (
  enabled: boolean,
  onOk: () => void | Promise<void>,
  itemName = '该项'
) => {
  return showConfirm({
    title: enabled ? '确认禁用' : '确认启用',
    content: `确定要${enabled ? '禁用' : '启用'}${itemName}吗？`,
    onOk,
    okType: 'primary'
  });
};

