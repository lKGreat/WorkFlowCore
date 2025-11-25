import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { MenuDto } from '../types';
import { menuService } from '../services/menuService';

export function useMenuTree() {
  const [data, setData] = useState<MenuDto[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const tree = await menuService.getTree();
      setData(tree);
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '加载数据失败');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleCreate = useCallback(async (values: Partial<MenuDto>) => {
    try {
      await menuService.create(values);
      message.success('创建成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '创建失败');
      throw error;
    }
  }, [loadData]);

  const handleUpdate = useCallback(async (values: Partial<MenuDto>) => {
    try {
      await menuService.update(values);
      message.success('更新成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '更新失败');
      throw error;
    }
  }, [loadData]);

  const handleDelete = useCallback(async (id: number) => {
    try {
      await menuService.delete(id);
      message.success('删除成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '删除失败');
    }
  }, [loadData]);

  return {
    data,
    loading,
    handleCreate,
    handleUpdate,
    handleDelete,
    reload: loadData,
  };
}

