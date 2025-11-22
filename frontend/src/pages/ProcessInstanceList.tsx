import React, { useState } from 'react';
import { Table, Button, Space, message, Tag, Modal, Form, Select, Input } from 'antd';
import { PlusOutlined, PauseCircleOutlined, PlayCircleOutlined, StopOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { workflowService } from '../services/workflowService';
import type { WorkflowInstance } from '../types/processDefinition.types';

const { Option } = Select;

export const ProcessInstanceList: React.FC = () => {
  const [data, setData] = useState<WorkflowInstance[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isStarting, setIsStarting] = useState(false);
  const [form] = Form.useForm();

  const handleStart = async () => {
    try {
      await form.validateFields();
      const values = form.getFieldsValue();
      
      setIsStarting(true);
      
      const instanceId = await workflowService.startWorkflow(
        values.workflowId,
        values.version,
        {},
        values.reference
      );
      
      message.success(`工作流已启动，实例ID: ${instanceId}`);
      setIsModalOpen(false);
      form.resetFields();
      
      // 可以刷新列表或跳转到实例详情页
    } catch (error: any) {
      message.error(error.message || '启动工作流失败');
    } finally {
      setIsStarting(false);
    }
  };

  const handleSuspend = async (id: string) => {
    try {
      await workflowService.suspendWorkflow(id);
      message.success('工作流已暂停');
      // 刷新列表
    } catch (error: any) {
      message.error(error.message || '暂停工作流失败');
    }
  };

  const handleResume = async (id: string) => {
    try {
      await workflowService.resumeWorkflow(id);
      message.success('工作流已恢复');
      // 刷新列表
    } catch (error: any) {
      message.error(error.message || '恢复工作流失败');
    }
  };

  const handleTerminate = (record: WorkflowInstance) => {
    Modal.confirm({
      title: '确认终止',
      content: `确定要终止工作流实例 "${record.id}" 吗？`,
      okText: '确定',
      cancelText: '取消',
      onOk: async () => {
        try {
          await workflowService.terminateWorkflow(record.id);
          message.success('工作流已终止');
          // 刷新列表
        } catch (error: any) {
          message.error(error.message || '终止工作流失败');
        }
      },
    });
  };

  const getStatusTag = (status: string) => {
    const statusMap: Record<string, { color: string; text: string }> = {
      'Runnable': { color: 'blue', text: '运行中' },
      'Suspended': { color: 'orange', text: '已暂停' },
      'Complete': { color: 'green', text: '已完成' },
      'Terminated': { color: 'red', text: '已终止' },
    };
    
    const config = statusMap[status] || { color: 'default', text: status };
    return <Tag color={config.color}>{config.text}</Tag>;
  };

  const columns: ColumnsType<WorkflowInstance> = [
    {
      title: '实例ID',
      dataIndex: 'id',
      key: 'id',
      width: 250,
      ellipsis: true,
    },
    {
      title: '流程定义ID',
      dataIndex: 'workflowDefinitionId',
      key: 'workflowDefinitionId',
      width: 200,
    },
    {
      title: '版本',
      dataIndex: 'version',
      key: 'version',
      width: 80,
      render: (version: number) => <Tag color="blue">V{version}</Tag>,
    },
    {
      title: '引用键',
      dataIndex: 'reference',
      key: 'reference',
      width: 150,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: string) => getStatusTag(status),
    },
    {
      title: '创建时间',
      dataIndex: 'createTime',
      key: 'createTime',
      width: 180,
      render: (date: string) => new Date(date).toLocaleString('zh-CN'),
    },
    {
      title: '完成时间',
      dataIndex: 'completeTime',
      key: 'completeTime',
      width: 180,
      render: (date?: string) => date ? new Date(date).toLocaleString('zh-CN') : '-',
    },
    {
      title: '操作',
      key: 'action',
      width: 200,
      fixed: 'right',
      render: (_: any, record: WorkflowInstance) => (
        <Space size="small">
          {record.status === 'Runnable' && (
            <Button
              type="link"
              size="small"
              icon={<PauseCircleOutlined />}
              onClick={() => handleSuspend(record.id)}
            >
              暂停
            </Button>
          )}
          {record.status === 'Suspended' && (
            <Button
              type="link"
              size="small"
              icon={<PlayCircleOutlined />}
              onClick={() => handleResume(record.id)}
            >
              恢复
            </Button>
          )}
          {(record.status === 'Runnable' || record.status === 'Suspended') && (
            <Button
              type="link"
              size="small"
              danger
              icon={<StopOutlined />}
              onClick={() => handleTerminate(record)}
            >
              终止
            </Button>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ marginBottom: '16px', display: 'flex', justifyContent: 'space-between' }}>
        <h2>流程实例管理</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setIsModalOpen(true)}
        >
          启动工作流
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={data}
        rowKey="id"
        loading={loading}
        pagination={{
          showSizeChanger: true,
          showTotal: (total) => `共 ${total} 条`,
        }}
        scroll={{ x: 1400 }}
      />

      <Modal
        title="启动工作流"
        open={isModalOpen}
        onOk={handleStart}
        onCancel={() => setIsModalOpen(false)}
        confirmLoading={isStarting}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            name="workflowId"
            label="流程定义Key"
            rules={[{ required: true, message: '请输入流程定义Key' }]}
          >
            <Input placeholder="请输入流程定义Key" />
          </Form.Item>
          <Form.Item name="version" label="版本号（可选）">
            <Input placeholder="留空则使用最新版本" type="number" />
          </Form.Item>
          <Form.Item name="reference" label="引用键（可选）">
            <Input placeholder="业务关联键" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

