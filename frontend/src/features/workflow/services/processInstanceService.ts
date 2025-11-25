import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type { ProcessInstanceListItem } from '../types';

const BASE_URL = '/api/ProcessInstances';

/**
 * 流程实例服务
 */
export const processInstanceService = {
  /**
   * 获取流程实例列表
   */
  async getProcessInstances(
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<ProcessInstanceListItem>> {
    return request<PagedResponse<ProcessInstanceListItem>>({
      method: 'GET',
      url: BASE_URL,
      params: { pageIndex, pageSize },
    });
  },

  /**
   * 获取流程实例详情
   */
  async getProcessInstance(id: string): Promise<ProcessInstanceListItem> {
    return request<ProcessInstanceListItem>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 挂起流程实例
   */
  async suspendProcessInstance(id: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/${id}/suspend`,
    });
  },

  /**
   * 恢复流程实例
   */
  async resumeProcessInstance(id: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/${id}/resume`,
    });
  },

  /**
   * 终止流程实例
   */
  async terminateProcessInstance(id: string, reason?: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/${id}/terminate`,
      data: { reason },
    });
  },
};
