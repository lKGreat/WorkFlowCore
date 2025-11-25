import { Table, Button, Space, Tag } from 'antd';
import { EditOutlined, HistoryOutlined, DeleteOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import type { ProcessDefinitionListItem } from '../types';
import { showDeleteConfirm } from '@/components/ConfirmModal';

type ProcessTableProps = {
  data: ProcessDefinitionListItem[];
  loading: boolean;
  pagination: TablePaginationConfig;
  onPageChange: (page: number, pageSize: number) => void;
  onDelete: (id: number, name: string) => void;
};

export function ProcessTable({
  data,
  loading,
  pagination,
  onPageChange,
  onDelete,
}: ProcessTableProps) {
  const navigate = useNavigate();

  const handleDelete = (record: ProcessDefinitionListItem) => {
    showDeleteConfirm(
      () => onDelete(record.id, record.name),
      `流程 "${record.name}"`
    );
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
      render: (_: unknown, record: ProcessDefinitionListItem) => (
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
    <Table
      columns={columns}
      dataSource={data}
      rowKey="id"
      loading={loading}
      pagination={{
        ...pagination,
        showSizeChanger: true,
        showTotal: (total) => `共 ${total} 条`,
        onChange: onPageChange,
      }}
      scroll={{ x: 1200 }}
    />
  );
}

