import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { ProcessInstanceListItem } from '../types';
import { processInstanceService } from '../services/processInstanceService';

export function useProcessInstances() {
  const [data, setData] = useState<ProcessInstanceListItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await processInstanceService.getProcessInstances(
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

  const handleSuspend = useCallback(async (id: number) => {
    try {
      await processInstanceService.suspendProcessInstance(String(id));
      message.success('挂起成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '挂起失败');
    }
  }, [loadData]);

  const handleResume = useCallback(async (id: number) => {
    try {
      await processInstanceService.resumeProcessInstance(String(id));
      message.success('恢复成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '恢复失败');
    }
  }, [loadData]);

  const handleTerminate = useCallback(async (id: number, reason?: string) => {
    try {
      await processInstanceService.terminateProcessInstance(String(id), reason);
      message.success('终止成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '终止失败');
    }
  }, [loadData]);

  return {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSuspend,
    handleResume,
    handleTerminate,
    reload: loadData,
  };
}

