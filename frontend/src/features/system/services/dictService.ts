import { request } from '@/api';
import type { PagedResponse, PagedRequest } from '@/api';
import type { DictTypeDto, DictDataDto } from '../types';

const BASE_URL = '/api/system/dict';

/**
 * 字典服务
 */
export const dictService = {
  // ========== 字典类型 ==========
  /**
   * 获取字典类型列表（分页）
   */
  async getTypes(params: PagedRequest): Promise<PagedResponse<DictTypeDto>> {
    return request<PagedResponse<DictTypeDto>>({
      method: 'GET',
      url: `${BASE_URL}/type/list`,
      params,
    });
  },

  /**
   * 获取字典类型详情
   */
  async getTypeById(id: number): Promise<DictTypeDto> {
    return request<DictTypeDto>({
      method: 'GET',
      url: `${BASE_URL}/type/${id}`,
    });
  },

  /**
   * 创建字典类型
   */
  async createType(data: Partial<DictTypeDto>): Promise<DictTypeDto> {
    return request<DictTypeDto>({
      method: 'POST',
      url: `${BASE_URL}/type`,
      data,
    });
  },

  /**
   * 更新字典类型
   */
  async updateType(id: number, data: Partial<DictTypeDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/type/${id}`,
      data,
    });
  },

  /**
   * 删除字典类型
   */
  async deleteType(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/type/${ids.join(',')}`,
    });
  },

  // ========== 字典数据 ==========
  /**
   * 获取字典数据列表（分页）
   */
  async getData(params: PagedRequest & { dictTypeId?: number }): Promise<PagedResponse<DictDataDto>> {
    return request<PagedResponse<DictDataDto>>({
      method: 'GET',
      url: `${BASE_URL}/data/list`,
      params,
    });
  },

  /**
   * 根据字典类型获取字典数据
   */
  async getDataByType(dictType: string): Promise<DictDataDto[]> {
    return request<DictDataDto[]>({
      method: 'GET',
      url: `${BASE_URL}/data/type/${dictType}`,
    });
  },

  /**
   * 创建字典数据
   */
  async createData(data: Partial<DictDataDto>): Promise<DictDataDto> {
    return request<DictDataDto>({
      method: 'POST',
      url: `${BASE_URL}/data`,
      data,
    });
  },

  /**
   * 更新字典数据
   */
  async updateData(id: number, data: Partial<DictDataDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: `${BASE_URL}/data/${id}`,
      data,
    });
  },

  /**
   * 删除字典数据
   */
  async deleteData(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/data/${ids.join(',')}`,
    });
  },
};

