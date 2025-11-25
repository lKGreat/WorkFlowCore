import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { ConfigDto } from '../types';
import { configService } from '../services/configService';

export function useConfigList() {
  const [data, setData] = useState<ConfigDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await configService.getPagedList({
        pageIndex: pagination.current,
        pageSize: pagination.pageSize,
      });
      
      setData(response.items);
      setPagination(prev => ({
        ...prev,
        total: response.totalCount,
      }));
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '加载数据失败');
    } finally {
      setLoading(false);
    }
  }, [pagination.current, pagination.pageSize]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handlePageChange = useCallback((page: number, pageSize: number) => {
    setPagination(prev => ({
      ...prev,
      current: page,
      pageSize,
    }));
  }, []);

  const handleCreate = useCallback(async (values: Partial<ConfigDto>) => {
    try {
      await configService.create(values);
      message.success('创建成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '创建失败');
      throw error;
    }
  }, [loadData]);

  const handleUpdate = useCallback(async (id: number, values: Partial<ConfigDto>) => {
    try {
      await configService.update(id, values);
      message.success('更新成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '更新失败');
      throw error;
    }
  }, [loadData]);

  const handleDelete = useCallback(async (ids: number[]) => {
    try {
      await configService.delete(ids);
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
    pagination,
    handlePageChange,
    handleCreate,
    handleUpdate,
    handleDelete,
    reload: loadData,
  };
}

