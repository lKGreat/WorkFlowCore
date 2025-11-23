import { request, httpClient } from '../api';
import { getToken } from '../utils/auth';
import type {
  FileStorageProviderDto,
  FileAttachmentDto,
  InitiateUploadRequest,
  InitiateUploadResponse,
  GetUploadProgressResponse,
} from '../types/file.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7136';

// ==================== 存储提供者 API ====================

export const getStorageProviders = async (): Promise<FileStorageProviderDto[]> => {
  return request<FileStorageProviderDto[]>({
    method: 'GET',
    url: '/api/file-storage-providers'
  });
};

// ==================== 文件上传 API ====================

export const initiateUpload = async (
  data: InitiateUploadRequest
): Promise<InitiateUploadResponse> => {
  return request<InitiateUploadResponse>({
    method: 'POST',
    url: '/api/files/initiate-upload',
    data
  });
};

export const uploadChunk = async (
  uploadId: string,
  chunkIndex: number,
  chunkData: Blob,
  onProgress?: (progress: number) => void
): Promise<void> => {
  const formData = new FormData();
  formData.append('uploadId', uploadId);
  formData.append('chunkIndex', chunkIndex.toString());
  formData.append('chunk', chunkData);

  await httpClient.post('/api/files/upload-chunk', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
    onUploadProgress: (progressEvent) => {
      if (progressEvent.total && onProgress) {
        const percentage = Math.round((progressEvent.loaded * 100) / progressEvent.total);
        onProgress(percentage);
      }
    },
  });
};

export const completeUpload = async (uploadId: string): Promise<FileAttachmentDto> => {
  return request<FileAttachmentDto>({
    method: 'POST',
    url: '/api/files/complete-upload',
    data: { uploadId }
  });
};

export const getUploadProgress = async (uploadId: string): Promise<GetUploadProgressResponse> => {
  return request<GetUploadProgressResponse>({
    method: 'GET',
    url: `/api/files/upload-progress/${uploadId}`
  });
};

export const cancelUpload = async (uploadId: string): Promise<void> => {
  await request({
    method: 'POST',
    url: `/api/files/cancel-upload/${uploadId}`
  });
};

// ==================== 文件访问 API ====================

export const getFileInfo = async (attachmentId: string): Promise<FileAttachmentDto> => {
  return request<FileAttachmentDto>({
    method: 'GET',
    url: `/api/files/${attachmentId}`
  });
};

export const getFileDownloadUrl = (attachmentId: string): string => {
  const token = getToken();
  const url = `${API_BASE_URL}/api/files/${attachmentId}/download`;
  return token ? `${url}?token=${encodeURIComponent(token)}` : url;
};

export const getAccessToken = async (attachmentId: string): Promise<string> => {
  return request<string>({
    method: 'GET',
    url: `/api/files/${attachmentId}/access-token`
  });
};

export const getFilesByBusiness = async (
  businessType: string,
  businessId: string
): Promise<FileAttachmentDto[]> => {
  return request<FileAttachmentDto[]>({
    method: 'GET',
    url: '/api/files/by-business',
    params: { businessType, businessId }
  });
};

export const deleteFile = async (attachmentId: string): Promise<void> => {
  await request({
    method: 'DELETE',
    url: `/api/files/${attachmentId}`
  });
};

