import { request } from '@/api';
import type { OnlineUserDto } from '../types';

const BASE_URL = '/api/monitor/online';

/**
 * 在线用户服务
 */
export const onlineUserService = {
  /**
   * 获取在线用户列表
   */
  async getList(): Promise<OnlineUserDto[]> {
    return request<OnlineUserDto[]>({
      method: 'GET',
      url: `${BASE_URL}/list`,
    });
  },

  /**
   * 强制用户下线
   */
  async forceLogout(userId: string): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${userId}`,
    });
  },
};

