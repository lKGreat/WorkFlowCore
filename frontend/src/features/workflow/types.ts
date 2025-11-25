/**
 * 流程定义列表项
 */
export type ProcessDefinitionListItem = {
  id: number;
  name: string;
  key: string;
  version: number;
  description?: string;
  isEnabled: boolean;
  createdAt: string;
  updatedAt?: string;
};

/**
 * 流程实例列表项
 */
export type ProcessInstanceListItem = {
  id: number;
  processDefinitionId: number;
  processDefinitionName: string;
  status: ProcessInstanceStatus;
  initiatorId: number;
  initiatorName: string;
  startTime: string;
  endTime?: string;
  currentStepName?: string;
};

/**
 * 流程实例状态
 */
export enum ProcessInstanceStatus {
  Running = 'Running',
  Completed = 'Completed',
  Suspended = 'Suspended',
  Terminated = 'Terminated',
}

/**
 * 流程实例状态文本映射
 */
export const ProcessInstanceStatusText: Record<ProcessInstanceStatus, string> = {
  [ProcessInstanceStatus.Running]: '运行中',
  [ProcessInstanceStatus.Completed]: '已完成',
  [ProcessInstanceStatus.Suspended]: '已挂起',
  [ProcessInstanceStatus.Terminated]: '已终止',
};

/**
 * 流程实例状态颜色映射
 */
export const ProcessInstanceStatusColor: Record<ProcessInstanceStatus, string> = {
  [ProcessInstanceStatus.Running]: 'blue',
  [ProcessInstanceStatus.Completed]: 'green',
  [ProcessInstanceStatus.Suspended]: 'orange',
  [ProcessInstanceStatus.Terminated]: 'red',
};

/**
 * 版本历史项
 */
export type VersionHistoryItem = {
  id: number;
  version: number;
  description?: string;
  isEnabled: boolean;
  createdAt: string;
  createdBy: string;
};

/**
 * 创建流程定义请求
 */
export type CreateProcessDefinitionRequest = {
  name: string;
  key: string;
  description?: string;
  definition: string;
  isEnabled: boolean;
};

/**
 * 更新流程定义请求
 */
export type UpdateProcessDefinitionRequest = {
  name: string;
  description?: string;
  definition: string;
  isEnabled: boolean;
};
