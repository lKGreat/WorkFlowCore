import { useState, useCallback, useRef } from 'react';
import SparkMD5 from 'spark-md5';
import {
  initiateUpload,
  uploadChunk,
  completeUpload,
  getUploadProgress,
  cancelUpload,
} from '../features/file/services/fileService';
import type {
  FileUploadProgress,
  StoredUploadTask,
  ChunkUploadProgress,
  InitiateUploadRequest,
  FileAttachmentDto,
} from '../features/file/types';

const CHUNK_SIZE = 5 * 1024 * 1024; // 5MB
const MAX_CONCURRENT_UPLOADS = 3; // 最大并发上传数
const STORAGE_KEY = 'chunk_upload_tasks';

type UploadOptions = {
  businessType: string;
  businessId?: string;
  storageProviderId?: string;
  onProgress?: (progress: FileUploadProgress) => void;
  onSuccess?: (attachment: FileAttachmentDto) => void;
  onError?: (error: Error) => void;
};

export const useChunkUpload = () => {
  const [uploading, setUploading] = useState(false);
  const [progress, setProgress] = useState<FileUploadProgress | null>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  // 计算文件 MD5
  const calculateMD5 = useCallback((file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const spark = new SparkMD5.ArrayBuffer();
      const fileReader = new FileReader();
      const chunkSize = CHUNK_SIZE;
      let currentChunk = 0;
      const chunks = Math.ceil(file.size / chunkSize);

      fileReader.onload = (e) => {
        if (e.target?.result) {
          spark.append(e.target.result as ArrayBuffer);
          currentChunk++;

          if (currentChunk < chunks) {
            loadNext();
          } else {
            resolve(spark.end());
          }
        }
      };

      fileReader.onerror = () => {
        reject(new Error('计算 MD5 失败'));
      };

      const loadNext = () => {
        const start = currentChunk * chunkSize;
        const end = Math.min(start + chunkSize, file.size);
        fileReader.readAsArrayBuffer(file.slice(start, end));
      };

      loadNext();
    });
  }, []);

  // 保存上传任务到本地存储
  const saveUploadTask = useCallback((task: StoredUploadTask) => {
    const tasks = getStoredUploadTasks();
    tasks[task.uploadId] = task;
    localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
  }, []);

  // 获取所有存储的上传任务
  const getStoredUploadTasks = useCallback((): Record<string, StoredUploadTask> => {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored ? JSON.parse(stored) : {};
  }, []);

  // 删除上传任务
  const removeUploadTask = useCallback((uploadId: string) => {
    const tasks = getStoredUploadTasks();
    delete tasks[uploadId];
    localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
  }, [getStoredUploadTasks]);

  // 更新进度
  const updateProgress = useCallback(
    (uploadId: string, updates: Partial<FileUploadProgress>) => {
      setProgress((prev) => {
        if (!prev || prev.uploadId !== uploadId) return prev;
        const newProgress = { ...prev, ...updates };

        // 计算整体进度
        if (newProgress.totalChunks > 0) {
          newProgress.percentage = Math.round(
            (newProgress.uploadedChunks / newProgress.totalChunks) * 100
          );
          newProgress.uploadedSize = Math.round(
            (newProgress.fileSize / newProgress.totalChunks) * newProgress.uploadedChunks
          );
        }

        return newProgress;
      });
    },
    []
  );

  // 上传文件
  const upload = useCallback(
    async (file: File, options: UploadOptions): Promise<FileAttachmentDto | null> => {
      const { businessType, businessId, storageProviderId, onProgress, onSuccess, onError } =
        options;

      setUploading(true);
      abortControllerRef.current = new AbortController();

      try {
        // 1. 计算 MD5
        const md5Hash = await calculateMD5(file);

        // 2. 初始化上传
        const initRequest: InitiateUploadRequest = {
          fileName: file.name,
          fileSize: file.size,
          md5Hash,
          contentType: file.type || 'application/octet-stream',
          businessType,
          businessId,
          storageProviderId,
        };

        const initResponse = await initiateUpload(initRequest);

        // 3. 初始化进度
        const initialProgress: FileUploadProgress = {
          uploadId: initResponse.uploadId,
          fileName: file.name,
          fileSize: file.size,
          uploadedSize: 0,
          percentage: 0,
          uploadedChunks: 0,
          totalChunks: initResponse.totalChunks,
          status: 'Uploading',
        };
        setProgress(initialProgress);
        onProgress?.(initialProgress);

        // 4. 保存上传任务
        const uploadTask: StoredUploadTask = {
          uploadId: initResponse.uploadId,
          attachmentId: initResponse.attachmentId,
          fileName: file.name,
          fileSize: file.size,
          md5Hash,
          totalChunks: initResponse.totalChunks,
          uploadedChunks: 0,
          completedChunkIndexes: [],
          businessType,
          businessId,
          createdAt: Date.now(),
        };
        saveUploadTask(uploadTask);

        // 5. 分片上传（并发控制）
        const totalChunks = initResponse.totalChunks;
        const chunkProgress: ChunkUploadProgress[] = Array.from({ length: totalChunks }, (_, i) => ({
          chunkIndex: i,
          loaded: 0,
          total: CHUNK_SIZE,
          percentage: 0,
        }));

        const uploadQueue: number[] = Array.from({ length: totalChunks }, (_, i) => i);
        let uploadedCount = 0;

        const uploadChunkWithRetry = async (chunkIndex: number, retries = 3): Promise<void> => {
          if (abortControllerRef.current?.signal.aborted) {
            throw new Error('上传已取消');
          }

          const start = chunkIndex * CHUNK_SIZE;
          const end = Math.min(start + CHUNK_SIZE, file.size);
          const chunkData = file.slice(start, end);

          try {
            await uploadChunk(initResponse.uploadId, chunkIndex, chunkData, (chunkPercentage) => {
              chunkProgress[chunkIndex] = {
                chunkIndex,
                loaded: Math.round((chunkPercentage / 100) * chunkData.size),
                total: chunkData.size,
                percentage: chunkPercentage,
              };
            });

            uploadedCount++;
            uploadTask.uploadedChunks = uploadedCount;
            uploadTask.completedChunkIndexes.push(chunkIndex);
            saveUploadTask(uploadTask);

            updateProgress(initResponse.uploadId, {
              uploadedChunks: uploadedCount,
              chunkProgress: [...chunkProgress],
            });

            const updatedProgress = {
              ...initialProgress,
              uploadedChunks: uploadedCount,
              percentage: Math.round((uploadedCount / totalChunks) * 100),
              uploadedSize: Math.round((file.size / totalChunks) * uploadedCount),
              chunkProgress: [...chunkProgress],
            };
            onProgress?.(updatedProgress);
          } catch (error) {
            if (retries > 0) {
              console.warn(`分片 ${chunkIndex} 上传失败，重试中... (剩余 ${retries} 次)`);
              await new Promise((resolve) => setTimeout(resolve, 1000));
              return uploadChunkWithRetry(chunkIndex, retries - 1);
            }
            throw error;
          }
        };

        // 并发上传分片
        while (uploadQueue.length > 0) {
          const batch = uploadQueue.splice(0, MAX_CONCURRENT_UPLOADS);
          await Promise.all(batch.map((index) => uploadChunkWithRetry(index)));
        }

        // 6. 完成上传
        const attachment = await completeUpload(initResponse.uploadId);

        // 7. 清理任务
        removeUploadTask(initResponse.uploadId);

        // 8. 更新最终状态
        const finalProgress: FileUploadProgress = {
          uploadId: initResponse.uploadId,
          fileName: file.name,
          fileSize: file.size,
          uploadedSize: file.size,
          percentage: 100,
          uploadedChunks: totalChunks,
          totalChunks,
          status: 'Completed',
        };
        setProgress(finalProgress);
        onProgress?.(finalProgress);
        onSuccess?.(attachment);

        return attachment;
      } catch (error) {
        const errorObj = error as Error;
        console.error('上传失败:', errorObj);
        updateProgress('', { status: 'Failed' });
        onError?.(errorObj);
        return null;
      } finally {
        setUploading(false);
        abortControllerRef.current = null;
      }
    },
    [calculateMD5, saveUploadTask, removeUploadTask, updateProgress]
  );

  // 恢复上传
  const resumeUpload = useCallback(
    async (uploadId: string, file: File): Promise<void> => {
      const tasks = getStoredUploadTasks();
      const task = tasks[uploadId];

      if (!task) {
        throw new Error('未找到上传任务');
      }

      // 获取服务器进度
      const serverProgress = await getUploadProgress(uploadId);
      const completedIndexes = new Set(serverProgress.completedChunkIndexes);

      // 继续上传未完成的分片
      const remainingChunks = Array.from(
        { length: task.totalChunks },
        (_, i) => i
      ).filter((i) => !completedIndexes.has(i));

      for (const chunkIndex of remainingChunks) {
        const start = chunkIndex * CHUNK_SIZE;
        const end = Math.min(start + CHUNK_SIZE, file.size);
        const chunkData = file.slice(start, end);

        await uploadChunk(uploadId, chunkIndex, chunkData);
        task.uploadedChunks++;
        saveUploadTask(task);
      }

      // 完成上传
      await completeUpload(uploadId);
      removeUploadTask(uploadId);
    },
    [getStoredUploadTasks, saveUploadTask, removeUploadTask]
  );

  // 取消上传
  const cancel = useCallback(
    async (uploadId: string) => {
      abortControllerRef.current?.abort();
      await cancelUpload(uploadId);
      removeUploadTask(uploadId);
      setUploading(false);
      setProgress(null);
    },
    [removeUploadTask]
  );

  // 获取未完成的上传任务
  const getPendingTasks = useCallback((): StoredUploadTask[] => {
    const tasks = getStoredUploadTasks();
    return Object.values(tasks);
  }, [getStoredUploadTasks]);

  return {
    uploading,
    progress,
    upload,
    resumeUpload,
    cancel,
    getPendingTasks,
  };
};

