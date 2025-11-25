/**
 * 文件服务适配层（兼容旧代码）
 */
import { httpClient } from '../api/httpClient';
import type {
  InitiateUploadRequest,
  InitiateUploadResponse,
  GetUploadProgressResponse,
  FileAttachmentDto,
} from '../features/file/types';

const BASE_URL = '/api/Files';

/**
 * 初始化分片上传
 */
export async function initiateUpload(
  request: InitiateUploadRequest
): Promise<InitiateUploadResponse> {
  const response = await httpClient.post<InitiateUploadResponse>(
    `${BASE_URL}/chunk/init`,
    request
  );
  return response.data;
}

/**
 * 上传分片
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
  const response = await httpClient.post<FileAttachmentDto>(
    `${BASE_URL}/chunk/complete`,
    { uploadId }
  );
  return response.data;
}

/**
 * 获取上传进度
 */
export async function getUploadProgress(
  uploadId: string
): Promise<GetUploadProgressResponse> {
  const response = await httpClient.get<GetUploadProgressResponse>(
    `${BASE_URL}/chunk/progress/${uploadId}`
  );
  return response.data;
}

/**
 * 取消上传
 */
export async function cancelUpload(uploadId: string): Promise<void> {
  await httpClient.post(`${BASE_URL}/chunk/cancel`, { uploadId });
}

/**
 * 获取业务关联的文件列表
 */
export async function getFilesByBusiness(
  businessType: string,
  businessId: string
): Promise<FileAttachmentDto[]> {
  const response = await httpClient.get<FileAttachmentDto[]>(
    `${BASE_URL}/business/${businessType}/${businessId}`
  );
  return response.data;
}

/**
 * 删除文件
 */
export async function deleteFile(attachmentId: string): Promise<void> {
  await httpClient.delete(`${BASE_URL}/${attachmentId}`);
}

/**
 * 获取文件下载URL
 */
export async function getFileDownloadUrl(attachmentId: string): Promise<string> {
  const response = await httpClient.get<{ url: string }>(
    `${BASE_URL}/${attachmentId}/download-url`
  );
  return response.data.url;
}

