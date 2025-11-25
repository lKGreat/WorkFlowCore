import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, InputNumber, Popconfirm, Tag } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useTaskList } from '../../hooks/useTaskList';
import type { TaskDto } from '../../types';

export default function TaskManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleCreate,
    handleUpdate,
    handleDelete,
  } = useTaskList();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<TaskDto | null>(null);
  const [form] = Form.useForm();

  const statusMap: Record<number, { text: string; color: string }> = {
    0: { text: '正常', color: 'success' },
    1: { text: '暂停', color: 'default' },
  };

  const columns: ColumnsType<TaskDto> = [
    {
      title: '任务编号',
      dataIndex: 'id',
      key: 'id',
      width: 100,
    },
    {
      title: '任务名称',
      dataIndex: 'taskName',
      key: 'taskName',
      width: 150,
    },
    {
      title: '任务组名',
      dataIndex: 'taskGroup',
      key: 'taskGroup',
      width: 120,
    },
    {
      title: '调用目标',
      dataIndex: 'invokeTarget',
      key: 'invokeTarget',
      width: 200,
      ellipsis: true,
    },
    {
      title: 'cron表达式',
      dataIndex: 'cronExpression',
      key: 'cronExpression',
      width: 150,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
      render: (status: number) => (
        <Tag color={statusMap[status]?.color}>{statusMap[status]?.text || '未知'}</Tag>
      ),
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
      render: (_: unknown, record: TaskDto) => (
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
            title="确定删除此任务吗？"
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

  const handleEdit = (record: TaskDto) => {
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

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="定时任务"
        subTitle="管理系统定时任务"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新建任务
          </Button>
        }
      />

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
        scroll={{ x: 1200 }}
      />

      <Modal
        title={editingRecord ? '编辑任务' : '新建任务'}
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        width={700}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ taskGroup: 'DEFAULT', status: 0, concurrent: 0, misfirePolicy: 0 }}
        >
          <Form.Item
            label="任务名称"
            name="taskName"
            rules={[{ required: true, message: '请输入任务名称' }]}
          >
            <Input placeholder="请输入任务名称" />
          </Form.Item>

          <Form.Item
            label="任务组名"
            name="taskGroup"
            rules={[{ required: true, message: '请输入任务组名' }]}
          >
            <Input placeholder="请输入任务组名" />
          </Form.Item>

          <Form.Item
            label="调用目标"
            name="invokeTarget"
            rules={[{ required: true, message: '请输入调用目标' }]}
            tooltip="Bean调用示例：myBean.myMethod(params)"
          >
            <Input placeholder="请输入调用目标" />
          </Form.Item>

          <Form.Item
            label="cron表达式"
            name="cronExpression"
            rules={[{ required: true, message: '请输入cron表达式' }]}
            tooltip="示例：0/5 * * * * ? 表示每5秒执行一次"
          >
            <Input placeholder="请输入cron表达式" />
          </Form.Item>

          <Form.Item
            label="状态"
            name="status"
            rules={[{ required: true, message: '请选择状态' }]}
          >
            <Select>
              <Select.Option value={0}>正常</Select.Option>
              <Select.Option value={1}>暂停</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="是否并发"
            name="concurrent"
          >
            <Select>
              <Select.Option value={0}>禁止</Select.Option>
              <Select.Option value={1}>允许</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="错失策略"
            name="misfirePolicy"
          >
            <Select>
              <Select.Option value={0}>默认</Select.Option>
              <Select.Option value={1}>立即执行</Select.Option>
              <Select.Option value={2}>执行一次</Select.Option>
              <Select.Option value={3}>放弃执行</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item label="备注" name="remark">
            <Input.TextArea rows={3} placeholder="请输入备注" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

