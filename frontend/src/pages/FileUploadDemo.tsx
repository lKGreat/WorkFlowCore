import React, { useState } from 'react';
import { Card, Space, Select, Input, Button, Divider } from 'antd';
import { FileUploadComponent } from '../components/FileUploadComponent';
import { FileAttachmentList } from '../components/FileAttachmentList';
import type { FileAttachmentDto } from '../types/file.types';

/**
 * 文件上传功能演示页面
 * 展示如何使用 FileUploadComponent 和 FileAttachmentList 组件
 */
export const FileUploadDemo: React.FC = () => {
  const [businessType, setBusinessType] = useState('Purchase');
  const [businessId, setBusinessId] = useState('');
  const [storageProviderId] = useState<string | undefined>(undefined);
  const [refreshKey, setRefreshKey] = useState(0);

  // 生成测试业务 ID
  const generateBusinessId = () => {
    setBusinessId(`TEST-${Date.now()}`);
  };

  return (
    <div style={{ padding: 24 }}>
      <Space direction="vertical" size="large" style={{ width: '100%' }}>
        <Card title="文件上传演示" bordered={false}>
          <Space direction="vertical" size="middle" style={{ width: '100%' }}>
            {/* 配置区域 */}
            <div>
              <h4>上传配置</h4>
              <Space wrap>
                <span>业务类型:</span>
                <Select
                  value={businessType}
                  onChange={setBusinessType}
                  style={{ width: 200 }}
                  options={[
                    { label: '采购单', value: 'Purchase' },
                    { label: '入库单', value: 'Warehouse' },
                    { label: '请假申请', value: 'Leave' },
                    { label: '报销单', value: 'Expense' },
                  ]}
                />

                <span>业务单据ID:</span>
                <Input
                  value={businessId}
                  onChange={(e) => setBusinessId(e.target.value)}
                  placeholder="输入业务单据ID"
                  style={{ width: 200 }}
                />
                <Button onClick={generateBusinessId}>生成测试ID</Button>
              </Space>
            </div>

            <Divider />

            {/* 上传组件 */}
            <div>
              <h4>文件上传</h4>
              {businessId ? (
                <FileUploadComponent
                  businessType={businessType}
                  businessId={businessId}
                  storageProviderId={storageProviderId}
                  maxSize={5 * 1024 * 1024 * 1024} // 5GB
                  maxCount={10}
                  multiple={true}
                  onUploadSuccess={(attachments: FileAttachmentDto[]) => {
                    console.log('上传成功:', attachments);
                    // 刷新附件列表
                    setRefreshKey((prev) => prev + 1);
                  }}
                  onUploadError={(error: Error) => {
                    console.error('上传失败:', error);
                  }}
                />
              ) : (
                <div style={{ color: '#999' }}>请先输入或生成业务单据ID</div>
              )}
            </div>

            <Divider />

            {/* 附件列表 */}
            <div>
              <h4>附件列表</h4>
              {businessId ? (
                <FileAttachmentList
                  key={refreshKey}
                  businessType={businessType}
                  businessId={businessId}
                  autoRefresh={false}
                  onDelete={(attachmentId) => {
                    console.log('删除附件:', attachmentId);
                  }}
                  onRefresh={() => {
                    setRefreshKey((prev) => prev + 1);
                  }}
                />
              ) : (
                <div style={{ color: '#999' }}>请先输入或生成业务单据ID</div>
              )}
            </div>
          </Space>
        </Card>

        {/* 使用说明 */}
        <Card title="使用说明" bordered={false}>
          <Space direction="vertical" size="small">
            <div>
              <strong>FileUploadComponent 组件参数：</strong>
              <ul>
                <li>businessType: 业务类型（必填）</li>
                <li>businessId: 业务单据ID（选填，用于关联业务）</li>
                <li>storageProviderId: 指定存储提供者（选填）</li>
                <li>maxSize: 最大文件大小限制（字节）</li>
                <li>accept: 接受的文件类型（如 ".jpg,.png,.pdf"）</li>
                <li>maxCount: 最大文件数量</li>
                <li>multiple: 是否支持多文件上传</li>
                <li>onUploadSuccess: 上传成功回调</li>
                <li>onUploadError: 上传失败回调</li>
              </ul>
            </div>
            <div>
              <strong>FileAttachmentList 组件参数：</strong>
              <ul>
                <li>businessType: 业务类型（必填）</li>
                <li>businessId: 业务单据ID（必填）</li>
                <li>autoRefresh: 是否自动刷新列表</li>
                <li>refreshInterval: 刷新间隔（毫秒）</li>
                <li>onDelete: 删除回调</li>
                <li>onRefresh: 刷新回调</li>
              </ul>
            </div>
            <div>
              <strong>功能特性：</strong>
              <ul>
                <li>支持大文件分片上传（默认 5MB/片）</li>
                <li>支持断点续传（刷新页面后可恢复）</li>
                <li>支持并发上传（最大 3 个分片并发）</li>
                <li>自动计算文件 MD5 防止重复上传</li>
                <li>上传失败自动重试（最多 3 次）</li>
                <li>实时显示上传进度（整体进度 + 分片进度）</li>
                <li>支持取消上传</li>
                <li>支持文件下载和预览</li>
              </ul>
            </div>
          </Space>
        </Card>
      </Space>
    </div>
  );
};

