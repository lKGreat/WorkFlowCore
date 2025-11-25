import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { LoginLogDto, LoginLogQueryDto } from '../types';
import { loginLogService } from '../services/loginLogService';

export function useLoginLogList() {
  const [data, setData] = useState<LoginLogDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [searchParams, setSearchParams] = useState<LoginLogQueryDto>({});

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await loginLogService.getPagedList({
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

  const handleSearch = useCallback((params: LoginLogQueryDto) => {
    setSearchParams(params);
    setPagination(prev => ({
      ...prev,
      current: 1,
    }));
  }, []);

  const handleDelete = useCallback(async (ids: number[]) => {
    try {
      await loginLogService.delete(ids);
      message.success('删除成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '删除失败');
    }
  }, [loadData]);

  const handleClean = useCallback(async () => {
    try {
      await loginLogService.clean();
      message.success('清空成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '清空失败');
    }
  }, [loadData]);

  return {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSearch,
    handleDelete,
    handleClean,
    reload: loadData,
  };
}

