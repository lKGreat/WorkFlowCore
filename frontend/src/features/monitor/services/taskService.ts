import { request } from '@/api';
import type { PagedResponse } from '@/api';
import type { TaskDto, TaskQueryDto, TaskLogDto, TaskLogQueryDto } from '../types';

const BASE_URL = '/api/monitor/job';

/**
 * 定时任务服务
 */
export const taskService = {
  /**
   * 获取定时任务列表（分页）
   */
  async getPagedList(query: TaskQueryDto): Promise<PagedResponse<TaskDto>> {
    return request<PagedResponse<TaskDto>>({
      method: 'GET',
      url: `${BASE_URL}/list`,
      params: query,
    });
  },

  /**
   * 获取任务详情
   */
  async getById(id: number): Promise<TaskDto> {
    return request<TaskDto>({
      method: 'GET',
      url: `${BASE_URL}/${id}`,
    });
  },

  /**
   * 创建任务
   */
  async create(data: Partial<TaskDto>): Promise<TaskDto> {
    return request<TaskDto>({
      method: 'POST',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 更新任务
   */
  async update(data: Partial<TaskDto>): Promise<void> {
    return request<void>({
      method: 'PUT',
      url: BASE_URL,
      data,
    });
  },

  /**
   * 删除任务
   */
  async delete(ids: number[]): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${ids.join(',')}`,
    });
  },

  /**
   * 获取任务日志列表（分页）
   */
  async getLogPagedList(query: TaskLogQueryDto): Promise<PagedResponse<TaskLogDto>> {
    return request<PagedResponse<TaskLogDto>>({
      method: 'GET',
      url: `${BASE_URL}/log/list`,
      params: query,
    });
  },

  /**
   * 清空任务日志
   */
  async clearLogs(taskId: number): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/log/clear/${taskId}`,
    });
  },
};

