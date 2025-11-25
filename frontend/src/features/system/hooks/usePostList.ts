import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { PostDto, PostQueryDto } from '../types';
import { postService } from '../services/postService';

export function usePostList() {
  const [data, setData] = useState<PostDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [searchParams, setSearchParams] = useState<PostQueryDto>({});

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await postService.getPagedList({
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

  const handleSearch = useCallback((params: PostQueryDto) => {
    setSearchParams(params);
    setPagination(prev => ({
      ...prev,
      current: 1,
    }));
  }, []);

  const handleCreate = useCallback(async (values: Partial<PostDto>) => {
    try {
      await postService.create(values);
      message.success('创建成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '创建失败');
      throw error;
    }
  }, [loadData]);

  const handleUpdate = useCallback(async (values: Partial<PostDto>) => {
    try {
      await postService.update(values);
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
      await postService.delete(ids);
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

