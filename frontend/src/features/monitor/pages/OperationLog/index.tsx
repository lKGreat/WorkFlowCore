import { Button, Space, Table, Form, Input, DatePicker, Popconfirm } from 'antd';
import { DeleteOutlined, ClearOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useOperationLogList } from '../../hooks/useOperationLogList';
import type { OperationLogDto } from '../../types';

const { RangePicker } = DatePicker;

export default function OperationLogManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSearch,
    handleDelete,
    handleClean,
  } = useOperationLogList();
  const [form] = Form.useForm();

  const columns: ColumnsType<OperationLogDto> = [
    {
      title: '日志编号',
      dataIndex: 'id',
      key: 'id',
      width: 100,
    },
    {
      title: '操作模块',
      dataIndex: 'operationModule',
      key: 'operationModule',
      width: 120,
    },
    {
      title: '操作类型',
      dataIndex: 'operationType',
      key: 'operationType',
      width: 100,
    },
    {
      title: '操作人员',
      dataIndex: 'operationUser',
      key: 'operationUser',
      width: 120,
    },
    {
      title: '请求方法',
      dataIndex: 'operationMethod',
      key: 'operationMethod',
      width: 150,
      ellipsis: true,
    },
    {
      title: '请求地址',
      dataIndex: 'operationUrl',
      key: 'operationUrl',
      width: 200,
      ellipsis: true,
    },
    {
      title: '操作IP',
      dataIndex: 'operationIp',
      key: 'operationIp',
      width: 140,
    },
    {
      title: '操作地点',
      dataIndex: 'operationLocation',
      key: 'operationLocation',
      width: 120,
    },
    {
      title: '耗时(ms)',
      dataIndex: 'operationDuration',
      key: 'operationDuration',
      width: 100,
    },
    {
      title: '操作时间',
      dataIndex: 'operationTime',
      key: 'operationTime',
      width: 180,
    },
    {
      title: '操作',
      key: 'action',
      width: 100,
      fixed: 'right',
      render: (_: unknown, record: OperationLogDto) => (
        <Popconfirm
          title="确定删除此日志吗？"
          onConfirm={() => handleDelete([record.id])}
          okText="确定"
          cancelText="取消"
        >
          <Button type="link" size="small" danger icon={<DeleteOutlined />}>
            删除
          </Button>
        </Popconfirm>
      ),
    },
  ];

  const onFinish = (values: { operationModule?: string; operationType?: string; operationUser?: string; timeRange?: [unknown, unknown] }) => {
    handleSearch({
      operationModule: values.operationModule,
      operationType: values.operationType,
      operationUser: values.operationUser,
      beginTime: values.timeRange?.[0] ? (values.timeRange[0] as { format: (s: string) => string }).format('YYYY-MM-DD HH:mm:ss') : undefined,
      endTime: values.timeRange?.[1] ? (values.timeRange[1] as { format: (s: string) => string }).format('YYYY-MM-DD HH:mm:ss') : undefined,
    });
  };

  const onReset = () => {
    form.resetFields();
    handleSearch({});
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="操作日志"
        subTitle="记录用户操作系统的日志"
        extra={
          <Popconfirm
            title="确定清空所有操作日志吗？"
            onConfirm={handleClean}
            okText="确定"
            cancelText="取消"
          >
            <Button danger icon={<ClearOutlined />}>
              清空日志
            </Button>
          </Popconfirm>
        }
      />

      <Form
        form={form}
        layout="inline"
        onFinish={onFinish}
        style={{ marginBottom: 16 }}
      >
        <Form.Item name="operationModule">
          <Input placeholder="操作模块" style={{ width: 150 }} />
        </Form.Item>

        <Form.Item name="operationType">
          <Input placeholder="操作类型" style={{ width: 120 }} />
        </Form.Item>

        <Form.Item name="operationUser">
          <Input placeholder="操作人员" style={{ width: 150 }} />
        </Form.Item>

        <Form.Item name="timeRange">
          <RangePicker showTime />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit" icon={<SearchOutlined />}>
              搜索
            </Button>
            <Button onClick={onReset}>
              重置
            </Button>
          </Space>
        </Form.Item>
      </Form>

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
        scroll={{ x: 1600 }}
      />
    </div>
  );
}

