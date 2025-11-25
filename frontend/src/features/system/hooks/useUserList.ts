import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { UserListItem } from '../types';
import { userService } from '../services/userService';

export function useUserList() {
  const [data, setData] = useState<UserListItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchText, setSearchText] = useState<string>();
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const response = await userService.getUsers(
        pagination.current,
        pagination.pageSize,
        searchText
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
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pagination.current, pagination.pageSize, searchText]);

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

  const handleSearch = useCallback((value: string) => {
    setSearchText(value);
    setPagination(prev => ({
      ...prev,
      current: 1,
    }));
  }, []);

  const handleDelete = useCallback(async (id: string) => {
    try {
      await userService.deleteUser(id);
      message.success('删除成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '删除失败');
    }
  }, [loadData]);

  const handleToggleStatus = useCallback(async (id: string, status: string) => {
    try {
      // status === '0' 表示正常，切换为禁用；status === '1' 表示禁用，切换为启用
      if (status === '0') {
        await userService.disableUser(id);
        message.success('已禁用');
      } else {
        await userService.enableUser(id);
        message.success('已启用');
      }
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '操作失败');
    }
  }, [loadData]);

  const handleResetPassword = useCallback(async (id: string) => {
    try {
      await userService.resetPassword(id, '123456');
      message.success('密码已重置为 123456');
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '重置密码失败');
    }
  }, []);

  return {
    data,
    loading,
    pagination,
    searchText,
    handlePageChange,
    handleSearch,
    handleDelete,
    handleToggleStatus,
    handleResetPassword,
    reload: loadData,
  };
}
