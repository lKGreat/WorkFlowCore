import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, InputNumber, Popconfirm, Row, Col, Card } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useDictType } from '../../hooks/useDictType';
import { useDictData } from '../../hooks/useDictData';
import type { DictTypeDto, DictDataDto } from '../../types';

export default function DictManagement() {
  const {
    data: typeData,
    loading: typeLoading,
    pagination: typePagination,
    handlePageChange: handleTypePageChange,
    handleCreate: handleTypeCreate,
    handleUpdate: handleTypeUpdate,
    handleDelete: handleTypeDelete,
  } = useDictType();

  const [selectedTypeId, setSelectedTypeId] = useState<number>();
  const {
    data: dataData,
    loading: dataLoading,
    pagination: dataPagination,
    handlePageChange: handleDataPageChange,
    handleCreate: handleDataCreate,
    handleUpdate: handleDataUpdate,
    handleDelete: handleDataDelete,
  } = useDictData(selectedTypeId);

  const [isTypeModalOpen, setIsTypeModalOpen] = useState(false);
  const [isDataModalOpen, setIsDataModalOpen] = useState(false);
  const [editingType, setEditingType] = useState<DictTypeDto | null>(null);
  const [editingData, setEditingData] = useState<DictDataDto | null>(null);
  const [typeForm] = Form.useForm();
  const [dataForm] = Form.useForm();

  const typeColumns: ColumnsType<DictTypeDto> = [
    {
      title: '字典编号',
      dataIndex: 'dictId',
      key: 'dictId',
      width: 100,
    },
    {
      title: '字典名称',
      dataIndex: 'dictName',
      key: 'dictName',
      width: 150,
    },
    {
      title: '字典类型',
      dataIndex: 'dictType',
      key: 'dictType',
      width: 150,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
      render: (status: string) => (status === '0' ? '正常' : '停用'),
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
      width: 180,
      fixed: 'right',
      render: (_: unknown, record: DictTypeDto) => (
        <Space>
          <Button
            type="link"
            size="small"
            onClick={() => setSelectedTypeId(record.dictId)}
          >
            数据
          </Button>
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => handleTypeEdit(record)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定删除此字典类型吗？"
            onConfirm={() => handleTypeDelete([record.dictId])}
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

  const dataColumns: ColumnsType<DictDataDto> = [
    {
      title: '字典编码',
      dataIndex: 'dictCode',
      key: 'dictCode',
      width: 100,
    },
    {
      title: '字典标签',
      dataIndex: 'dictLabel',
      key: 'dictLabel',
      width: 150,
    },
    {
      title: '字典键值',
      dataIndex: 'dictValue',
      key: 'dictValue',
      width: 150,
    },
    {
      title: '排序',
      dataIndex: 'dictSort',
      key: 'dictSort',
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
      render: (_: unknown, record: DictDataDto) => (
        <Space>
          <Button
            type="link"
            size="small"
            icon={<EditOutlined />}
            onClick={() => handleDataEdit(record)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定删除此字典数据吗？"
            onConfirm={() => handleDataDelete([record.dictCode])}
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

  const handleTypeAdd = () => {
    setEditingType(null);
    typeForm.resetFields();
    setIsTypeModalOpen(true);
  };

  const handleTypeEdit = (record: DictTypeDto) => {
    setEditingType(record);
    typeForm.setFieldsValue(record);
    setIsTypeModalOpen(true);
  };

  const handleTypeModalOk = async () => {
    try {
      const values = await typeForm.validateFields();
      if (editingType) {
        await handleTypeUpdate(editingType.dictId, values);
      } else {
        await handleTypeCreate(values);
      }
      setIsTypeModalOpen(false);
      typeForm.resetFields();
    } catch (error) {
      // 表单验证失败或提交失败
    }
  };

  const handleDataAdd = () => {
    if (!selectedTypeId) {
      return;
    }
    setEditingData(null);
    dataForm.resetFields();
    dataForm.setFieldsValue({ dictTypeId: selectedTypeId });
    setIsDataModalOpen(true);
  };

  const handleDataEdit = (record: DictDataDto) => {
    setEditingData(record);
    dataForm.setFieldsValue(record);
    setIsDataModalOpen(true);
  };

  const handleDataModalOk = async () => {
    try {
      const values = await dataForm.validateFields();
      if (editingData) {
        await handleDataUpdate(editingData.dictCode, values);
      } else {
        await handleDataCreate(values);
      }
      setIsDataModalOpen(false);
      dataForm.resetFields();
    } catch (error) {
      // 表单验证失败或提交失败
    }
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="字典管理"
        subTitle="管理系统数据字典"
      />

      <Row gutter={16}>
        <Col span={12}>
          <Card
            title="字典类型"
            extra={
              <Button type="primary" size="small" icon={<PlusOutlined />} onClick={handleTypeAdd}>
                新建类型
              </Button>
            }
          >
            <Table
              columns={typeColumns}
              dataSource={typeData}
              loading={typeLoading}
              rowKey="dictId"
              pagination={{
                current: typePagination.current,
                pageSize: typePagination.pageSize,
                total: typePagination.total,
                showSizeChanger: true,
                showQuickJumper: true,
                showTotal: (total) => `共 ${total} 条`,
                onChange: handleTypePageChange,
              }}
              bordered
              size="small"
            />
          </Card>
        </Col>

        <Col span={12}>
          <Card
            title="字典数据"
            extra={
              <Button
                type="primary"
                size="small"
                icon={<PlusOutlined />}
                onClick={handleDataAdd}
                disabled={!selectedTypeId}
              >
                新建数据
              </Button>
            }
          >
            <Table
              columns={dataColumns}
              dataSource={dataData}
              loading={dataLoading}
              rowKey="dictCode"
              pagination={{
                current: dataPagination.current,
                pageSize: dataPagination.pageSize,
                total: dataPagination.total,
                showSizeChanger: true,
                showQuickJumper: true,
                showTotal: (total) => `共 ${total} 条`,
                onChange: handleDataPageChange,
              }}
              bordered
              size="small"
            />
          </Card>
        </Col>
      </Row>

      {/* 字典类型弹窗 */}
      <Modal
        title={editingType ? '编辑字典类型' : '新建字典类型'}
        open={isTypeModalOpen}
        onOk={handleTypeModalOk}
        onCancel={() => setIsTypeModalOpen(false)}
        width={600}
      >
        <Form
          form={typeForm}
          layout="vertical"
          initialValues={{ status: '0' }}
        >
          <Form.Item
            label="字典名称"
            name="dictName"
            rules={[{ required: true, message: '请输入字典名称' }]}
          >
            <Input placeholder="请输入字典名称" />
          </Form.Item>

          <Form.Item
            label="字典类型"
            name="dictType"
            rules={[{ required: true, message: '请输入字典类型' }]}
          >
            <Input placeholder="请输入字典类型" />
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

          <Form.Item label="备注" name="remark">
            <Input.TextArea rows={4} placeholder="请输入备注" />
          </Form.Item>
        </Form>
      </Modal>

      {/* 字典数据弹窗 */}
      <Modal
        title={editingData ? '编辑字典数据' : '新建字典数据'}
        open={isDataModalOpen}
        onOk={handleDataModalOk}
        onCancel={() => setIsDataModalOpen(false)}
        width={600}
      >
        <Form
          form={dataForm}
          layout="vertical"
          initialValues={{ status: '0', dictSort: 0, isDefault: false }}
        >
          <Form.Item name="dictTypeId" hidden>
            <Input />
          </Form.Item>

          <Form.Item
            label="字典标签"
            name="dictLabel"
            rules={[{ required: true, message: '请输入字典标签' }]}
          >
            <Input placeholder="请输入字典标签" />
          </Form.Item>

          <Form.Item
            label="字典键值"
            name="dictValue"
            rules={[{ required: true, message: '请输入字典键值' }]}
          >
            <Input placeholder="请输入字典键值" />
          </Form.Item>

          <Form.Item label="显示排序" name="dictSort">
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item label="样式类名" name="cssClass">
            <Input placeholder="请输入样式类名" />
          </Form.Item>

          <Form.Item label="回显样式" name="listClass">
            <Input placeholder="请输入回显样式" />
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

