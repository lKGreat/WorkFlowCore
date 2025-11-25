import { request, httpClient } from '@/api';
import type {
  InitChunkUploadResponse,
  CompleteChunkUploadRequest,
  FileUploadResponse,
} from '../types';

const BASE_URL = '/api/files';

/**
 * 文件服务
 */
export const fileService = {
  /**
   * 简单文件上传
   * 注意：文件上传需要使用 httpClient 以支持 FormData 和进度回调
   */
  async uploadFile(file: File): Promise<FileUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await httpClient.post<{ success: boolean; data: FileUploadResponse }>(
      `${BASE_URL}/upload`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data.data;
  },

  /**
   * 初始化分片上传
   */
  async initChunkUpload(
    fileName: string,
    fileSize: number
  ): Promise<InitChunkUploadResponse> {
    return request<InitChunkUploadResponse>({
      method: 'POST',
      url: `${BASE_URL}/chunk/init`,
      data: { fileName, fileSize },
    });
  },

  /**
   * 上传分片
   * 注意：分片上传需要使用 httpClient 以支持 FormData
   */
  async uploadChunk(
    uploadId: string,
    chunkIndex: number,
    chunk: Blob
  ): Promise<void> {
    const formData = new FormData();
    formData.append('uploadId', uploadId);
    formData.append('chunkIndex', String(chunkIndex));
    formData.append('chunk', chunk);
    
    await httpClient.post(`${BASE_URL}/chunk/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  /**
   * 完成分片上传
   */
  async completeChunkUpload(
    data: CompleteChunkUploadRequest
  ): Promise<FileUploadResponse> {
    return request<FileUploadResponse>({
      method: 'POST',
      url: `${BASE_URL}/chunk/complete`,
      data,
    });
  },

  /**
   * 取消分片上传
   */
  async cancelChunkUpload(uploadId: string): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/chunk/cancel`,
      data: { uploadId },
    });
  },

  /**
   * 删除文件
   */
  async deleteFile(id: string): Promise<void> {
    return request<void>({
      method: 'DELETE',
      url: `${BASE_URL}/${id}`,
    });
  },
};
