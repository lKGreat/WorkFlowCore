import { Table, Button, Space, Tag } from 'antd';
import {
  PauseOutlined,
  PlayCircleOutlined,
  CloseCircleOutlined,
  EyeOutlined,
} from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import type { ProcessInstanceListItem } from '../types';
import {
  ProcessInstanceStatus,
  ProcessInstanceStatusText,
  ProcessInstanceStatusColor,
} from '../types';
import { showConfirm } from '@/components/ConfirmModal';

type ProcessInstanceTableProps = {
  data: ProcessInstanceListItem[];
  loading: boolean;
  pagination: TablePaginationConfig;
  onPageChange: (page: number, pageSize: number) => void;
  onSuspend: (id: number) => void;
  onResume: (id: number) => void;
  onTerminate: (id: number, reason?: string) => void;
};

export function ProcessInstanceTable({
  data,
  loading,
  pagination,
  onPageChange,
  onSuspend,
  onResume,
  onTerminate,
}: ProcessInstanceTableProps) {
  const handleTerminate = (id: number) => {
    showConfirm(
      '确认终止流程',
      '终止流程后将无法恢复，确定要继续吗？',
      () => onTerminate(id, '用户手动终止')
    );
  };

  const columns: ColumnsType<ProcessInstanceListItem> = [
    {
      title: '实例ID',
      dataIndex: 'id',
      key: 'id',
      width: 100,
    },
    {
      title: '流程名称',
      dataIndex: 'processDefinitionName',
      key: 'processDefinitionName',
      width: 200,
    },
    {
      title: '当前步骤',
      dataIndex: 'currentStepName',
      key: 'currentStepName',
      width: 150,
      render: (text?: string) => text || '-',
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: ProcessInstanceStatus) => (
        <Tag color={ProcessInstanceStatusColor[status]}>
          {ProcessInstanceStatusText[status]}
        </Tag>
      ),
    },
    {
      title: '发起人',
      dataIndex: 'initiatorName',
      key: 'initiatorName',
      width: 120,
    },
    {
      title: '开始时间',
      dataIndex: 'startTime',
      key: 'startTime',
      width: 180,
      render: (date: string) => new Date(date).toLocaleString('zh-CN'),
    },
    {
      title: '结束时间',
      dataIndex: 'endTime',
      key: 'endTime',
      width: 180,
      render: (date?: string) =>
        date ? new Date(date).toLocaleString('zh-CN') : '-',
    },
    {
      title: '操作',
      key: 'action',
      width: 250,
      fixed: 'right',
      render: (_: unknown, record: ProcessInstanceListItem) => (
        <Space size="small">
          <Button
            type="link"
            size="small"
            icon={<EyeOutlined />}
          >
            详情
          </Button>
          {record.status === ProcessInstanceStatus.Running && (
            <>
              <Button
                type="link"
                size="small"
                icon={<PauseOutlined />}
                onClick={() => onSuspend(record.id)}
              >
                挂起
              </Button>
              <Button
                type="link"
                size="small"
                danger
                icon={<CloseCircleOutlined />}
                onClick={() => handleTerminate(record.id)}
              >
                终止
              </Button>
            </>
          )}
          {record.status === ProcessInstanceStatus.Suspended && (
            <Button
              type="link"
              size="small"
              icon={<PlayCircleOutlined />}
              onClick={() => onResume(record.id)}
            >
              恢复
            </Button>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={data}
      rowKey="id"
      loading={loading}
      pagination={{
        ...pagination,
        showSizeChanger: true,
        showTotal: (total) => `共 ${total} 条`,
        onChange: onPageChange,
      }}
      scroll={{ x: 1400 }}
    />
  );
}

