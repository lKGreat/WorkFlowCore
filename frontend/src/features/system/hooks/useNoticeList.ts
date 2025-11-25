import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { NoticeDto, NoticeQueryDto } from '../types';
import { noticeService } from '../services/noticeService';

export function useNoticeList() {
  const [data, setData] = useState<NoticeDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [searchParams, setSearchParams] = useState<NoticeQueryDto>({});

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await noticeService.getPagedList({
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

  const handleSearch = useCallback((params: NoticeQueryDto) => {
    setSearchParams(params);
    setPagination(prev => ({
      ...prev,
      current: 1,
    }));
  }, []);

  const handleCreate = useCallback(async (values: Partial<NoticeDto>) => {
    try {
      await noticeService.create(values);
      message.success('创建成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '创建失败');
      throw error;
    }
  }, [loadData]);

  const handleUpdate = useCallback(async (values: Partial<NoticeDto>) => {
    try {
      await noticeService.update(values);
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
      await noticeService.delete(ids);
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

