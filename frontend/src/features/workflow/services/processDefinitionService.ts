import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type {
  ProcessDefinitionListItem,
  CreateProcessDefinitionRequest,
  UpdateProcessDefinitionRequest,
  VersionHistoryItem,
} from '../types';

const BASE_URL = '/api/ProcessDefinitions';

/**
 * 流程定义服务
 */
export const processDefinitionService = {
  /**
   * 获取流程定义列表
   */
  async getProcessDefinitions(
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<ProcessDefinitionListItem>> {
    return request<PagedResponse<ProcessDefinitionListItem>>({
      method: 'GET',
      url: BASE_URL,
      params: { pageIndex, pageSize },
    });
  },

  /**
   * 获取流程定义详情
   */
  async getProcessDefinition(id: string): Promise<ProcessDefinitionListItem> {
    return request<ProcessDefinitionListItem>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建流程定义
   */
  async createProcessDefinition(
    data: CreateProcessDefinitionRequest
  ): Promise<ProcessDefinitionListItem> {
    return request<ProcessDefinitionListItem>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新流程定义
   */
  async updateProcessDefinition(
    id: string,
    data: UpdateProcessDefinitionRequest
  ): Promise<ProcessDefinitionListItem> {
    return request<ProcessDefinitionListItem>({
      method: 'PUT',
      url: `${BASE_URL}/${id}`,
      data,
    });
  },

  /**
   * 删除流程定义
   */
  async deleteProcessDefinition(id: string): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 启用流程定义
   */
  async enableProcessDefinition(id: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/${id}/enable`,
    });
  },

  /**
   * 禁用流程定义
   */
  async disableProcessDefinition(id: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/${id}/disable`,
    });
  },

  /**
   * 获取流程版本历史
   */
  async getVersionHistory(key: string): Promise<VersionHistoryItem[]> {
    return request<VersionHistoryItem[]>({
      method: 'GET',
      url: `${BASE_URL}/${key}/versions`,
    });
  },
};
