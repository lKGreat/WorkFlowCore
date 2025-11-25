import { httpClient } from '@/api/httpClient';
import type {
  RoleListItem,
  CreateRoleRequest,
  UpdateRoleRequest,
} from '../types';

const BASE_URL = '/api/Roles';

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
 * 角色服务
 */
export const roleService = {
  /**
   * 获取角色列表
   */
  async getRoles(
    page: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<RoleListItem>> {
    const response = await httpClient.get<PagedResponse<RoleListItem>>(
      `${BASE_URL}`,
      { params: { page, pageSize } }
    );
    return response.data;
  },

  /**
   * 获取所有角色（不分页，用于下拉选择）
   */
  async getAllRoles(): Promise<RoleListItem[]> {
    const response = await httpClient.get<RoleListItem[]>(`${BASE_URL}/all`);
    return response.data;
  },

  /**
   * 获取角色详情
   */
  async getRole(id: string): Promise<RoleListItem> {
    const response = await httpClient.get<RoleListItem>(`${BASE_URL}/${id}`);
    return response.data;
  },

  /**
   * 创建角色
   */
  async createRole(data: CreateRoleRequest): Promise<RoleListItem> {
    const response = await httpClient.post<RoleListItem>(BASE_URL, data);
    return response.data;
  },

  /**
   * 更新角色
   */
  async updateRole(id: string, data: UpdateRoleRequest): Promise<RoleListItem> {
    const response = await httpClient.put<RoleListItem>(`${BASE_URL}/${id}`, data);
    return response.data;
  },

  /**
   * 删除角色
   */
  async deleteRole(id: string): Promise<void> {
    await httpClient.delete(`${BASE_URL}/${id}`);
  },
};

