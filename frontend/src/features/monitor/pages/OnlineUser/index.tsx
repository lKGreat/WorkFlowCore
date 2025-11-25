import { Button, Table, Popconfirm } from 'antd';
import { ReloadOutlined, LogoutOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useOnlineUserList } from '../../hooks/useOnlineUserList';
import type { OnlineUserDto } from '../../types';

export default function OnlineUserManagement() {
  const {
    data,
    loading,
    handleForceLogout,
    reload,
  } = useOnlineUserList();

  const columns: ColumnsType<OnlineUserDto> = [
    {
      title: '用户编号',
      dataIndex: 'userId',
      key: 'userId',
      width: 200,
    },
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
      title: '部门名称',
      dataIndex: 'deptName',
      key: 'deptName',
      width: 150,
    },
    {
      title: '登录IP',
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
      title: '登录时间',
      dataIndex: 'loginTime',
      key: 'loginTime',
      width: 180,
    },
    {
      title: '操作',
      key: 'action',
      width: 120,
      fixed: 'right',
      render: (_: unknown, record: OnlineUserDto) => (
        <Popconfirm
          title="确定强制此用户下线吗？"
          onConfirm={() => handleForceLogout(record.userId)}
          okText="确定"
          cancelText="取消"
        >
          <Button type="link" size="small" danger icon={<LogoutOutlined />}>
            强制下线
          </Button>
        </Popconfirm>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="在线用户"
        subTitle="查看当前在线用户信息"
        extra={
          <Button icon={<ReloadOutlined />} onClick={reload}>
            刷新
          </Button>
        }
      />

      <Table
        columns={columns}
        dataSource={data}
        loading={loading}
        rowKey="userId"
        pagination={false}
        bordered
        scroll={{ x: 1400 }}
      />
    </div>
  );
}

