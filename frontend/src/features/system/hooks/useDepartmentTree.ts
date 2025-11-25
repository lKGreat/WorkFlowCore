import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { DepartmentDto } from '../types';
import { departmentService } from '../services/departmentService';

export function useDepartmentTree() {
  const [data, setData] = useState<DepartmentDto[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const tree = await departmentService.getTree();
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

  const handleCreate = useCallback(async (values: Partial<DepartmentDto>) => {
    try {
      await departmentService.create(values);
      message.success('创建成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '创建失败');
      throw error;
    }
  }, [loadData]);

  const handleUpdate = useCallback(async (id: number, values: Partial<DepartmentDto>) => {
    try {
      await departmentService.update(id, values);
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
      await departmentService.delete(id);
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

