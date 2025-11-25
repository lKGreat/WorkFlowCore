import { Table, Button, Space, Tag, Switch } from 'antd';
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
  onDelete: (id: number) => void;
  onToggleStatus: (id: number, isActive: boolean) => void;
  onResetPassword: (id: number) => void;
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
      () => onDelete(record.id),
      `用户 "${record.fullName}"`
    );
  };

  const columns: ColumnsType<UserListItem> = [
    {
      title: '用户名',
      dataIndex: 'username',
      key: 'username',
      width: 120,
      fixed: 'left',
    },
    {
      title: '姓名',
      dataIndex: 'fullName',
      key: 'fullName',
      width: 120,
    },
    {
      title: '邮箱',
      dataIndex: 'email',
      key: 'email',
      width: 180,
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
      title: '角色',
      dataIndex: 'roles',
      key: 'roles',
      width: 200,
      render: (roles: string[]) => (
        <>
          {roles.map((role) => (
            <Tag key={role} color="blue">
              {role}
            </Tag>
          ))}
        </>
      ),
    },
    {
      title: '状态',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive: boolean, record: UserListItem) => (
        <Switch
          checked={isActive}
          onChange={() => onToggleStatus(record.id, isActive)}
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
            onClick={() => onResetPassword(record.id)}
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
      rowKey="id"
      loading={loading}
      pagination={{
        ...pagination,
        showSizeChanger: true,
        showTotal: (total) => `共 ${total} 条`,
        onChange: onPageChange,
      }}
      scroll={{ x: 1600 }}
    />
  );
}

