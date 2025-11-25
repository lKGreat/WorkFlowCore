import { useState } from 'react';
import { Button, message } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { PageHeader } from '@/components/PageHeader';
import { RoleTable } from '../../components/RoleTable';
import { useRoleList } from '../../hooks/useRoleList';
import type { RoleListItem } from '../../types';

export default function RoleManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleDelete,
  } = useRoleList();

  const [, setSelectedRole] = useState<RoleListItem | null>(null);

  const handleEdit = (record: RoleListItem) => {
    setSelectedRole(record);
    message.info('编辑功能开发中...');
  };

  const handleCreate = () => {
    setSelectedRole(null);
    message.info('新建功能开发中...');
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="角色管理"
        subTitle="管理系统角色及其权限配置"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
            新建角色
          </Button>
        }
      />

      <RoleTable
        data={data}
        loading={loading}
        pagination={pagination}
        onPageChange={handlePageChange}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
    </div>
  );
}
