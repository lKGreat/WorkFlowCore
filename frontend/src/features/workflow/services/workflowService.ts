import { request } from '../../../api';
import type { ProcessInstanceListItem as WorkflowInstance } from '../types';

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
    return request<string>({
      method: 'POST',
      url: '/workflow/start',
      data: {
        workflowId,
        version,
        data,
        reference,
      }
    });
  },

  /**
   * 获取工作流实例
   */
  async getWorkflowInstance(id: string): Promise<WorkflowInstance> {
    return request<WorkflowInstance>({
      method: 'GET',
      url: `/workflow/instance/${id}`
    });
  },

  /**
   * 暂停工作流
   */
  async suspendWorkflow(id: string): Promise<void> {
    await request({
      method: 'POST',
      url: `/workflow/suspend/${id}`
    });
  },

  /**
   * 恢复工作流
   */
  async resumeWorkflow(id: string): Promise<void> {
    await request({
      method: 'POST',
      url: `/workflow/resume/${id}`
    });
  },

  /**
   * 终止工作流
   */
  async terminateWorkflow(id: string): Promise<void> {
    await request({
      method: 'POST',
      url: `/workflow/terminate/${id}`
    });
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
    await request({
      method: 'POST',
      url: '/workflow/complete-task',
      data: {
        workflowId,
        stepId,
        approved,
        comment,
      }
    });
  },
};

