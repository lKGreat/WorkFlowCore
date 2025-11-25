import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type { PostDto, PostQueryDto } from '../types';

const BASE_URL = '/api/system/post';

/**
 * 岗位服务
 */
export const postService = {
  /**
   * 获取岗位列表（分页）
   */
  async getPagedList(query: PostQueryDto): Promise<PagedResponse<PostDto>> {
    return request<PagedResponse<PostDto>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params: query,
    });
  },

  /**
   * 获取岗位详情
   */
  async getById(id: number): Promise<PostDto> {
    return request<PostDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建岗位
   */
  async create(data: Partial<PostDto>): Promise<PostDto> {
    return request<PostDto>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新岗位
   */
  async update(data: Partial<PostDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 删除岗位
   */
  async delete(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${ids.join(',')}`,
    });
  },

  /**
   * 获取岗位下拉选择列表
   */
  async getOptionSelect(): Promise<PostDto[]> {
    return request<PostDto[]>({
      method: 'GET',
      url: `${BASE_URL}/optionselect`,
    });
  },
};

