import { api } from './apiClient';
import type { WorkflowInstance } from '../types/processDefinition.types';

/**
 * 工作流服务
 */
export const workflowService = {
  /**
   * 启动工作流
   */
  async startWorkflow(
    workflowId: string,
    version?: number,
    data?: Record<string, any>,
    reference?: string
  ): Promise<string> {
    const response = await api.post<string>('/workflow/start', {
      workflowId,
      version,
      data,
      reference,
    });
    if (!response.success || !response.data) {
      throw new Error(response.message || '启动工作流失败');
    }
    return response.data;
  },

  /**
   * 获取工作流实例
   */
  async getWorkflowInstance(id: string): Promise<WorkflowInstance> {
    const response = await api.get<WorkflowInstance>(`/workflow/instance/${id}`);
    if (!response.success || !response.data) {
      throw new Error(response.message || '获取工作流实例失败');
    }
    return response.data;
  },

  /**
   * 暂停工作流
   */
  async suspendWorkflow(id: string): Promise<void> {
    const response = await api.post(`/workflow/suspend/${id}`);
    if (!response.success) {
      throw new Error(response.message || '暂停工作流失败');
    }
  },

  /**
   * 恢复工作流
   */
  async resumeWorkflow(id: string): Promise<void> {
    const response = await api.post(`/workflow/resume/${id}`);
    if (!response.success) {
      throw new Error(response.message || '恢复工作流失败');
    }
  },

  /**
   * 终止工作流
   */
  async terminateWorkflow(id: string): Promise<void> {
    const response = await api.post(`/workflow/terminate/${id}`);
    if (!response.success) {
      throw new Error(response.message || '终止工作流失败');
    }
  },

  /**
   * 完成审批任务
   */
  async completeTask(
    workflowId: string,
    stepId: string,
    approved: boolean,
    comment?: string
  ): Promise<void> {
    const response = await api.post('/workflow/complete-task', {
      workflowId,
      stepId,
      approved,
      comment,
    });
    if (!response.success) {
      throw new Error(response.message || '完成任务失败');
    }
  },
};

