import { create } from 'zustand';
import type { NotificationPlacement } from 'antd/es/notification/interface';

type LoadingState = {
  [key: string]: boolean;
};

type NotificationConfig = {
  type: 'success' | 'info' | 'warning' | 'error';
  message: string;
  description?: string;
  duration?: number;
  placement?: NotificationPlacement;
};

type UiState = {
  // 全局加载状态
  globalLoading: boolean;
  // 按键加载状态（用于按钮等独立加载状态）
  loadingStates: LoadingState;
  // 侧边栏折叠状态
  sidebarCollapsed: boolean;
  // 通知队列
  notifications: NotificationConfig[];
};;

type UiActions = {
  setGlobalLoading: (loading: boolean) => void;
  setLoadingState: (key: string, loading: boolean) => void;
  clearLoadingState: (key: string) => void;
  toggleSidebar: () => void;
  setSidebarCollapsed: (collapsed: boolean) => void;
  showNotification: (config: NotificationConfig) => void;
  clearNotifications: () => void;
  showSuccess: (message: string, description?: string) => void;
  showError: (message: string, description?: string) => void;
  showWarning: (message: string, description?: string) => void;
  showInfo: (message: string, description?: string) => void;
};;

const initialState: UiState = {
  globalLoading: false,
  loadingStates: {},
  sidebarCollapsed: false,
  notifications: []
};

/**
 * UI 状态管理
 */
export const useUiStore = create<UiState & UiActions>((set, get) => ({
  ...initialState,

  setGlobalLoading: (loading) =>
    set({ globalLoading: loading }),

  setLoadingState: (key, loading) =>
    set((state) => ({
      loadingStates: {
        ...state.loadingStates,
        [key]: loading
      }
    })),

  clearLoadingState: (key) =>
    set((state) => {
      const newStates = { ...state.loadingStates };
      delete newStates[key];
      return { loadingStates: newStates };
    }),

  toggleSidebar: () =>
    set((state) => ({ sidebarCollapsed: !state.sidebarCollapsed })),

  setSidebarCollapsed: (collapsed) =>
    set({ sidebarCollapsed: collapsed }),

  showNotification: (config) =>
    set((state) => ({
      notifications: [...state.notifications, config]
    })),

  clearNotifications: () =>
    set({ notifications: [] }),

  showSuccess: (message, description) =>
    get().showNotification({
      type: 'success',
      message,
      description,
      duration: 4.5
    }),

  showError: (message, description) =>
    get().showNotification({
      type: 'error',
      message,
      description,
      duration: 4.5
    }),

  showWarning: (message, description) =>
    get().showNotification({
      type: 'warning',
      message,
      description,
      duration: 4.5
    }),

  showInfo: (message, description) =>
    get().showNotification({
      type: 'info',
      message,
      description,
      duration: 4.5
    })
}));

