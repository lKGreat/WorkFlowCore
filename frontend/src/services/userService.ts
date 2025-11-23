import { httpClient } from '../api/httpClient';
import type { UserListDto, CreateUserInput, UpdateUserInput, UserPagedRequest } from '../types/user.types';
import type { PagedResponse } from '../api/types';

/**
 * 获取用户分页列表
 */
export const getUserList = async (params: UserPagedRequest): Promise<PagedResponse<UserListDto>> => {
  const response = await httpClient.get('/system/user/list', { params });
  return response.data.data!;
};

/**
 * 获取用户详情
 */
export const getUserById = async (id: string): Promise<UserListDto> => {
  const response = await httpClient.get(`/system/user/${id}`);
  return response.data.data!;
};

/**
 * 创建用户
 */
export const createUser = async (data: CreateUserInput): Promise<UserListDto> => {
  const response = await httpClient.post('/system/user', data);
  return response.data.data!;
};

/**
 * 更新用户
 */
export const updateUser = async (id: string, data: UpdateUserInput): Promise<void> => {
  await httpClient.put(`/system/user/${id}`, data);
};

/**
 * 删除用户(批量)
 */
export const deleteUsers = async (ids: string[]): Promise<void> => {
  await httpClient.delete(`/system/user/${ids.join(',')}`);
};

/**
 * 重置密码
 */
export const resetPassword = async (userId: string, newPassword: string): Promise<void> => {
  await httpClient.put('/system/user/resetPwd', { userId, newPassword });
};

/**
 * 更改状态
 */
export const changeStatus = async (userId: string, status: string): Promise<void> => {
  await httpClient.post('/system/user/changeStatus', { userId, status });
};

