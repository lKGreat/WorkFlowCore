import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, InputNumber, Popconfirm, TreeSelect } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import type { DataNode } from 'antd/es/tree';
import { PageHeader } from '@/components/PageHeader';
import { useDepartmentTree } from '../../hooks/useDepartmentTree';
import { departmentService } from '../../services/departmentService';
import type { DepartmentDto } from '../../types';

export default function DepartmentManagement() {
  const { data, loading, handleCreate, handleUpdate, handleDelete } = useDepartmentTree();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<DepartmentDto | null>(null);
  const [form] = Form.useForm();
  const [treeData, setTreeData] = useState<DepartmentDto[]>([]);

  const columns: ColumnsType<DepartmentDto> = [
    {
      title: '部门名称',
      dataIndex: 'name',
      key: 'name',
      width: 200,
    },
    {
      title: '部门编码',
      dataIndex: 'code',
      key: 'code',
      width: 120,
    },
    {
      title: '负责人',
      dataIndex: 'managerName',
      key: 'managerName',
      width: 100,
    },
    {
      title: '排序',
      dataIndex: 'sortOrder',
      key: 'sortOrder',
      width: 80,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
      render: (status: string) => (status === '0' ? '正常' : '停用'),
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
      render: (_: unknown, record: DepartmentDto) => (
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
            title="确定删除此部门吗？"
            onConfirm={() => handleDelete(record.id)}
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

  const handleAdd = async () => {
    setEditingRecord(null);
    form.resetFields();
    const tree = await departmentService.getTreeSelect();
    setTreeData(tree);
    setIsModalOpen(true);
  };

  const handleEdit = async (record: DepartmentDto) => {
    setEditingRecord(record);
    const tree = await departmentService.getTreeExclude(record.id);
    setTreeData(tree);
    form.setFieldsValue({
      ...record,
      parentId: record.parentId || undefined,
    });
    setIsModalOpen(true);
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await handleUpdate(editingRecord.id, values);
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

  // 转换树数据为 TreeSelect 格式
  const convertToTreeSelectData = (nodes: DepartmentDto[]): DataNode[] => {
    return nodes.map(node => ({
      title: node.name,
      value: node.id,
      key: node.id,
      children: node.children ? convertToTreeSelectData(node.children) : undefined,
    }));
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="部门管理"
        subTitle="管理组织架构部门信息"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新建部门
          </Button>
        }
      />

      <Table
        columns={columns}
        dataSource={data}
        loading={loading}
        rowKey="id"
        pagination={false}
        bordered
      />

      <Modal
        title={editingRecord ? '编辑部门' : '新建部门'}
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: '0', sortOrder: 0 }}
        >
          <Form.Item
            label="上级部门"
            name="parentId"
          >
            <TreeSelect
              placeholder="选择上级部门"
              treeData={convertToTreeSelectData(treeData)}
              allowClear
              treeDefaultExpandAll
            />
          </Form.Item>

          <Form.Item
            label="部门名称"
            name="name"
            rules={[{ required: true, message: '请输入部门名称' }]}
          >
            <Input placeholder="请输入部门名称" />
          </Form.Item>

          <Form.Item
            label="部门编码"
            name="code"
          >
            <Input placeholder="请输入部门编码" />
          </Form.Item>

          <Form.Item
            label="显示排序"
            name="sortOrder"
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
        </Form>
      </Modal>
    </div>
  );
}

