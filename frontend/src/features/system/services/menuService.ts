import { request } from '@/api';
import type { MenuDto } from '../types';

const BASE_URL = '/api/menu';

/**
 * 菜单服务
 */
export const menuService = {
  /**
   * 获取菜单列表
   */
  async getList(): Promise<MenuDto[]> {
    return request<MenuDto[]>({
      method: 'GET',
      url: `${BASE_URL}/list`,
    });
  },

  /**
   * 获取菜单树
   */
  async getTree(): Promise<MenuDto[]> {
    return request<MenuDto[]>({
      method: 'GET',
      url: `${BASE_URL}/treelist`,
    });
  },

  /**
   * 获取菜单详情
   */
  async getById(id: number): Promise<MenuDto> {
    return request<MenuDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建菜单
   */
  async create(data: Partial<MenuDto>): Promise<MenuDto> {
    return request<MenuDto>({
      method: 'PUT',
      url: `${BASE_URL}/add`,
      data,
    });
  },

  /**
   * 更新菜单
   */
  async update(data: Partial<MenuDto>): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/edit`,
      data,
    });
  },

  /**
   * 删除菜单
   */
  async delete(id: number): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 获取菜单下拉树
   */
  async getTreeSelect(): Promise<MenuDto[]> {
    return request<MenuDto[]>({
      method: 'GET',
      url: `${BASE_URL}/treeSelect`,
    });
  },
};

