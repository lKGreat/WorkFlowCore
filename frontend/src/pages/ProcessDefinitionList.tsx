import React, { useEffect, useState } from 'react';
import { Table, Button, Space, Modal, message, Tag, Input } from 'antd';
import { PlusOutlined, EditOutlined, HistoryOutlined, DeleteOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import type { ColumnsType } from 'antd/es/table';
import { processDefinitionService } from '../services/processDefinitionService';
import type { ProcessDefinitionListItem } from '../types/processDefinition.types';

const { Search } = Input;

export const ProcessDefinitionList: React.FC = () => {
  const [data, setData] = useState<ProcessDefinitionListItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  
  const navigate = useNavigate();

  useEffect(() => {
    loadData();
  }, [pagination.current, pagination.pageSize]);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await processDefinitionService.getProcessDefinitions(
        pagination.current,
        pagination.pageSize
      );
      
      setData(response.items);
      setPagination({
        ...pagination,
        total: response.totalCount,
      });
    } catch (error: any) {
      message.error(error.message || '加载数据失败');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = (record: ProcessDefinitionListItem) => {
    Modal.confirm({
      title: '确认删除',
      content: `确定要删除流程 "${record.name}" 吗？`,
      okText: '确定',
      cancelText: '取消',
      onOk: async () => {
        try {
          await processDefinitionService.deleteProcessDefinition(record.id);
          message.success('删除成功');
          loadData();
        } catch (error: any) {
          message.error(error.message || '删除失败');
        }
      },
    });
  };

  const columns: ColumnsType<ProcessDefinitionListItem> = [
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
      title: '版本',
      dataIndex: 'version',
      key: 'version',
      width: 80,
      render: (version: number) => <Tag color="blue">V{version}</Tag>,
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
      title: '操作',
      key: 'action',
      width: 250,
      fixed: 'right',
      render: (_: any, record: ProcessDefinitionListItem) => (
        <Space size="small">
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => navigate(`/designer/${record.id}`)}
          >
            编辑
          </Button>
          <Button
            type="link"
            size="small"
            icon={<HistoryOutlined />}
            onClick={() => navigate(`/versions/${record.key}`)}
          >
            版本历史
          </Button>
          <Button
            type="link"
            size="small"
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDelete(record)}
          >
            删除
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ marginBottom: '16px', display: 'flex', justifyContent: 'space-between' }}>
        <h2>流程定义管理</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => navigate('/designer')}
        >
          新建流程
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={data}
        rowKey="id"
        loading={loading}
        pagination={{
          ...pagination,
          showSizeChanger: true,
          showTotal: (total) => `共 ${total} 条`,
        }}
        onChange={(newPagination) => {
          setPagination({
            current: newPagination.current || 1,
            pageSize: newPagination.pageSize || 10,
            total: pagination.total,
          });
        }}
        scroll={{ x: 1200 }}
      />
    </div>
  );
};

