import { httpClient } from '@/api/httpClient';
import type {
  UserListItem,
  CreateUserRequest,
  UpdateUserRequest,
} from '../types';

const BASE_URL = '/api/Users';

/**
 * 分页响应
 */
type PagedResponse<T> = {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};

/**
 * 用户服务
 */
export const userService = {
  /**
   * 获取用户列表
   */
  async getUsers(
    page: number = 1,
    pageSize: number = 10,
    searchText?: string
  ): Promise<PagedResponse<UserListItem>> {
    const response = await httpClient.get<PagedResponse<UserListItem>>(
      `${BASE_URL}`,
      { params: { page, pageSize, searchText } }
    );
    return response.data;
  },

  /**
   * 获取用户详情
   */
  async getUser(id: string): Promise<UserListItem> {
    const response = await httpClient.get<UserListItem>(`${BASE_URL}/${id}`);
    return response.data;
  },

  /**
   * 创建用户
   */
  async createUser(data: CreateUserRequest): Promise<UserListItem> {
    const response = await httpClient.post<UserListItem>(BASE_URL, data);
    return response.data;
  },

  /**
   * 更新用户
   */
  async updateUser(id: string, data: UpdateUserRequest): Promise<UserListItem> {
    const response = await httpClient.put<UserListItem>(`${BASE_URL}/${id}`, data);
    return response.data;
  },

  /**
   * 删除用户
   */
  async deleteUser(id: string): Promise<void> {
    await httpClient.delete(`${BASE_URL}/${id}`);
  },

  /**
   * 重置密码
   */
  async resetPassword(id: string, newPassword: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/reset-password`, { newPassword });
  },

  /**
   * 启用用户
   */
  async enableUser(id: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/enable`);
  },

  /**
   * 禁用用户
   */
  async disableUser(id: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/${id}/disable`);
  },
};
