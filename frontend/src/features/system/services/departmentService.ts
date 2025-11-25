import { request } from '@/api';
import type { DepartmentDto } from '../types';

const BASE_URL = '/api/system/dept';

/**
 * 部门服务
 */
export const departmentService = {
  /**
   * 获取部门树
   */
  async getTree(): Promise<DepartmentDto[]> {
    return request<DepartmentDto[]>({
      method: 'GET',
      url: `${BASE_URL}/list`,
    });
  },

  /**
   * 获取部门树（排除指定节点）
   */
  async getTreeExclude(deptId: number): Promise<DepartmentDto[]> {
    return request<DepartmentDto[]>({
      method: 'GET',
      url: `${BASE_URL}/list/exclude/${deptId}`,
    });
  },

  /**
   * 获取部门详情
   */
  async getById(id: number): Promise<DepartmentDto> {
    return request<DepartmentDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建部门
   */
  async create(data: Partial<DepartmentDto>): Promise<DepartmentDto> {
    return request<DepartmentDto>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新部门
   */
  async update(id: number, data: Partial<DepartmentDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/${id}`,
      data,
    });
  },

  /**
   * 删除部门
   */
  async delete(id: number): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 获取部门下拉树
   */
  async getTreeSelect(): Promise<DepartmentDto[]> {
    return request<DepartmentDto[]>({
      method: 'GET',
      url: `${BASE_URL}/treeselect`,
    });
  },
};

