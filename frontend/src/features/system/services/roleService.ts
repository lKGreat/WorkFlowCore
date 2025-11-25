import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type {
  RoleListItem,
  CreateRoleRequest,
  UpdateRoleRequest,
} from '../types';

const BASE_URL = '/api/system/role';

/**
 * 角色服务
 */
export const roleService = {
  /**
   * 获取角色列表
   */
  async getRoles(
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<PagedResponse<RoleListItem>> {
    return request<PagedResponse<RoleListItem>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params: { pageIndex, pageSize },
    });
  },

  /**
   * 获取所有角色（不分页，用于下拉选择）
   */
  async getAllRoles(): Promise<RoleListItem[]> {
    return request<RoleListItem[]>({
      method: 'GET',
      url: `${BASE_URL}/optionselect`,
    });
  },

  /**
   * 获取角色详情
   */
  async getRole(id: string): Promise<RoleListItem> {
    return request<RoleListItem>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建角色
   */
  async createRole(data: CreateRoleRequest): Promise<RoleListItem> {
    return request<RoleListItem>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新角色
   */
  async updateRole(id: string, data: UpdateRoleRequest): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/${id}`,
      data,
    });
  },

  /**
   * 删除角色
   */
  async deleteRole(id: string): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 获取角色的菜单ID列表
   */
  async getRoleMenuIds(roleId: string): Promise<number[]> {
    return request<number[]>({
      method: 'GET',
      url: `${BASE_URL}/${roleId}/menu`,
    });
  },

  /**
   * 分配角色菜单
   */
  async assignMenus(roleId: string, menuIds: number[]): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/${roleId}/menu`,
      data: menuIds,
    });
  },
};
