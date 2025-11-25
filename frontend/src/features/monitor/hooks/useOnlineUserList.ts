import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import type { OnlineUserDto } from '../types';
import { onlineUserService } from '../services/onlineUserService';

export function useOnlineUserList() {
  const [data, setData] = useState<OnlineUserDto[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const list = await onlineUserService.getList();
      setData(list);
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

  const handleForceLogout = useCallback(async (userId: string) => {
    try {
      await onlineUserService.forceLogout(userId);
      message.success('强制下线成功');
      loadData();
    } catch (error: unknown) {
      const err = error as Error;
      message.error(err.message || '强制下线失败');
    }
  }, [loadData]);

  return {
    data,
    loading,
    handleForceLogout,
    reload: loadData,
  };
}

