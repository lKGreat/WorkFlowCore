import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type { NoticeDto, NoticeQueryDto } from '../types';

const BASE_URL = '/api/system/notice';

/**
 * 通知公告服务
 */
export const noticeService = {
  /**
   * 获取通知公告列表（分页）
   */
  async getPagedList(query: NoticeQueryDto): Promise<PagedResponse<NoticeDto>> {
    return request<PagedResponse<NoticeDto>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params: query,
    });
  },

  /**
   * 获取通知公告详情
   */
  async getById(id: number): Promise<NoticeDto> {
    return request<NoticeDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建通知公告
   */
  async create(data: Partial<NoticeDto>): Promise<NoticeDto> {
    return request<NoticeDto>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新通知公告
   */
  async update(data: Partial<NoticeDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 删除通知公告
   */
  async delete(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${ids.join(',')}`,
    });
  },

  /**
   * 获取弹出公告
   */
  async getPopupNotices(): Promise<NoticeDto[]> {
    return request<NoticeDto[]>({
      method: 'GET',
      url: `${BASE_URL}/popup`,
    });
  },
};

