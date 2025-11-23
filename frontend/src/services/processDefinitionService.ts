import { request } from '../api';
import type {
  ProcessDefinition,
  ProcessDefinitionListItem,
  ProcessDefinitionVersion,
  CreateProcessDefinitionRequest,
  UpdateProcessDefinitionRequest,
} from '../types/processDefinition.types';
import type { PagedResponse } from '../api/types';

/**
 * 流程定义服务
 */
export const processDefinitionService = {
  /**
   * 创建流程定义
   */
  async createProcessDefinition(
    data: CreateProcessDefinitionRequest
  ): Promise<ProcessDefinition> {
    return request<ProcessDefinition>({
      method: 'POST',
      url: '/processdefinitions',
      data
    });
  },

  /**
   * 更新流程定义
   */
  async updateProcessDefinition(
    id: string,
    data: UpdateProcessDefinitionRequest,
    createNewVersion: boolean = false
  ): Promise<ProcessDefinition> {
    return request<ProcessDefinition>({
      method: 'PUT',
      url: `/processdefinitions/${id}?createNewVersion=${createNewVersion}`,
      data
    });
  },

  /**
   * 删除流程定义
   */
  async deleteProcessDefinition(id: string): Promise<void> {
    await request({
      method: 'DELETE',
      url: `/processdefinitions/${id}`
    });
  },

  /**
   * 获取单个流程定义
   */
  async getProcessDefinition(id: string): Promise<ProcessDefinition> {
    return request<ProcessDefinition>({
      method: 'GET',
      url: `/processdefinitions/${id}`
    });
  },

  /**
   * 获取最新版本的流程定义
   */
  async getLatestVersion(key: string): Promise<ProcessDefinition> {
    return request<ProcessDefinition>({
      method: 'GET',
      url: `/processdefinitions/by-key/${key}/latest`
    });
  },

  /**
   * 获取流程定义的版本历史
   */
  async getVersionHistory(key: string): Promise<ProcessDefinitionVersion[]> {
    return request<ProcessDefinitionVersion[]>({
      method: 'GET',
      url: `/processdefinitions/by-key/${key}/versions`
    });
  },

  /**
   * 分页查询流程定义
   */
  async getProcessDefinitions(
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<ProcessDefinitionListItem>> {
    return request<PagedResponse<ProcessDefinitionListItem>>({
      method: 'GET',
      url: '/processdefinitions',
      params: { pageIndex, pageSize }
    });
  },
};

