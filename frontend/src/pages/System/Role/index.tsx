import React, { useState, useEffect } from 'react';
import { Table, Button, message, Tag } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { httpClient } from '../../../api/httpClient';

const RoleManagement: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<any[]>([]);
  const [total, setTotal] = useState(0);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  useEffect(() => {
    loadData();
  }, [pageIndex, pageSize]);

  const loadData = async () => {
    setLoading(true);
    try {
      const response = await httpClient.get('/system/role/list', {
        params: { pageIndex, pageSize }
      });
      setData(response.data.data?.items || []);
      setTotal(response.data.data?.total || 0);
    } finally {
      setLoading(false);
    }
  };

  const columns: any[] = [
    { title: '角色名称', dataIndex: 'roleName', key: 'roleName' },
    { title: '角色标识', dataIndex: 'roleKey', key: 'roleKey' },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={status === '0' ? 'success' : 'error'}>
          {status === '0' ? '正常' : '停用'}
        </Tag>
      ),
    },
    {
      title: '创建时间',
      dataIndex: 'creationTime',
      key: 'creationTime',
      render: (text: string) => new Date(text).toLocaleString(),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Button type="primary" icon={<PlusOutlined />} onClick={() => message.info('功能开发中')} style={{ marginBottom: 16 }}>
        新增角色
      </Button>

      <Table
        rowKey="roleId"
        columns={columns}
        dataSource={data}
        loading={loading}
        pagination={{
          current: pageIndex,
          pageSize: pageSize,
          total: total,
          onChange: (page, size) => {
            setPageIndex(page);
            setPageSize(size);
          },
        }}
      />
    </div>
  );
};

export default RoleManagement;

