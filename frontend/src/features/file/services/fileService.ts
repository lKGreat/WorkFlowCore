import { request, httpClient } from '@/api';
import type {
  InitChunkUploadResponse,
  CompleteChunkUploadRequest,
  FileUploadResponse,
  InitiateUploadRequest,
  InitiateUploadResponse,
  GetUploadProgressResponse,
  FileAttachmentDto,
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
    chunk: Blob,
    onProgress?: (percentage: number) => void
  ): Promise<void> {
    const formData = new FormData();
    formData.append('uploadId', uploadId);
    formData.append('chunkIndex', String(chunkIndex));
    formData.append('chunk', chunk);
    
    await httpClient.post(`${BASE_URL}/chunk/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const percentage = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(percentage);
        }
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

/**
 * 初始化分片上传（兼容旧版本接口名）
 */
export async function initiateUpload(
  data: InitiateUploadRequest
): Promise<InitiateUploadResponse> {
  return request<InitiateUploadResponse>({
    method: 'POST',
    url: `${BASE_URL}/chunk/init`,
    data,
  });
}

/**
 * 上传分片（兼容旧版本接口名）
 */
export async function uploadChunk(
  uploadId: string,
  chunkIndex: number,
  chunk: Blob,
  onProgress?: (percentage: number) => void
): Promise<void> {
  return fileService.uploadChunk(uploadId, chunkIndex, chunk, onProgress);
}

/**
 * 完成分片上传（兼容旧版本接口名）
 */
export async function completeUpload(uploadId: string): Promise<FileAttachmentDto> {
  return request<FileAttachmentDto>({
    method: 'POST',
    url: `${BASE_URL}/chunk/complete`,
    data: { uploadId },
  });
}

/**
 * 获取上传进度
 */
export async function getUploadProgress(
  uploadId: string
): Promise<GetUploadProgressResponse> {
  return request<GetUploadProgressResponse>({
    method: 'GET',
    url: `${BASE_URL}/chunk/progress/${uploadId}`,
  });
}

/**
 * 取消上传
 */
export async function cancelUpload(uploadId: string): Promise<void> {
  return fileService.cancelChunkUpload(uploadId);
}

/**
 * 获取业务关联的文件列表
 */
export async function getFilesByBusiness(
  businessType: string,
  businessId: string
): Promise<FileAttachmentDto[]> {
  return request<FileAttachmentDto[]>({
    method: 'GET',
    url: `${BASE_URL}/business/${businessType}/${businessId}`,
  });
}

/**
 * 删除文件
 */
export async function deleteFile(attachmentId: string): Promise<void> {
  return fileService.deleteFile(attachmentId);
}

/**
 * 获取文件下载URL
 */
export async function getFileDownloadUrl(attachmentId: string): Promise<string> {
  const result = await request<{ url: string }>({
    method: 'GET',
    url: `${BASE_URL}/${attachmentId}/download-url`,
  });
  return result.url;
}
