import { useState } from 'react';
import { Button, Space, Table, Modal, Form, Input, Select, InputNumber, Popconfirm, TreeSelect, Radio } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { PageHeader } from '@/components/PageHeader';
import { useMenuTree } from '../../hooks/useMenuTree';
import { menuService } from '../../services/menuService';
import type { MenuDto } from '../../types';

export default function MenuManagement() {
  const { data, loading, handleCreate, handleUpdate, handleDelete } = useMenuTree();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<MenuDto | null>(null);
  const [form] = Form.useForm();
  const [treeData, setTreeData] = useState<MenuDto[]>([]);
  const [menuType, setMenuType] = useState<string>('C');

  const columns: ColumnsType<MenuDto> = [
    {
      title: '菜单名称',
      dataIndex: 'menuName',
      key: 'menuName',
      width: 200,
    },
    {
      title: '图标',
      dataIndex: 'icon',
      key: 'icon',
      width: 80,
    },
    {
      title: '排序',
      dataIndex: 'orderNum',
      key: 'orderNum',
      width: 80,
    },
    {
      title: '权限标识',
      dataIndex: 'permissionCode',
      key: 'permissionCode',
      width: 150,
    },
    {
      title: '组件路径',
      dataIndex: 'component',
      key: 'component',
      width: 200,
      ellipsis: true,
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
      render: (_: unknown, record: MenuDto) => (
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
            title="确定删除此菜单吗？"
            onConfirm={() => handleDelete(record.menuId)}
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
    setMenuType('C');
    const tree = await menuService.getTreeSelect();
    setTreeData(tree);
    setIsModalOpen(true);
  };

  const handleEdit = async (record: MenuDto) => {
    setEditingRecord(record);
    setMenuType(record.menuType);
    const tree = await menuService.getTreeSelect();
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
        await handleUpdate({ ...values, menuId: editingRecord.menuId });
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
  const convertToTreeSelectData = (nodes: MenuDto[]): unknown[] => {
    return nodes.map(node => ({
      title: node.menuName,
      value: node.menuId,
      children: node.children ? convertToTreeSelectData(node.children) : undefined,
    }));
  };

  const handleMenuTypeChange = (e: { target: { value: string } }) => {
    setMenuType(e.target.value);
  };

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="菜单管理"
        subTitle="管理系统菜单和权限"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
            新建菜单
          </Button>
        }
      />

      <Table
        columns={columns}
        dataSource={data}
        loading={loading}
        rowKey="menuId"
        pagination={false}
        bordered
      />

      <Modal
        title={editingRecord ? '编辑菜单' : '新建菜单'}
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        width={700}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: '0', orderNum: 0, menuType: 'C', visible: true, isFrame: false, isCache: '0' }}
        >
          <Form.Item
            label="上级菜单"
            name="parentId"
          >
            <TreeSelect
              placeholder="选择上级菜单"
              treeData={convertToTreeSelectData(treeData)}
              allowClear
              treeDefaultExpandAll
            />
          </Form.Item>

          <Form.Item
            label="菜单类型"
            name="menuType"
            rules={[{ required: true, message: '请选择菜单类型' }]}
          >
            <Radio.Group onChange={handleMenuTypeChange}>
              <Radio value="M">目录</Radio>
              <Radio value="C">菜单</Radio>
              <Radio value="F">按钮</Radio>
            </Radio.Group>
          </Form.Item>

          <Form.Item
            label="菜单名称"
            name="menuName"
            rules={[{ required: true, message: '请输入菜单名称' }]}
          >
            <Input placeholder="请输入菜单名称" />
          </Form.Item>

          {(menuType === 'M' || menuType === 'C') && (
            <>
              <Form.Item label="菜单图标" name="icon">
                <Input placeholder="请输入图标名称" />
              </Form.Item>

              <Form.Item label="路由地址" name="path">
                <Input placeholder="请输入路由地址" />
              </Form.Item>
            </>
          )}

          {menuType === 'C' && (
            <Form.Item label="组件路径" name="component">
              <Input placeholder="请输入组件路径" />
            </Form.Item>
          )}

          {(menuType === 'C' || menuType === 'F') && (
            <Form.Item label="权限标识" name="permissionCode">
              <Input placeholder="请输入权限标识" />
            </Form.Item>
          )}

          <Form.Item label="显示排序" name="orderNum">
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>

          {(menuType === 'M' || menuType === 'C') && (
            <>
              <Form.Item label="是否可见" name="visible" valuePropName="checked">
                <Radio.Group>
                  <Radio value={true}>显示</Radio>
                  <Radio value={false}>隐藏</Radio>
                </Radio.Group>
              </Form.Item>

              <Form.Item label="是否外链" name="isFrame" valuePropName="checked">
                <Radio.Group>
                  <Radio value={true}>是</Radio>
                  <Radio value={false}>否</Radio>
                </Radio.Group>
              </Form.Item>
            </>
          )}

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

