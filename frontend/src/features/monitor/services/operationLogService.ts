import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type { OperationLogDto, OperationLogPagedRequest } from '../types';

const BASE_URL = '/api/monitor/operlog';

/**
 * 操作日志服务
 */
export const operationLogService = {
  /**
   * 获取操作日志列表（分页）
   */
  async getPagedList(params: OperationLogPagedRequest): Promise<PagedResponse<OperationLogDto>> {
    return request<PagedResponse<OperationLogDto>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params,
    });
  },

  /**
   * 获取操作日志详情
   */
  async getById(id: number): Promise<OperationLogDto> {
    return request<OperationLogDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 删除操作日志
   */
  async delete(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${ids.join(',')}`,
    });
  },

  /**
   * 清空操作日志
   */
  async clean(): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/clean`,
    });
  },
};

