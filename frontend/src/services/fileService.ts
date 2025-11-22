import axios from 'axios';
import type {
  FileStorageProviderDto,
  FileAttachmentDto,
  InitiateUploadRequest,
  InitiateUploadResponse,
  GetUploadProgressResponse,
} from '../types/file.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7136';

// API 响应包装类型
type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T;
  errorCode?: string;
  timestamp: number;
};

// 创建 axios 实例
const fileApiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器 - 添加认证 Token
fileApiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// 响应拦截器 - 解包数据
fileApiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

// ==================== 存储提供者 API ====================

export const getStorageProviders = async (): Promise<FileStorageProviderDto[]> => {
  const response = await fileApiClient.get<ApiResponse<FileStorageProviderDto[]>>(
    '/api/file-storage-providers'
  );
  return (response as unknown as ApiResponse<FileStorageProviderDto[]>).data;
};

// ==================== 文件上传 API ====================

export const initiateUpload = async (
  request: InitiateUploadRequest
): Promise<InitiateUploadResponse> => {
  const response = await fileApiClient.post<ApiResponse<InitiateUploadResponse>>(
    '/api/files/initiate-upload',
    request
  );
  return (response as unknown as ApiResponse<InitiateUploadResponse>).data;
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

  await fileApiClient.post('/api/files/upload-chunk', formData, {
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
  const response = await fileApiClient.post<ApiResponse<FileAttachmentDto>>(
    '/api/files/complete-upload',
    { uploadId }
  );
  return (response as unknown as ApiResponse<FileAttachmentDto>).data;
};

export const getUploadProgress = async (uploadId: string): Promise<GetUploadProgressResponse> => {
  const response = await fileApiClient.get<ApiResponse<GetUploadProgressResponse>>(
    `/api/files/upload-progress/${uploadId}`
  );
  return (response as unknown as ApiResponse<GetUploadProgressResponse>).data;
};

export const cancelUpload = async (uploadId: string): Promise<void> => {
  await fileApiClient.post(`/api/files/cancel-upload/${uploadId}`);
};

// ==================== 文件访问 API ====================

export const getFileInfo = async (attachmentId: string): Promise<FileAttachmentDto> => {
  const response = await fileApiClient.get<ApiResponse<FileAttachmentDto>>(
    `/api/files/${attachmentId}`
  );
  return (response as unknown as ApiResponse<FileAttachmentDto>).data;
};

export const getFileDownloadUrl = async (attachmentId: string): Promise<string> => {
  return `${API_BASE_URL}/api/files/${attachmentId}/download`;
};

export const getAccessToken = async (attachmentId: string): Promise<string> => {
  const response = await fileApiClient.get<ApiResponse<string>>(
    `/api/files/${attachmentId}/access-token`
  );
  return (response as unknown as ApiResponse<string>).data;
};

export const getFilesByBusiness = async (
  businessType: string,
  businessId: string
): Promise<FileAttachmentDto[]> => {
  const response = await fileApiClient.get<ApiResponse<FileAttachmentDto[]>>(
    '/api/files/by-business',
    {
      params: { businessType, businessId },
    }
  );
  return (response as unknown as ApiResponse<FileAttachmentDto[]>).data;
};

export const deleteFile = async (attachmentId: string): Promise<void> => {
  await fileApiClient.delete(`/api/files/${attachmentId}`);
};

