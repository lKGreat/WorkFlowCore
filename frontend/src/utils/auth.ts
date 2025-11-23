import { useAuthStore } from '../stores/authStore';

/**
 * 获取Token（从 Zustand store）
 */
export const getToken = (): string | null => {
  return useAuthStore.getState().token;
};

/**
 * 设置Token（更新 Zustand store）
 */
export const setToken = (token: string): void => {
  useAuthStore.getState().setToken(token);
};

/**
 * 移除Token（清除 Zustand store）
 */
export const removeToken = (): void => {
  useAuthStore.getState().setToken(null);
};

