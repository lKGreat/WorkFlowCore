import React, { useEffect, useState } from 'react';
import { Table, Button, Space, message, Tag, Card } from 'antd';
import { ArrowLeftOutlined, EditOutlined } from '@ant-design/icons';
import { useNavigate, useParams } from 'react-router-dom';
import type { ColumnsType } from 'antd/es/table';
import { processDefinitionService } from '../services/processDefinitionService';
import type { ProcessDefinitionVersion } from '../types/processDefinition.types';

export const VersionHistory: React.FC = () => {
  const [data, setData] = useState<ProcessDefinitionVersion[]>([]);
  const [loading, setLoading] = useState(false);
  const [processName, setProcessName] = useState('');
  
  const navigate = useNavigate();
  const { key } = useParams<{ key: string }>();

  useEffect(() => {
    if (key) {
      loadVersionHistory(key);
    }
  }, [key]);

  const loadVersionHistory = async (processKey: string) => {
    try {
      setLoading(true);
      const versions = await processDefinitionService.getVersionHistory(processKey);
      setData(versions);
      
      if (versions.length > 0) {
        setProcessName(versions[0].name);
      }
    } catch (error: any) {
      message.error(error.message || '加载版本历史失败');
    } finally {
      setLoading(false);
    }
  };

  const columns: ColumnsType<ProcessDefinitionVersion> = [
    {
      title: '版本号',
      dataIndex: 'version',
      key: 'version',
      width: 100,
      render: (version: number) => <Tag color="blue">V{version}</Tag>,
    },
    {
      title: '流程名称',
      dataIndex: 'name',
      key: 'name',
      width: 200,
    },
    {
      title: '流程Key',
      dataIndex: 'key',
      key: 'key',
      width: 150,
    },
    {
      title: '描述',
      dataIndex: 'description',
      key: 'description',
      ellipsis: true,
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
      title: '创建时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 180,
      render: (date: string) => new Date(date).toLocaleString('zh-CN'),
    },
    {
      title: '更新时间',
      dataIndex: 'updatedAt',
      key: 'updatedAt',
      width: 180,
      render: (date: string) => new Date(date).toLocaleString('zh-CN'),
    },
    {
      title: '操作',
      key: 'action',
      width: 120,
      fixed: 'right',
      render: (_: any, record: ProcessDefinitionVersion) => (
        <Space size="small">
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => navigate(`/designer/${record.id}`)}
          >
            编辑
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Card
        title={
          <Space>
            <Button
              type="text"
              icon={<ArrowLeftOutlined />}
              onClick={() => navigate('/')}
            >
              返回
            </Button>
            <span>版本历史 - {processName} ({key})</span>
          </Space>
        }
        bordered={false}
      >
        <Table
          columns={columns}
          dataSource={data}
          rowKey="id"
          loading={loading}
          pagination={false}
          scroll={{ x: 1200 }}
        />
      </Card>
    </div>
  );
};

