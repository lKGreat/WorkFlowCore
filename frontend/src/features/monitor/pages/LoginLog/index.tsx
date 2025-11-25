import { Button, Space, Table, Form, Input, Select, DatePicker, Popconfirm, Tag } from 'antd';
import { DeleteOutlined, ClearOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useLoginLogList } from '../../hooks/useLoginLogList';
import type { LoginLogDto } from '../../types';

const { RangePicker } = DatePicker;

export default function LoginLogManagement() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSearch,
    handleDelete,
    handleClean,
  } = useLoginLogList();
  const [form] = Form.useForm();

  const columns: ColumnsType<LoginLogDto> = [
    {
      title: '访问编号',
      dataIndex: 'infoId',
      key: 'infoId',
      width: 100,
    },
    {
      title: '用户名',
      dataIndex: 'userName',
      key: 'userName',
      width: 120,
    },
    {
      title: '登录地址',
      dataIndex: 'ipaddr',
      key: 'ipaddr',
      width: 140,
    },
    {
      title: '登录地点',
      dataIndex: 'loginLocation',
      key: 'loginLocation',
      width: 150,
    },
    {
      title: '浏览器',
      dataIndex: 'browser',
      key: 'browser',
      width: 120,
      ellipsis: true,
    },
    {
      title: '操作系统',
      dataIndex: 'os',
      key: 'os',
      width: 120,
      ellipsis: true,
    },
    {
      title: '登录状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: string) => (
        <Tag color={status === '0' ? 'success' : 'error'}>
          {status === '0' ? '成功' : '失败'}
        </Tag>
      ),
    },
    {
      title: '操作信息',
      dataIndex: 'msg',
      key: 'msg',
      width: 150,
      ellipsis: true,
    },
    {
      title: '登录时间',
      dataIndex: 'loginTime',
      key: 'loginTime',
      width: 180,
    },
    {
      title: '操作',
      key: 'action',
      width: 100,
      fixed: 'right',
      render: (_: unknown, record: LoginLogDto) => (
        <Popconfirm
          title="确定删除此日志吗？"
          onConfirm={() => handleDelete([record.infoId])}
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

  const onFinish = (values: { userName?: string; status?: string; timeRange?: [unknown, unknown] }) => {
    handleSearch({
      userName: values.userName,
      status: values.status,
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
        title="登录日志"
        subTitle="记录用户登录系统的日志"
        extra={
          <Popconfirm
            title="确定清空所有登录日志吗？"
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
        <Form.Item name="userName">
          <Input placeholder="用户名" style={{ width: 150 }} />
        </Form.Item>

        <Form.Item name="status">
          <Select placeholder="登录状态" style={{ width: 120 }} allowClear>
            <Select.Option value="0">成功</Select.Option>
            <Select.Option value="1">失败</Select.Option>
          </Select>
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
        rowKey="infoId"
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
        scroll={{ x: 1400 }}
      />
    </div>
  );
}

