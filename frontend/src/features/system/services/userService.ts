import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type {
  UserListItem,
  CreateUserRequest,
  UpdateUserRequest,
} from '../types';

const BASE_URL = '/api/system/user';

/**
 * 用户服务
 */
export const userService = {
  /**
   * 获取用户列表
   */
  async getUsers(
    pageIndex: number = 1,
    pageSize: number = 10,
    searchText?: string
  ): Promise<PagedResponse<UserListItem>> {
    return request<PagedResponse<UserListItem>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params: { pageIndex, pageSize, searchText },
    });
  },

  /**
   * 获取用户详情
   */
  async getUser(id: string): Promise<UserListItem> {
    return request<UserListItem>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建用户
   */
  async createUser(data: CreateUserRequest): Promise<UserListItem> {
    return request<UserListItem>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新用户
   */
  async updateUser(id: string, data: UpdateUserRequest): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/${id}`,
      data,
    });
  },

  /**
   * 删除用户
   */
  async deleteUser(id: string): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 重置密码
   */
  async resetPassword(userId: string, newPassword: string): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/resetPwd`,
      data: { userId, newPassword },
    });
  },

  /**
   * 更改用户状态
   */
  async changeStatus(userId: string, status: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/changeStatus`,
      data: { userId, status },
    });
  },

  /**
   * 启用用户
   */
  async enableUser(userId: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/changeStatus`,
      data: { userId, status: '0' }, // 0 表示正常状态
    });
  },

  /**
   * 禁用用户
   */
  async disableUser(userId: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/changeStatus`,
      data: { userId, status: '1' }, // 1 表示禁用状态
    });
  },
};
