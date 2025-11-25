/**
 * 文件服务适配层（兼容旧代码）
 */
import { request, httpClient } from '../api';
import type {
  InitiateUploadRequest,
  InitiateUploadResponse,
  GetUploadProgressResponse,
  FileAttachmentDto,
} from '../features/file/types';

const BASE_URL = '/api/files';

/**
 * 初始化分片上传
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
 * 上传分片
 * 注意：分片上传需要使用 httpClient 以支持 FormData 和进度回调
 */
export async function uploadChunk(
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
}

/**
 * 完成分片上传
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
  return request<void>({
    method: 'POST',
    url: `${BASE_URL}/chunk/cancel`,
    data: { uploadId },
  });
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
  return request<void>({
    method: 'DELETE',
    url: `${BASE_URL}/${attachmentId}`,
  });
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
