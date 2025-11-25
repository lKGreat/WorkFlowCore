import { httpClient } from '@/api/httpClient';
import type {
  ProcessDefinitionListItem,
  CreateProcessDefinitionRequest,
  UpdateProcessDefinitionRequest,
  VersionHistoryItem,
} from '../types';

const BASE_URL = '/api/ProcessDefinitions';

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
 * 流程定义服务
 */
export const processDefinitionService = {
  /**
   * 获取流程定义列表
   */
  async getProcessDefinitions(
    page: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<ProcessDefinitionListItem>> {
    const response = await httpClient.get<PagedResponse<ProcessDefinitionListItem>>(
      `${BASE_URL}`,
      { params: { page, pageSize } }
    );
    return response.data;
  },

  /**
   * 获取流程定义详情
   */
  async getProcessDefinition(id: string): Promise<ProcessDefinitionListItem> {
    const response = await httpClient.get<ProcessDefinitionListItem>(`${BASE_URL}/${id}`);
    return response.data;
  },

  /**
   * 创建流程定义
   */
  async createProcessDefinition(
    data: CreateProcessDefinitionRequest
  ): Promise<ProcessDefinitionListItem> {
    const response = await httpClient.post<ProcessDefinitionListItem>(BASE_URL, data);
    return response.data;
  },

  /**
   * 更新流程定义
   */
  async updateProcessDefinition(
    id: string,
    data: UpdateProcessDefinitionRequest
  ): Promise<ProcessDefinitionListItem> {
    const response = await httpClient.put<ProcessDefinitionListItem>(
      `${BASE_URL}/${id}`,
      data
    );
    return response.data;
  },

  /**
   * 删除流程定义
   */
  async deleteProcessDefinition(id: string): Promise<void> {
    await httpClient.delete(`${BASE_URL}/${id}`);
  },

  /**
   * 启用流程定义
   */
  async enableProcessDefinition(id: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/enable`);
  },

  /**
   * 禁用流程定义
   */
  async disableProcessDefinition(id: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/disable`);
  },

  /**
   * 获取流程版本历史
   */
  async getVersionHistory(key: string): Promise<VersionHistoryItem[]> {
    const response = await httpClient.get<VersionHistoryItem[]>(
      `${BASE_URL}/${key}/versions`
    );
    return response.data;
  },
};
