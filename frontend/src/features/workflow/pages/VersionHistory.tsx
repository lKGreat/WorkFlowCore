import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Table, Button, Tag, message, Space } from 'antd';
import { ArrowLeftOutlined, EyeOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { processDefinitionService } from '../services/processDefinitionService';
import type { VersionHistoryItem } from '../types';

export function VersionHistory() {
  const { key } = useParams<{ key: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<VersionHistoryItem[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!key) return;

    const loadData = async () => {
      try {
        setLoading(true);
        const response = await processDefinitionService.getVersionHistory(key);
        setData(response);
      } catch (error: unknown) {
        const err = error as Error;
        message.error(err.message || '加载版本历史失败');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [key]);

  const columns: ColumnsType<VersionHistoryItem> = [
    {
      title: '版本号',
      dataIndex: 'version',
      key: 'version',
      width: 100,
      render: (version: number) => <Tag color="blue">V{version}</Tag>,
    },
    {
      title: '描述',
      dataIndex: 'description',
      key: 'description',
      ellipsis: true,
      render: (text?: string) => text || '-',
    },
    {
      title: '状态',
      dataIndex: 'isEnabled',
      key: 'isEnabled',
      width: 100,
      render: (isEnabled: boolean) => (
        <Tag color={isEnabled ? 'green' : 'red'}>
          {isEnabled ? '已启用' : '已禁用'}
        </Tag>
      ),
    },
    {
      title: '创建人',
      dataIndex: 'createdBy',
      key: 'createdBy',
      width: 120,
    },
    {
      title: '创建时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 180,
      render: (date: string) => new Date(date).toLocaleString('zh-CN'),
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      fixed: 'right',
      render: (_: unknown, record: VersionHistoryItem) => (
        <Space size="small">
          <Button
            type="link"
            size="small"
            icon={<EyeOutlined />}
            onClick={() => navigate(`/designer/${record.id}`)}
          >
            查看详情
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title={`流程版本历史：${key || ''}`}
        extra={
          <Button icon={<ArrowLeftOutlined />} onClick={() => navigate('/')}>
            返回
          </Button>
        }
      />

      <Table
        columns={columns}
        dataSource={data}
        rowKey="id"
        loading={loading}
        pagination={{
          pageSize: 10,
          showSizeChanger: true,
          showTotal: (total) => `共 ${total} 个版本`,
        }}
        scroll={{ x: 1000 }}
      />
    </div>
  );
}
