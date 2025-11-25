import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, InputNumber, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { usePostList } from '../../hooks/usePostList';
import type { PostDto } from '../../types';

const { Search } = Input;

export default function PostManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSearch,
    handleCreate,
    handleUpdate,
    handleDelete,
  } = usePostList();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<PostDto | null>(null);
  const [form] = Form.useForm();

  const columns: ColumnsType<PostDto> = [
    {
      title: '岗位编号',
      dataIndex: 'id',
      key: 'id',
      width: 100,
    },
    {
      title: '岗位编码',
      dataIndex: 'postCode',
      key: 'postCode',
      width: 150,
    },
    {
      title: '岗位名称',
      dataIndex: 'postName',
      key: 'postName',
      width: 150,
    },
    {
      title: '岗位排序',
      dataIndex: 'postSort',
      key: 'postSort',
      width: 100,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
      render: (status: string) => (status === '0' ? '正常' : '停用'),
    },
    {
      title: '用户数',
      dataIndex: 'userNum',
      key: 'userNum',
      width: 80,
    },
    {
      title: '创建时间',
      dataIndex: 'creationTime',
      key: 'creationTime',
      width: 180,
    },
    {
      title: '备注',
      dataIndex: 'remark',
      key: 'remark',
      ellipsis: true,
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      fixed: 'right',
      render: (_: unknown, record: PostDto) => (
        <Space>
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定删除此岗位吗？"
            onConfirm={() => handleDelete([record.id])}
            okText="确定"
            cancelText="取消"
          >
            <Button type="link" size="small" danger icon={<DeleteOutlined />}>
              删除
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleEdit = (record: PostDto) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setIsModalOpen(true);
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await handleUpdate({ ...values, id: editingRecord.id });
      } else {
        await handleCreate(values);
      }
      setIsModalOpen(false);
      form.resetFields();
    } catch (error) {
      // 表单验证失败或提交失败
    }
  };

  const handleModalCancel = () => {
    setIsModalOpen(false);
    form.resetFields();
  };

  const onSearch = (value: string) => {
    handleSearch({ postName: value });
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="岗位管理"
        subTitle="管理系统岗位信息"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新建岗位
          </Button>
        }
      />

      <Space style={{ marginBottom: 16 }}>
        <Search
          placeholder="搜索岗位名称"
          allowClear
          enterButton={<SearchOutlined />}
          onSearch={onSearch}
          style={{ width: 300 }}
        />
      </Space>

      <Table
        columns={columns}
        dataSource={data}
        loading={loading}
        rowKey="id"
        pagination={{
          current: pagination.current,
          pageSize: pagination.pageSize,
          total: pagination.total,
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (total) => `共 ${total} 条`,
          onChange: handlePageChange,
        }}
        bordered
      />

      <Modal
        title={editingRecord ? '编辑岗位' : '新建岗位'}
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: '0', postSort: 0 }}
        >
          <Form.Item
            label="岗位名称"
            name="postName"
            rules={[{ required: true, message: '请输入岗位名称' }]}
          >
            <Input placeholder="请输入岗位名称" />
          </Form.Item>

          <Form.Item
            label="岗位编码"
            name="postCode"
            rules={[{ required: true, message: '请输入岗位编码' }]}
          >
            <Input placeholder="请输入岗位编码" />
          </Form.Item>

          <Form.Item
            label="岗位排序"
            name="postSort"
          >
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            label="状态"
            name="status"
            rules={[{ required: true, message: '请选择状态' }]}
          >
            <Select>
              <Select.Option value="0">正常</Select.Option>
              <Select.Option value="1">停用</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="备注"
            name="remark"
          >
            <Input.TextArea rows={4} placeholder="请输入备注" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

