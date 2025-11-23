import { httpClient } from '../api/httpClient';
import type { RouterConfig } from '../stores/routerStore';

export interface GetInfoResponse {
  user: {
    userId: string;
    userName: string;
    nickName: string;
    avatar?: string;
    email?: string;
    phoneNumber?: string;
    departmentId?: number;
    departmentName?: string;
    sex?: string;
    status: string;
  };
  roles: string[];
  permissions: string[];
}

/**
 * 获取当前用户信息
 */
export const getInfo = async (): Promise<GetInfoResponse> => {
  const response = await httpClient.get<GetInfoResponse>('/getInfo');
  return response.data;
};

/**
 * 获取动态路由
 */
export const getRouters = async (): Promise<RouterConfig[]> => {
  const response = await httpClient.get<RouterConfig[]>('/getRouters');
  return response.data;
};

/**
 * 退出登录
 */
export const logout = async (): Promise<void> => {
  await httpClient.post('/LogOut');
};

