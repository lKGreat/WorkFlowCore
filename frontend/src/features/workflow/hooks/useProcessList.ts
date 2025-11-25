import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { ProcessDefinitionListItem } from '../types';
import { processDefinitionService } from '../services/processDefinitionService';

export function useProcessList() {
  const [data, setData] = useState<ProcessDefinitionListItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await processDefinitionService.getProcessDefinitions(
        pagination.current,
        pagination.pageSize
      );
      
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

  const handleDelete = useCallback(async (id: number) => {
    try {
      await processDefinitionService.deleteProcessDefinition(String(id));
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
    handleDelete,
    reload: loadData,
  };
}

