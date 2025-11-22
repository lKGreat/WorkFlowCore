import { api } from './apiClient';
import type {
  ProcessDefinition,
  ProcessDefinitionListItem,
  ProcessDefinitionVersion,
  CreateProcessDefinitionRequest,
  UpdateProcessDefinitionRequest,
  PagedResponse,
} from '../types/processDefinition.types';

/**
 * 流程定义服务
 */
export const processDefinitionService = {
  /**
   * 创建流程定义
   */
  async createProcessDefinition(
    request: CreateProcessDefinitionRequest
  ): Promise<ProcessDefinition> {
    const response = await api.post<ProcessDefinition>(
      '/processdefinitions',
      request
    );
    if (!response.success || !response.data) {
      throw new Error(response.message || '创建流程定义失败');
    }
    return response.data;
  },

  /**
   * 更新流程定义
   */
  async updateProcessDefinition(
    id: string,
    request: UpdateProcessDefinitionRequest,
    createNewVersion: boolean = false
  ): Promise<ProcessDefinition> {
    const response = await api.put<ProcessDefinition>(
      `/processdefinitions/${id}?createNewVersion=${createNewVersion}`,
      request
    );
    if (!response.success || !response.data) {
      throw new Error(response.message || '更新流程定义失败');
    }
    return response.data;
  },

  /**
   * 删除流程定义
   */
  async deleteProcessDefinition(id: string): Promise<void> {
    const response = await api.delete(`/processdefinitions/${id}`);
    if (!response.success) {
      throw new Error(response.message || '删除流程定义失败');
    }
  },

  /**
   * 获取单个流程定义
   */
  async getProcessDefinition(id: string): Promise<ProcessDefinition> {
    const response = await api.get<ProcessDefinition>(
      `/processdefinitions/${id}`
    );
    if (!response.success || !response.data) {
      throw new Error(response.message || '获取流程定义失败');
    }
    return response.data;
  },

  /**
   * 获取最新版本的流程定义
   */
  async getLatestVersion(key: string): Promise<ProcessDefinition> {
    const response = await api.get<ProcessDefinition>(
      `/processdefinitions/by-key/${key}/latest`
    );
    if (!response.success || !response.data) {
      throw new Error(response.message || '获取流程定义失败');
    }
    return response.data;
  },

  /**
   * 获取流程定义的版本历史
   */
  async getVersionHistory(key: string): Promise<ProcessDefinitionVersion[]> {
    const response = await api.get<ProcessDefinitionVersion[]>(
      `/processdefinitions/by-key/${key}/versions`
    );
    if (!response.success || !response.data) {
      throw new Error(response.message || '获取版本历史失败');
    }
    return response.data;
  },

  /**
   * 分页查询流程定义
   */
  async getProcessDefinitions(
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<ProcessDefinitionListItem>> {
    const response = await api.get<PagedResponse<ProcessDefinitionListItem>>(
      '/processdefinitions',
      { pageIndex, pageSize }
    );
    if (!response.success || !response.data) {
      throw new Error(response.message || '查询流程定义失败');
    }
    return response.data;
  },
};

