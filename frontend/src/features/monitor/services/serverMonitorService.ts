import { request } from '@/api';
import type { ServerInfoDto } from '../types';

const BASE_URL = '/api/monitor/server';

/**
 * 服务器监控服务
 */
export const serverMonitorService = {
  /**
   * 获取服务器信息
   */
  async getServerInfo(): Promise<ServerInfoDto> {
    return request<ServerInfoDto>({
      method: 'GET',
      url: BASE_URL,
    });
  },
};

