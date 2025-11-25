import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, DatePicker, Popconfirm, Tag } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { PageHeader } from '@/components/PageHeader';
import { useNoticeList } from '../../hooks/useNoticeList';
import type { NoticeDto } from '../../types';

const { RangePicker } = DatePicker;

export default function NoticeManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleCreate,
    handleUpdate,
    handleDelete,
  } = useNoticeList();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<NoticeDto | null>(null);
  const [form] = Form.useForm();

  const noticeTypeMap: Record<number, { text: string; color: string }> = {
    1: { text: '通知', color: 'blue' },
    2: { text: '公告', color: 'green' },
  };

  const statusMap: Record<number, { text: string; color: string }> = {
    0: { text: '正常', color: 'success' },
    1: { text: '关闭', color: 'default' },
  };

  const columns: ColumnsType<NoticeDto> = [
    {
      title: '公告ID',
      dataIndex: 'noticeId',
      key: 'noticeId',
      width: 100,
    },
    {
      title: '公告标题',
      dataIndex: 'noticeTitle',
      key: 'noticeTitle',
      width: 250,
    },
    {
      title: '公告类型',
      dataIndex: 'noticeType',
      key: 'noticeType',
      width: 100,
      render: (type: number) => (
        <Tag color={noticeTypeMap[type]?.color}>{noticeTypeMap[type]?.text || '未知'}</Tag>
      ),
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
      title: '发布人',
      dataIndex: 'publisher',
      key: 'publisher',
      width: 120,
    },
    {
      title: '是否弹出',
      dataIndex: 'popup',
      key: 'popup',
      width: 100,
      render: (popup: number) => (popup === 1 ? '是' : '否'),
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
      render: (_: unknown, record: NoticeDto) => (
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
            title="确定删除此公告吗？"
            onConfirm={() => handleDelete([record.noticeId])}
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

  const handleEdit = (record: NoticeDto) => {
    setEditingRecord(record);
    const formValues = {
      ...record,
      timeRange: record.beginTime && record.endTime
        ? [dayjs(record.beginTime), dayjs(record.endTime)]
        : undefined,
    };
    form.setFieldsValue(formValues);
    setIsModalOpen(true);
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      const submitData = {
        ...values,
        beginTime: values.timeRange?.[0]?.format('YYYY-MM-DD HH:mm:ss'),
        endTime: values.timeRange?.[1]?.format('YYYY-MM-DD HH:mm:ss'),
        timeRange: undefined,
      };

      if (editingRecord) {
        await handleUpdate({ ...submitData, id: editingRecord.id, noticeId: editingRecord.noticeId });
      } else {
        await handleCreate(submitData);
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
        title="通知公告"
        subTitle="管理系统通知和公告"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新建公告
          </Button>
        }
      />

      <Table
        columns={columns}
        dataSource={data}
        loading={loading}
        rowKey="noticeId"
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
        title={editingRecord ? '编辑公告' : '新建公告'}
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        width={800}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ noticeType: 1, status: 0, popup: 0 }}
        >
          <Form.Item
            label="公告标题"
            name="noticeTitle"
            rules={[{ required: true, message: '请输入公告标题' }]}
          >
            <Input placeholder="请输入公告标题" />
          </Form.Item>

          <Form.Item
            label="公告类型"
            name="noticeType"
            rules={[{ required: true, message: '请选择公告类型' }]}
          >
            <Select>
              <Select.Option value={1}>通知</Select.Option>
              <Select.Option value={2}>公告</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="状态"
            name="status"
            rules={[{ required: true, message: '请选择状态' }]}
          >
            <Select>
              <Select.Option value={0}>正常</Select.Option>
              <Select.Option value={1}>关闭</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="是否弹出"
            name="popup"
          >
            <Select>
              <Select.Option value={0}>否</Select.Option>
              <Select.Option value={1}>是</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="显示时间范围"
            name="timeRange"
          >
            <RangePicker showTime style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            label="公告内容"
            name="noticeContent"
          >
            {/* TODO: 可以替换为富文本编辑器，如 React Quill */}
            <Input.TextArea rows={8} placeholder="请输入公告内容" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

