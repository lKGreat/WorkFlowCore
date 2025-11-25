import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type { LoginLogDto, LoginLogQueryDto } from '../types';

const BASE_URL = '/api/monitor/logininfor';

/**
 * 登录日志服务
 */
export const loginLogService = {
  /**
   * 获取登录日志列表（分页）
   */
  async getPagedList(query: LoginLogQueryDto): Promise<PagedResponse<LoginLogDto>> {
    return request<PagedResponse<LoginLogDto>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params: query,
    });
  },

  /**
   * 删除登录日志
   */
  async delete(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${ids.join(',')}`,
    });
  },

  /**
   * 清空登录日志
   */
  async clean(): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/clean`,
    });
  },
};

