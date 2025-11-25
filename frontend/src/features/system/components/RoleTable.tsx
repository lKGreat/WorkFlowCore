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
  onDelete: (id: string) => void;
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
    showDeleteConfirm(
      () => onDelete(record.roleId),
      `角色 "${record.roleName}"`
    );
  };

  const columns: ColumnsType<RoleListItem> = [
    {
      title: '角色名称',
      dataIndex: 'roleName',
      key: 'roleName',
      width: 150,
      fixed: 'left',
    },
    {
      title: '角色标识',
      dataIndex: 'roleKey',
      key: 'roleKey',
      width: 150,
    },
    {
      title: '排序',
      dataIndex: 'roleSort',
      key: 'roleSort',
      width: 80,
    },
    {
      title: '备注',
      dataIndex: 'remark',
      key: 'remark',
      ellipsis: true,
      render: (text?: string) => text || '-',
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: string) => (
        <Tag color={status === '0' ? 'green' : 'red'}>
          {status === '0' ? '正常' : '停用'}
        </Tag>
      ),
    },
    {
      title: '创建时间',
      dataIndex: 'creationTime',
      key: 'creationTime',
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
      rowKey="roleId"
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
