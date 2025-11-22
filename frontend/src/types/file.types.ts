// 存储提供者类型
export type StorageProviderType = 'Local' | 'AliyunOss' | 'AwsS3' | 'AzureBlob';

// 上传状态
export type UploadStatus = 'Pending' | 'Uploading' | 'Completed' | 'Failed' | 'Cancelled';

// 文件存储提供者配置
export type FileStorageProviderDto = {
  id: string;
  providerName: string;
  providerType: StorageProviderType;
  configuration: Record<string, unknown>;
  isEnabled: boolean;
  priority: number;
  createdAt: string;
};

// 文件附件信息
export type FileAttachmentDto = {
  id: string;
  fileName: string;
  originalFileName: string;
  fileSize: number;
  contentType: string;
  fileExtension: string;
  storageProviderId: string;
  storagePath: string;
  md5Hash: string;
  businessType: string;
  businessId: string;
  uploadStatus: UploadStatus;
  totalChunks: number;
  uploadedChunks: number;
  createdAt: string;
};

// 初始化上传请求
export type InitiateUploadRequest = {
  fileName: string;
  fileSize: number;
  md5Hash: string;
  contentType: string;
  businessType: string;
  businessId?: string;
  storageProviderId?: string;
};

// 初始化上传响应
export type InitiateUploadResponse = {
  uploadId: string;
  attachmentId: string;
  chunkSize: number;
  totalChunks: number;
  uploadUrls?: string[];
};

// 上传进度响应
export type GetUploadProgressResponse = {
  uploadId: string;
  totalChunks: number;
  uploadedChunks: number;
  uploadStatus: UploadStatus;
  completedChunkIndexes: number[];
};

// 分片上传进度
export type ChunkUploadProgress = {
  chunkIndex: number;
  loaded: number;
  total: number;
  percentage: number;
};

// 整体上传进度
export type FileUploadProgress = {
  uploadId: string;
  fileName: string;
  fileSize: number;
  uploadedSize: number;
  percentage: number;
  uploadedChunks: number;
  totalChunks: number;
  status: UploadStatus;
  chunkProgress?: ChunkUploadProgress[];
};

// 本地存储的上传任务
export type StoredUploadTask = {
  uploadId: string;
  attachmentId: string;
  fileName: string;
  fileSize: number;
  md5Hash: string;
  totalChunks: number;
  uploadedChunks: number;
  completedChunkIndexes: number[];
  businessType: string;
  businessId?: string;
  createdAt: number;
};

