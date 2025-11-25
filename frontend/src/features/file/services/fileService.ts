import { httpClient } from '@/api/httpClient';
import type {
  InitChunkUploadResponse,
  CompleteChunkUploadRequest,
  FileUploadResponse,
} from '../types';

const BASE_URL = '/api/Files';

/**
 * 文件服务
 */
export const fileService = {
  /**
   * 简单文件上传
   */
  async uploadFile(file: File): Promise<FileUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await httpClient.post<FileUploadResponse>(
      `${BASE_URL}/upload`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },

  /**
   * 初始化分片上传
   */
  async initChunkUpload(
    fileName: string,
    fileSize: number
  ): Promise<InitChunkUploadResponse> {
    const response = await httpClient.post<InitChunkUploadResponse>(
      `${BASE_URL}/chunk/init`,
      { fileName, fileSize }
    );
    return response.data;
  },

  /**
   * 上传分片
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
    const response = await httpClient.post<FileUploadResponse>(
      `${BASE_URL}/chunk/complete`,
      data
    );
    return response.data;
  },

  /**
   * 取消分片上传
   */
  async cancelChunkUpload(uploadId: string): Promise<void> {
    await httpClient.post(`${BASE_URL}/chunk/cancel`, { uploadId });
  },

  /**
   * 删除文件
   */
  async deleteFile(id: string): Promise<void> {
    await httpClient.delete(`${BASE_URL}/${id}`);
  },
};
