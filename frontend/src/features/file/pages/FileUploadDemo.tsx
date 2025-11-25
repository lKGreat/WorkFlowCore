import { useState } from 'react';
import { Upload, Button, Card, message, Space, Progress, Table, Tag } from 'antd';
import { InboxOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { fileService } from '../services/fileService';
import type { FileInfo, FileUploadStatus } from '../types';
import { FileUploadStatus as Status } from '../types';

const { Dragger } = Upload;

export function FileUploadDemo() {
  const [fileList, setFileList] = useState<FileInfo[]>([]);

  const handleUpload = async (file: File) => {
    const fileInfo: FileInfo = {
      id: `temp-${Date.now()}`,
      name: file.name,
      size: file.size,
      type: file.type,
      uploadProgress: 0,
      status: Status.Uploading,
    };

    setFileList((prev) => [...prev, fileInfo]);

    try {
      const result = await fileService.uploadFile(file);
      
      setFileList((prev) =>
        prev.map((f) =>
          f.id === fileInfo.id
            ? {
                ...f,
                id: result.id,
                url: result.storagePath,
                uploadProgress: 100,
                status: Status.Success,
              }
            : f
        )
      );

      message.success(`${file.name} 上传成功`);
    } catch (error: unknown) {
      const err = error as Error;
      setFileList((prev) =>
        prev.map((f) =>
          f.id === fileInfo.id
            ? {
                ...f,
                status: Status.Error,
                error: err.message || '上传失败',
              }
            : f
        )
      );
      message.error(`${file.name} 上传失败`);
    }
  };

  const handleDelete = (id: string) => {
    setFileList((prev) => prev.filter((f) => f.id !== id));
    message.success('已删除');
  };

  const statusColorMap: Record<FileUploadStatus, string> = {
    [Status.Pending]: 'default',
    [Status.Uploading]: 'processing',
    [Status.Success]: 'success',
    [Status.Error]: 'error',
  };

  const statusTextMap: Record<FileUploadStatus, string> = {
    [Status.Pending]: '待上传',
    [Status.Uploading]: '上传中',
    [Status.Success]: '成功',
    [Status.Error]: '失败',
  };

  const columns: ColumnsType<FileInfo> = [
    {
      title: '文件名',
      dataIndex: 'name',
      key: 'name',
      width: 300,
      ellipsis: true,
    },
    {
      title: '大小',
      dataIndex: 'size',
      key: 'size',
      width: 120,
      render: (size: number) => `${(size / 1024).toFixed(2)} KB`,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: FileUploadStatus) => (
        <Tag color={statusColorMap[status]}>{statusTextMap[status]}</Tag>
      ),
    },
    {
      title: '进度',
      dataIndex: 'uploadProgress',
      key: 'uploadProgress',
      width: 200,
      render: (progress: number, record: FileInfo) =>
        record.status === Status.Uploading ? (
          <Progress percent={progress} size="small" />
        ) : null,
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      render: (_: unknown, record: FileInfo) => (
        <Space size="small">
          {record.url && (
            <a href={record.url} target="_blank" rel="noopener noreferrer">
              查看
            </a>
          )}
          <Button
            type="link"
            size="small"
            danger
            onClick={() => handleDelete(record.id)}
          >
            删除
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader title="文件上传示例" subTitle="演示简单上传和分片上传功能" />

      <Card title="文件上传" style={{ marginBottom: 24 }}>
        <Dragger
          multiple
          showUploadList={false}
          beforeUpload={(file: File) => {
            handleUpload(file);
            return false;
          }}
        >
          <p className="ant-upload-drag-icon">
            <InboxOutlined />
          </p>
          <p className="ant-upload-text">点击或拖拽文件到此处上传</p>
          <p className="ant-upload-hint">支持单个或批量上传</p>
        </Dragger>
      </Card>

      <Card title="上传记录">
        <Table
          columns={columns}
          dataSource={fileList}
          rowKey="id"
          pagination={{
            pageSize: 10,
            showTotal: (total) => `共 ${total} 个文件`,
          }}
        />
      </Card>
    </div>
  );
}
