import { create } from 'zustand';
import type { ProcessDefinition } from '../types/processDefinition.types';

type WorkflowState = {
  // 当前选中的流程定义
  currentProcessDefinition: ProcessDefinition | null;
  // 流程定义列表
  processDefinitions: ProcessDefinition[];
  // 搜索关键字
  searchKeyword: string;
  // 是否显示已禁用的流程
  showDisabled: boolean;
};

type WorkflowActions = {
  setCurrentProcessDefinition: (definition: ProcessDefinition | null) => void;
  setProcessDefinitions: (definitions: ProcessDefinition[]) => void;
  setSearchKeyword: (keyword: string) => void;
  setShowDisabled: (show: boolean) => void;
  clearCurrentProcessDefinition: () => void;
  reset: () => void;
};

const initialState: WorkflowState = {
  currentProcessDefinition: null,
  processDefinitions: [],
  searchKeyword: '',
  showDisabled: false
};

/**
 * 工作流状态管理
 */
export const useWorkflowStore = create<WorkflowState & WorkflowActions>((set) => ({
  ...initialState,

  setCurrentProcessDefinition: (definition) =>
    set({ currentProcessDefinition: definition }),

  setProcessDefinitions: (definitions) =>
    set({ processDefinitions: definitions }),

  setSearchKeyword: (keyword) =>
    set({ searchKeyword: keyword }),

  setShowDisabled: (show) =>
    set({ showDisabled: show }),

  clearCurrentProcessDefinition: () =>
    set({ currentProcessDefinition: null }),

  reset: () =>
    set(initialState)
}));

