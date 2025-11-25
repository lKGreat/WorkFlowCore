import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useConfigList } from '../../hooks/useConfigList';
import type { ConfigDto } from '../../types';

export default function ConfigManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleCreate,
    handleUpdate,
    handleDelete,
  } = useConfigList();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<ConfigDto | null>(null);
  const [form] = Form.useForm();

  const columns: ColumnsType<ConfigDto> = [
    {
      title: '参数主键',
      dataIndex: 'configId',
      key: 'configId',
      width: 100,
    },
    {
      title: '参数名称',
      dataIndex: 'configName',
      key: 'configName',
      width: 200,
    },
    {
      title: '参数键名',
      dataIndex: 'configKey',
      key: 'configKey',
      width: 200,
    },
    {
      title: '参数键值',
      dataIndex: 'configValue',
      key: 'configValue',
      width: 200,
      ellipsis: true,
    },
    {
      title: '系统内置',
      dataIndex: 'configType',
      key: 'configType',
      width: 100,
      render: (type: string) => (type === 'Y' ? '是' : '否'),
    },
    {
      title: '备注',
      dataIndex: 'remark',
      key: 'remark',
      ellipsis: true,
    },
    {
      title: '创建时间',
      dataIndex: 'creationTime',
      key: 'creationTime',
      width: 180,
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      fixed: 'right',
      render: (_: unknown, record: ConfigDto) => (
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
            title="确定删除此配置吗？"
            onConfirm={() => handleDelete([record.configId])}
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

  const handleEdit = (record: ConfigDto) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setIsModalOpen(true);
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await handleUpdate(editingRecord.configId, values);
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

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="参数设置"
        subTitle="管理系统参数配置"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新建参数
          </Button>
        }
      />

      <Table
        columns={columns}
        dataSource={data}
        loading={loading}
        rowKey="configId"
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
        title={editingRecord ? '编辑参数' : '新建参数'}
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ configType: 'N' }}
        >
          <Form.Item
            label="参数名称"
            name="configName"
            rules={[{ required: true, message: '请输入参数名称' }]}
          >
            <Input placeholder="请输入参数名称" />
          </Form.Item>

          <Form.Item
            label="参数键名"
            name="configKey"
            rules={[{ required: true, message: '请输入参数键名' }]}
          >
            <Input placeholder="请输入参数键名" />
          </Form.Item>

          <Form.Item
            label="参数键值"
            name="configValue"
            rules={[{ required: true, message: '请输入参数键值' }]}
          >
            <Input placeholder="请输入参数键值" />
          </Form.Item>

          <Form.Item
            label="系统内置"
            name="configType"
            rules={[{ required: true, message: '请选择系统内置' }]}
          >
            <Select>
              <Select.Option value="Y">是</Select.Option>
              <Select.Option value="N">否</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item label="备注" name="remark">
            <Input.TextArea rows={4} placeholder="请输入备注" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

