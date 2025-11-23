import React, { useState, useEffect } from 'react';
import { Table, Button, Space, Input, Form, message, Modal, Tag, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined, ReloadOutlined, KeyOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getUserList, deleteUsers, changeStatus, resetPassword } from '../../../services/userService';
import type { UserListDto, UserPagedRequest } from '../../../types/user.types';
import UserForm from './UserForm';

const UserManagement: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<UserListDto[]>([]);
  const [total, setTotal] = useState(0);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchForm] = Form.useForm();
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingUser, setEditingUser] = useState<UserListDto | null>(null);

  useEffect(() => {
    loadData();
  }, [pageIndex, pageSize]);

  const loadData = async (searchParams?: any) => {
    setLoading(true);
    try {
      const params: UserPagedRequest = {
        pageIndex,
        pageSize,
        ...searchParams
      };
      
      const result = await getUserList(params);
      setData(result.items);
      setTotal(result.totalCount);
    } catch (error) {
      console.error('加载用户列表失败:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    const values = searchForm.getFieldsValue();
    setPageIndex(1);
    loadData(values);
  };

  const handleReset = () => {
    searchForm.resetFields();
    setPageIndex(1);
    loadData();
  };

  const handleAdd = () => {
    setEditingUser(null);
    setModalVisible(true);
  };

  const handleEdit = (record: UserListDto) => {
    setEditingUser(record);
    setModalVisible(true);
  };

  const handleDelete = async (ids: string[]) => {
    try {
      await deleteUsers(ids);
      message.success('删除成功');
      setSelectedRowKeys([]);
      loadData();
    } catch (error) {
      message.error('删除失败');
    }
  };

  const handleChangeStatus = async (userId: string, status: string) => {
    try {
      await changeStatus(userId, status === '0' ? '1' : '0');
      message.success('状态更新成功');
      loadData();
    } catch (error) {
      message.error('状态更新失败');
    }
  };

  const handleResetPassword = (userId: string) => {
    Modal.confirm({
      title: '重置密码',
      content: '确定要重置该用户密码为 123456 吗？',
      onOk: async () => {
        try {
          await resetPassword(userId, '123456');
          message.success('密码重置成功');
        } catch (error) {
          message.error('密码重置失败');
        }
      }
    });
  };

  const columns: ColumnsType<UserListDto> = [
    {
      title: '用户名',
      dataIndex: 'userName',
      key: 'userName',
      width: 120,
    },
    {
      title: '昵称',
      dataIndex: 'nickName',
      key: 'nickName',
      width: 120,
    },
    {
      title: '部门',
      dataIndex: 'departmentName',
      key: 'departmentName',
      width: 150,
    },
    {
      title: '手机号',
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
      width: 130,
    },
    {
      title: '角色',
      dataIndex: 'roles',
      key: 'roles',
      width: 200,
      render: (roles: string[]) => (
        <>
          {roles.map(role => (
            <Tag color="blue" key={role}>{role}</Tag>
          ))}
        </>
      ),
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
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
      width: 180,
      render: (text: string) => new Date(text).toLocaleString(),
    },
    {
      title: '操作',
      key: 'action',
      fixed: 'right',
      width: 250,
      render: (_, record) => (
        <Space size="small">
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            编辑
          </Button>
          <Button
            type="link"
            size="small"
            icon={<KeyOutlined />}
            onClick={() => handleResetPassword(record.userId)}
          >
            重置密码
          </Button>
          <Button
            type="link"
            size="small"
            onClick={() => handleChangeStatus(record.userId, record.status)}
          >
            {record.status === '0' ? '停用' : '启用'}
          </Button>
          <Popconfirm
            title="确定删除该用户吗？"
            onConfirm={() => handleDelete([record.userId])}
          >
            <Button
              type="link"
              size="small"
              danger
              icon={<DeleteOutlined />}
            >
              删除
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Form form={searchForm} layout="inline" style={{ marginBottom: 16 }}>
        <Form.Item name="userName" label="用户名">
          <Input placeholder="请输入用户名" style={{ width: 200 }} />
        </Form.Item>
        <Form.Item name="phoneNumber" label="手机号">
          <Input placeholder="请输入手机号" style={{ width: 200 }} />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" icon={<SearchOutlined />} onClick={handleSearch}>
              搜索
            </Button>
            <Button icon={<ReloadOutlined />} onClick={handleReset}>
              重置
            </Button>
          </Space>
        </Form.Item>
      </Form>

      <div style={{ marginBottom: 16 }}>
        <Space>
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新增用户
          </Button>
          {selectedRowKeys.length > 0 && (
            <Popconfirm
              title={`确定删除选中的 ${selectedRowKeys.length} 个用户吗？`}
              onConfirm={() => handleDelete(selectedRowKeys as string[])}
            >
              <Button danger icon={<DeleteOutlined />}>
                批量删除
              </Button>
            </Popconfirm>
          )}
        </Space>
      </div>

      <Table
        rowKey="userId"
        columns={columns}
        dataSource={data}
        loading={loading}
        rowSelection={{
          selectedRowKeys,
          onChange: setSelectedRowKeys,
        }}
        pagination={{
          current: pageIndex,
          pageSize: pageSize,
          total: total,
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (total) => `共 ${total} 条`,
          onChange: (page, size) => {
            setPageIndex(page);
            setPageSize(size);
          },
        }}
        scroll={{ x: 1400 }}
      />

      <UserForm
        visible={modalVisible}
        editingUser={editingUser}
        onCancel={() => {
          setModalVisible(false);
          setEditingUser(null);
        }}
        onSuccess={() => {
          setModalVisible(false);
          setEditingUser(null);
          loadData();
        }}
      />
    </div>
  );
};

export default UserManagement;

