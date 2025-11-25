import { useState } from 'react';
import { Button, Input, Space, message } from 'antd';
import { PlusOutlined, SearchOutlined } from '@ant-design/icons';
import { PageHeader } from '@/components/PageHeader';
import { UserTable } from '../../components/UserTable';
import { useUserList } from '../../hooks/useUserList';
import type { UserListItem } from '../../types';

const { Search } = Input;

export default function UserManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSearch,
    handleDelete,
    handleToggleStatus,
    handleResetPassword,
  } = useUserList();

  const [, setSelectedUser] = useState<UserListItem | null>(null);

  const handleEdit = (record: UserListItem) => {
    setSelectedUser(record);
    message.info('编辑功能开发中...');
  };

  const handleCreate = () => {
    setSelectedUser(null);
    message.info('新建功能开发中...');
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="用户管理"
        subTitle="管理系统用户及其权限"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
            新建用户
          </Button>
        }
      />

      <Space style={{ marginBottom: 16 }}>
        <Search
          placeholder="搜索用户名、姓名、邮箱"
          allowClear
          enterButton={<SearchOutlined />}
          onSearch={handleSearch}
          style={{ width: 300 }}
        />
      </Space>

      <UserTable
        data={data}
        loading={loading}
        pagination={pagination}
        onPageChange={handlePageChange}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onToggleStatus={handleToggleStatus}
        onResetPassword={handleResetPassword}
      />
    </div>
  );
}
