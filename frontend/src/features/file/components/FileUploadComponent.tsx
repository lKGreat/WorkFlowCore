import React, { useState, useCallback } from 'react';
import { Upload, Button, Progress, message, Space, Modal, List } from 'antd';
import {
  UploadOutlined,
  DeleteOutlined,
  PauseOutlined,
  PlayCircleOutlined,
  FileOutlined,
} from '@ant-design/icons';
import type { UploadFile } from 'antd';
import { useChunkUpload } from '../../../hooks/useChunkUpload';
import type { FileUploadProgress, FileAttachmentDto } from '../types';

type FileUploadComponentProps = {
  businessType: string;
  businessId?: string;
  storageProviderId?: string;
  maxSize?: number; // 最大文件大小（字节）
  accept?: string; // 接受的文件类型
  maxCount?: number; // 最大文件数量
  multiple?: boolean; // 是否支持多文件上传
  onUploadSuccess?: (attachments: FileAttachmentDto[]) => void;
  onUploadError?: (error: Error) => void;
};

type UploadingFile = {
  uid: string;
  file: File;
  progress: FileUploadProgress;
};

export const FileUploadComponent: React.FC<FileUploadComponentProps> = ({
  businessType,
  businessId,
  storageProviderId,
  maxSize = 5 * 1024 * 1024 * 1024, // 默认 5GB
  accept = '*',
  maxCount = 10,
  multiple = true,
  onUploadSuccess,
  onUploadError,
}) => {
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const [uploadingFiles, setUploadingFiles] = useState<UploadingFile[]>([]);
  const [completedAttachments, setCompletedAttachments] = useState<FileAttachmentDto[]>([]);
  const [showPendingModal, setShowPendingModal] = useState(false);

  const { uploading, upload, cancel, getPendingTasks } = useChunkUpload();

  // 格式化文件大小
  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return `${(bytes / Math.pow(k, i)).toFixed(2)} ${sizes[i]}`;
  };

  // 文件选择前验证
  const beforeUpload = useCallback(
    (file: File): boolean => {
      // 检查文件大小
      if (file.size > maxSize) {
        message.error(`文件 ${file.name} 超过最大限制 ${formatFileSize(maxSize)}`);
        return false;
      }

      // 检查文件数量
      if (fileList.length >= maxCount) {
        message.error(`最多只能上传 ${maxCount} 个文件`);
        return false;
      }

      return true;
    },
    [fileList.length, maxCount, maxSize]
  );

  // 处理文件上传
  const handleUpload = useCallback(
    async (file: File) => {
      const uploadingFile: UploadingFile = {
        uid: file.name + Date.now(),
        file,
        progress: {
          uploadId: '',
          fileName: file.name,
          fileSize: file.size,
          uploadedSize: 0,
          percentage: 0,
          uploadedChunks: 0,
          totalChunks: 0,
          status: 'Pending',
        },
      };

      setUploadingFiles((prev) => [...prev, uploadingFile]);

      try {
        const attachment = await upload(file, {
          businessType,
          businessId,
          storageProviderId,
          onProgress: (progress) => {
            setUploadingFiles((prev) =>
              prev.map((uf) => (uf.uid === uploadingFile.uid ? { ...uf, progress } : uf))
            );
          },
          onSuccess: (attachment) => {
            setCompletedAttachments((prev) => [...prev, attachment]);
            setUploadingFiles((prev) => prev.filter((uf) => uf.uid !== uploadingFile.uid));
            message.success(`文件 ${file.name} 上传成功`);
          },
          onError: (error) => {
            message.error(`文件 ${file.name} 上传失败: ${error.message}`);
            setUploadingFiles((prev) => prev.filter((uf) => uf.uid !== uploadingFile.uid));
            onUploadError?.(error);
          },
        });

        if (attachment) {
          onUploadSuccess?.([attachment]);
        }
      } catch (error) {
        console.error('上传失败:', error);
      }
    },
    [businessType, businessId, storageProviderId, upload, onUploadSuccess, onUploadError]
  );

  // 自定义上传请求
  const customRequest = useCallback(
    (options: { file: File | Blob | string }) => {
      if (options.file instanceof File) {
        handleUpload(options.file);
      }
      return { abort: () => {} };
    },
    [handleUpload]
  );

  // 取消上传
  const handleCancel = useCallback(
    async (uploadId: string, uid: string) => {
      try {
        await cancel(uploadId);
        setUploadingFiles((prev) => prev.filter((uf) => uf.uid !== uid));
        message.info('已取消上传');
      } catch (error) {
        console.error('取消失败:', error);
      }
    },
    [cancel]
  );

  // 查看未完成任务
  const handleShowPendingTasks = useCallback(() => {
    setShowPendingModal(true);
  }, []);

  return (
    <div style={{ width: '100%' }}>
      <Space direction="vertical" style={{ width: '100%' }} size="large">
        {/* 上传区域 */}
        <Upload
          fileList={fileList}
          beforeUpload={beforeUpload}
          customRequest={customRequest}
          onChange={({ fileList: newFileList }) => setFileList(newFileList)}
          multiple={multiple}
          accept={accept}
          maxCount={maxCount}
          showUploadList={false}
        >
          <Button icon={<UploadOutlined />} disabled={uploading || fileList.length >= maxCount}>
            选择文件
          </Button>
          <span style={{ marginLeft: 8, color: '#999', fontSize: 12 }}>
            最大 {formatFileSize(maxSize)}，最多 {maxCount} 个文件
          </span>
        </Upload>

        {/* 显示未完成任务按钮 */}
        {getPendingTasks().length > 0 && (
          <Button icon={<PlayCircleOutlined />} onClick={handleShowPendingTasks}>
            查看未完成任务 ({getPendingTasks().length})
          </Button>
        )}

        {/* 上传进度列表 */}
        {uploadingFiles.length > 0 && (
          <div>
            <h4>上传中 ({uploadingFiles.length})</h4>
            <List
              dataSource={uploadingFiles}
              renderItem={(item) => (
                <List.Item
                  key={item.uid}
                  actions={[
                    <Button
                      key="cancel"
                      type="link"
                      danger
                      icon={<PauseOutlined />}
                      onClick={() => handleCancel(item.progress.uploadId, item.uid)}
                    >
                      取消
                    </Button>,
                  ]}
                >
                  <List.Item.Meta
                    avatar={<FileOutlined style={{ fontSize: 24 }} />}
                    title={item.file.name}
                    description={
                      <Space direction="vertical" style={{ width: '100%' }}>
                        <div>
                          {formatFileSize(item.progress.uploadedSize)} /{' '}
                          {formatFileSize(item.file.size)}
                        </div>
                        <Progress
                          percent={item.progress.percentage}
                          status={item.progress.status === 'Failed' ? 'exception' : 'active'}
                          format={(percent) => `${percent}%`}
                        />
                        <div style={{ fontSize: 12, color: '#999' }}>
                          分片进度: {item.progress.uploadedChunks} / {item.progress.totalChunks}
                        </div>
                      </Space>
                    }
                  />
                </List.Item>
              )}
            />
          </div>
        )}

        {/* 已完成列表 */}
        {completedAttachments.length > 0 && (
          <div>
            <h4>上传完成 ({completedAttachments.length})</h4>
            <List
              dataSource={completedAttachments}
              renderItem={(item) => (
                <List.Item
                  key={item.id}
                  actions={[
                    <Button
                      key="delete"
                      type="link"
                      danger
                      icon={<DeleteOutlined />}
                      onClick={() => {
                        setCompletedAttachments((prev) => prev.filter((a) => a.id !== item.id));
                      }}
                    >
                      移除
                    </Button>,
                  ]}
                >
                  <List.Item.Meta
                    avatar={<FileOutlined style={{ fontSize: 24, color: '#52c41a' }} />}
                    title={item.originalFileName}
                    description={formatFileSize(item.fileSize)}
                  />
                </List.Item>
              )}
            />
          </div>
        )}
      </Space>

      {/* 未完成任务对话框 */}
      <Modal
        title="未完成的上传任务"
        open={showPendingModal}
        onCancel={() => setShowPendingModal(false)}
        footer={null}
        width={600}
      >
        <List
          dataSource={getPendingTasks()}
          renderItem={(task) => (
            <List.Item
              actions={[
                <Button key="resume" type="primary" icon={<PlayCircleOutlined />}>
                  恢复
                </Button>,
                <Button key="delete" danger icon={<DeleteOutlined />}>
                  删除
                </Button>,
              ]}
            >
              <List.Item.Meta
                avatar={<FileOutlined style={{ fontSize: 24 }} />}
                title={task.fileName}
                description={
                  <>
                    <div>大小: {formatFileSize(task.fileSize)}</div>
                    <div>
                      进度: {task.uploadedChunks} / {task.totalChunks} 分片
                    </div>
                  </>
                }
              />
            </List.Item>
          )}
        />
      </Modal>
    </div>
  );
};

