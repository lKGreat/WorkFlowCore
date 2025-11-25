import { request } from '@/api';
import type { PagedResponse, PagedRequest } from '@/api';
import type { ConfigDto } from '../types';

const BASE_URL = '/api/system/config';

/**
 * 系统配置服务
 */
export const configService = {
  /**
   * 获取配置列表（分页）
   */
  async getPagedList(params: PagedRequest): Promise<PagedResponse<ConfigDto>> {
    return request<PagedResponse<ConfigDto>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params,
    });
  },

  /**
   * 获取配置详情
   */
  async getById(id: number): Promise<ConfigDto> {
    return request<ConfigDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 根据配置键获取配置值
   */
  async getByKey(key: string): Promise<string> {
    return request<string>({
      method: 'GET',
      url: `${BASE_URL}/configKey/${key}`,
    });
  },

  /**
   * 创建配置
   */
  async create(data: Partial<ConfigDto>): Promise<ConfigDto> {
    return request<ConfigDto>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新配置
   */
  async update(id: number, data: Partial<ConfigDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/${id}`,
      data,
    });
  },

  /**
   * 删除配置
   */
  async delete(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${ids.join(',')}`,
    });
  },
};

