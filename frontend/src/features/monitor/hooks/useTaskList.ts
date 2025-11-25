import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { TaskDto, TaskQueryDto } from '../types';
import { taskService } from '../services/taskService';

export function useTaskList() {
  const [data, setData] = useState<TaskDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [searchParams, setSearchParams] = useState<TaskQueryDto>({});

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await taskService.getPagedList({
        pageIndex: pagination.current,
        pageSize: pagination.pageSize,
        ...searchParams,
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
  }, [pagination.current, pagination.pageSize, searchParams]);

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

  const handleSearch = useCallback((params: TaskQueryDto) => {
    setSearchParams(params);
    setPagination(prev => ({
      ...prev,
      current: 1,
    }));
  }, []);

  const handleCreate = useCallback(async (values: Partial<TaskDto>) => {
    try {
      await taskService.create(values);
      message.success('创建成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '创建失败');
      throw error;
    }
  }, [loadData]);

  const handleUpdate = useCallback(async (values: Partial<TaskDto>) => {
    try {
      await taskService.update(values);
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
      await taskService.delete(ids);
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
    handleSearch,
    handleCreate,
    handleUpdate,
    handleDelete,
    reload: loadData,
  };
}

