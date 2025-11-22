import type { Node, Edge } from '@xyflow/react';

// 流程定义
export type ProcessDefinition = {
  id: string;
  name: string;
  key: string;
  version: number;
  description?: string;
  content: string;
  contentFormat: string;
  isEnabled: boolean;
  tenantId: string;
  createdAt: string;
  updatedAt: string;
};

// 流程定义列表项
export type ProcessDefinitionListItem = {
  id: string;
  name: string;
  key: string;
  version: number;
  description?: string;
  contentFormat: string;
  isEnabled: boolean;
  createdAt: string;
  updatedAt: string;
};

// 流程定义版本
export type ProcessDefinitionVersion = {
  id: string;
  name: string;
  key: string;
  version: number;
  description?: string;
  isEnabled: boolean;
  createdAt: string;
  updatedAt: string;
};

// 创建流程定义请求
export type CreateProcessDefinitionRequest = {
  name: string;
  key: string;
  description?: string;
  content: string;
  contentFormat?: string;
  isEnabled?: boolean;
};

// 更新流程定义请求
export type UpdateProcessDefinitionRequest = {
  name?: string;
  description?: string;
  content?: string;
  contentFormat?: string;
  isEnabled?: boolean;
};

// 流程图数据
export type ProcessFlowData = {
  nodes: Node[];
  edges: Edge[];
};

// 分页请求
export type PagedRequest = {
  pageIndex: number;
  pageSize: number;
};

// 分页响应
export type PagedResponse<T> = {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages?: number;
};

// 工作流实例
export type WorkflowInstance = {
  id: string;
  workflowDefinitionId: string;
  version: number;
  reference?: string;
  status: string;
  createTime: string;
  completeTime?: string;
  data?: Record<string, any>;
};

