import { httpClient } from '@/api/httpClient';
import type { ProcessInstanceListItem } from '../types';

const BASE_URL = '/api/ProcessInstances';

/**
 * 分页响应
 */
type PagedResponse<T> = {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};

/**
 * 流程实例服务
 */
export const processInstanceService = {
  /**
   * 获取流程实例列表
   */
  async getProcessInstances(
    page: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<ProcessInstanceListItem>> {
    const response = await httpClient.get<PagedResponse<ProcessInstanceListItem>>(
      `${BASE_URL}`,
      { params: { page, pageSize } }
    );
    return response.data;
  },

  /**
   * 获取流程实例详情
   */
  async getProcessInstance(id: string): Promise<ProcessInstanceListItem> {
    const response = await httpClient.get<ProcessInstanceListItem>(`${BASE_URL}/${id}`);
    return response.data;
  },

  /**
   * 挂起流程实例
   */
  async suspendProcessInstance(id: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/suspend`);
  },

  /**
   * 恢复流程实例
   */
  async resumeProcessInstance(id: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/resume`);
  },

  /**
   * 终止流程实例
   */
  async terminateProcessInstance(id: string, reason?: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/terminate`, { reason });
  },
};

