import { Table, Button, Space, Tag } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import type { RoleListItem } from '../types';
import { showDeleteConfirm } from '@/components/ConfirmModal';

type RoleTableProps = {
  data: RoleListItem[];
  loading: boolean;
  pagination: TablePaginationConfig;
  onPageChange: (page: number, pageSize: number) => void;
  onEdit: (record: RoleListItem) => void;
  onDelete: (id: number) => void;
};

export function RoleTable({
  data,
  loading,
  pagination,
  onPageChange,
  onEdit,
  onDelete,
}: RoleTableProps) {
  const handleDelete = (record: RoleListItem) => {
    if (record.isDefault) {
      return;
    }
    showDeleteConfirm(
      () => onDelete(record.id),
      `角色 "${record.name}"`
    );
  };

  const columns: ColumnsType<RoleListItem> = [
    {
      title: '角色名称',
      dataIndex: 'name',
      key: 'name',
      width: 150,
      fixed: 'left',
    },
    {
      title: '角色代码',
      dataIndex: 'code',
      key: 'code',
      width: 150,
    },
    {
      title: '描述',
      dataIndex: 'description',
      key: 'description',
      ellipsis: true,
      render: (text?: string) => text || '-',
    },
    {
      title: '权限数量',
      dataIndex: 'permissions',
      key: 'permissions',
      width: 100,
      render: (permissions: string[]) => permissions.length,
    },
    {
      title: '用户数量',
      dataIndex: 'userCount',
      key: 'userCount',
      width: 100,
    },
    {
      title: '类型',
      dataIndex: 'isDefault',
      key: 'isDefault',
      width: 100,
      render: (isDefault: boolean) => (
        <Tag color={isDefault ? 'gold' : 'default'}>
          {isDefault ? '系统' : '自定义'}
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
      width: 180,
      fixed: 'right',
      render: (_: unknown, record: RoleListItem) => (
        <Space size="small">
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => onEdit(record)}
          >
            编辑
          </Button>
          {!record.isDefault && (
            <Button
              type="link"
              size="small"
              danger
              icon={<DeleteOutlined />}
              onClick={() => handleDelete(record)}
            >
              删除
            </Button>
          )}
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
      scroll={{ x: 1400 }}
    />
  );
}

