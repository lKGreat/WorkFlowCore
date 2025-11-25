import React, { useState, useEffect, useCallback } from 'react';
import { List, Button, Space, Modal, message, Empty, Spin, Tag } from 'antd';
import {
  DownloadOutlined,
  DeleteOutlined,
  EyeOutlined,
  FileOutlined,
  ExclamationCircleOutlined,
} from '@ant-design/icons';
import { getFilesByBusiness, deleteFile, getFileDownloadUrl } from '../services/fileService';
import type { FileAttachmentDto } from '../types';

type FileAttachmentListProps = {
  businessType: string;
  businessId: string;
  onDelete?: (attachmentId: string) => void;
  onRefresh?: () => void;
  autoRefresh?: boolean; // 自动刷新列表
  refreshInterval?: number; // 刷新间隔（毫秒）
};

export const FileAttachmentList: React.FC<FileAttachmentListProps> = ({
  businessType,
  businessId,
  onDelete,
  onRefresh,
  autoRefresh = false,
  refreshInterval = 5000,
}) => {
  const [attachments, setAttachments] = useState<FileAttachmentDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [deleting, setDeleting] = useState<string | null>(null);

  // 格式化文件大小
  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return `${(bytes / Math.pow(k, i)).toFixed(2)} ${sizes[i]}`;
  };

  // 格式化日期
  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  // 获取文件图标颜色
  const getFileIconColor = (extension: string): string => {
    const colorMap: Record<string, string> = {
      '.pdf': '#ff4d4f',
      '.doc': '#1890ff',
      '.docx': '#1890ff',
      '.xls': '#52c41a',
      '.xlsx': '#52c41a',
      '.jpg': '#faad14',
      '.jpeg': '#faad14',
      '.png': '#faad14',
      '.gif': '#faad14',
      '.zip': '#722ed1',
      '.rar': '#722ed1',
    };
    return colorMap[extension.toLowerCase()] || '#999';
  };

  // 获取上传状态标签
  const getStatusTag = (status: string) => {
    const statusMap: Record<string, { color: string; text: string }> = {
      Pending: { color: 'default', text: '等待中' },
      Uploading: { color: 'processing', text: '上传中' },
      Completed: { color: 'success', text: '已完成' },
      Failed: { color: 'error', text: '失败' },
      Cancelled: { color: 'warning', text: '已取消' },
    };
    const config = statusMap[status] || { color: 'default', text: status };
    return <Tag color={config.color}>{config.text}</Tag>;
  };

  // 加载附件列表
  const loadAttachments = useCallback(async () => {
    if (!businessId) return;

    setLoading(true);
    try {
      const data = await getFilesByBusiness(businessType, businessId);
      setAttachments(data);
    } catch (error) {
      console.error('加载附件列表失败:', error);
      message.error('加载附件列表失败');
    } finally {
      setLoading(false);
    }
  }, [businessType, businessId]);

  // 下载文件
  const handleDownload = useCallback(async (attachment: FileAttachmentDto) => {
    try {
      const downloadUrl = await getFileDownloadUrl(attachment.id);
      const link = document.createElement('a');
      link.href = downloadUrl;
      link.download = attachment.originalFileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      message.success('开始下载');
    } catch (error) {
      console.error('下载失败:', error);
      message.error('下载失败');
    }
  }, []);

  // 预览文件
  const handlePreview = useCallback(async (attachment: FileAttachmentDto) => {
    try {
      const downloadUrl = await getFileDownloadUrl(attachment.id);
      window.open(downloadUrl, '_blank');
    } catch (error) {
      console.error('预览失败:', error);
      message.error('预览失败');
    }
  }, []);

  // 删除文件
  const handleDelete = useCallback(
    async (attachment: FileAttachmentDto) => {
      Modal.confirm({
        title: '确认删除',
        icon: <ExclamationCircleOutlined />,
        content: `确定要删除文件 "${attachment.originalFileName}" 吗？`,
        okText: '确定',
        cancelText: '取消',
        okType: 'danger',
        onOk: async () => {
          setDeleting(attachment.id);
          try {
            await deleteFile(attachment.id);
            setAttachments((prev) => prev.filter((a) => a.id !== attachment.id));
            message.success('删除成功');
            onDelete?.(attachment.id);
            onRefresh?.();
          } catch (error) {
            console.error('删除失败:', error);
            message.error('删除失败');
          } finally {
            setDeleting(null);
          }
        },
      });
    },
    [onDelete, onRefresh]
  );

  // 初始加载
  useEffect(() => {
    loadAttachments();
  }, [loadAttachments]);

  // 自动刷新
  useEffect(() => {
    if (!autoRefresh) return;

    const timer = setInterval(() => {
      loadAttachments();
    }, refreshInterval);

    return () => clearInterval(timer);
  }, [autoRefresh, refreshInterval, loadAttachments]);

  return (
    <div style={{ width: '100%' }}>
      <Space direction="vertical" style={{ width: '100%' }} size="large">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h3>附件列表 ({attachments.length})</h3>
          <Button onClick={loadAttachments} loading={loading}>
            刷新
          </Button>
        </div>

        <Spin spinning={loading}>
          {attachments.length === 0 ? (
            <Empty description="暂无附件" />
          ) : (
            <List
              itemLayout="horizontal"
              dataSource={attachments}
              renderItem={(attachment) => (
                <List.Item
                  key={attachment.id}
                  actions={[
                    <Button
                      key="preview"
                      type="link"
                      icon={<EyeOutlined />}
                      onClick={() => handlePreview(attachment)}
                      disabled={attachment.uploadStatus !== 'Completed'}
                    >
                      预览
                    </Button>,
                    <Button
                      key="download"
                      type="link"
                      icon={<DownloadOutlined />}
                      onClick={() => handleDownload(attachment)}
                      disabled={attachment.uploadStatus !== 'Completed'}
                    >
                      下载
                    </Button>,
                    <Button
                      key="delete"
                      type="link"
                      danger
                      icon={<DeleteOutlined />}
                      loading={deleting === attachment.id}
                      onClick={() => handleDelete(attachment)}
                    >
                      删除
                    </Button>,
                  ]}
                >
                  <List.Item.Meta
                    avatar={
                      <FileOutlined
                        style={{
                          fontSize: 32,
                          color: getFileIconColor(attachment.fileExtension),
                        }}
                      />
                    }
                    title={
                      <Space>
                        <span>{attachment.originalFileName}</span>
                        {getStatusTag(attachment.uploadStatus)}
                      </Space>
                    }
                    description={
                      <Space direction="vertical" size={2}>
                        <div>
                          <span style={{ color: '#999' }}>大小: </span>
                          {formatFileSize(attachment.fileSize)}
                        </div>
                        <div>
                          <span style={{ color: '#999' }}>类型: </span>
                          {attachment.contentType}
                        </div>
                        <div>
                          <span style={{ color: '#999' }}>上传时间: </span>
                          {formatDate(attachment.createdAt)}
                        </div>
                        {attachment.uploadStatus === 'Uploading' && (
                          <div>
                            <span style={{ color: '#999' }}>进度: </span>
                            {attachment.uploadedChunks} / {attachment.totalChunks} 分片
                          </div>
                        )}
                      </Space>
                    }
                  />
                </List.Item>
              )}
            />
          )}
        </Spin>
      </Space>
    </div>
  );
};

