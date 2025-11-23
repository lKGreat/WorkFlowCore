import { request } from '../api';
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
  return request<GetInfoResponse>({ method: 'GET', url: '/getInfo' });
};

/**
 * 获取动态路由
 */
export const getRouters = async (): Promise<RouterConfig[]> => {
  return request<RouterConfig[]>({ method: 'GET', url: '/getRouters' });
};

/**
 * 退出登录
 */
export const logout = async (): Promise<void> => {
  await request({ method: 'POST', url: '/LogOut' });
};

