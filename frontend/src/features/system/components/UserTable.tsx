import { Table, Button, Space, Switch } from 'antd';
import { EditOutlined, DeleteOutlined, KeyOutlined } from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import type { UserListItem } from '../types';
import { showDeleteConfirm } from '@/components/ConfirmModal';

type UserTableProps = {
  data: UserListItem[];
  loading: boolean;
  pagination: TablePaginationConfig;
  onPageChange: (page: number, pageSize: number) => void;
  onEdit: (record: UserListItem) => void;
  onDelete: (id: string) => void;
  onToggleStatus: (id: string, status: string) => void;
  onResetPassword: (id: string) => void;
};

export function UserTable({
  data,
  loading,
  pagination,
  onPageChange,
  onEdit,
  onDelete,
  onToggleStatus,
  onResetPassword,
}: UserTableProps) {
  const handleDelete = (record: UserListItem) => {
    showDeleteConfirm(
      () => onDelete(record.userId),
      `用户 "${record.nickName || record.userName}"`
    );
  };

  const columns: ColumnsType<UserListItem> = [
    {
      title: '用户名',
      dataIndex: 'userName',
      key: 'userName',
      width: 120,
      fixed: 'left',
    },
    {
      title: '昵称',
      dataIndex: 'nickName',
      key: 'nickName',
      width: 120,
    },
    {
      title: '邮箱',
      dataIndex: 'email',
      key: 'email',
      width: 180,
      render: (text?: string) => text || '-',
    },
    {
      title: '手机号',
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
      width: 120,
      render: (text?: string) => text || '-',
    },
    {
      title: '部门',
      dataIndex: 'departmentName',
      key: 'departmentName',
      width: 150,
      render: (text?: string) => text || '-',
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: string, record: UserListItem) => (
        <Switch
          checked={status === '0'}
          onChange={() => onToggleStatus(record.userId, status)}
          checkedChildren="启用"
          unCheckedChildren="禁用"
        />
      ),
    },
    {
      title: '最后登录',
      dataIndex: 'lastLoginTime',
      key: 'lastLoginTime',
      width: 180,
      render: (date?: string) =>
        date ? new Date(date).toLocaleString('zh-CN') : '-',
    },
    {
      title: '操作',
      key: 'action',
      width: 250,
      fixed: 'right',
      render: (_: unknown, record: UserListItem) => (
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
            icon={<KeyOutlined />}
            onClick={() => onResetPassword(record.userId)}
          >
            重置密码
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
      rowKey="userId"
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
