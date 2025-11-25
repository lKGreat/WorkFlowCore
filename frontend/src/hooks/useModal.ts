import { useState, useCallback } from 'react';
import { Modal } from 'antd';
import type { ModalFuncProps } from 'antd';

type UseModalResult = {
  visible: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
  confirm: (options: ModalFuncProps) => void;
};

/**
 * 弹窗通用逻辑 Hook
 */
export function useModal(): UseModalResult {
  const [visible, setVisible] = useState(false);

  const open = useCallback(() => {
    setVisible(true);
  }, []);

  const close = useCallback(() => {
    setVisible(false);
  }, []);

  const toggle = useCallback(() => {
    setVisible((prev) => !prev);
  }, []);

  const confirm = useCallback((options: ModalFuncProps) => {
    Modal.confirm({
      okText: '确定',
      cancelText: '取消',
      ...options,
    });
  }, []);

  return { visible, open, close, toggle, confirm };
}

/**
 * 确认删除弹窗
 */
export function useDeleteConfirm(
  onConfirm: () => void | Promise<void>,
  itemName = '该项'
) {
  const { confirm } = useModal();

  const showDeleteConfirm = useCallback(() => {
    confirm({
      title: '确认删除',
      content: `确定要删除${itemName}吗？此操作不可恢复。`,
      okType: 'danger',
      onOk: async () => {
        await onConfirm();
      },
    });
  }, [confirm, onConfirm, itemName]);

  return showDeleteConfirm;
}

