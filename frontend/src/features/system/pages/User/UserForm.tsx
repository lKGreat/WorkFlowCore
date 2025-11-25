import React, { useEffect, useState } from 'react';
import { Modal, Form, Input, Select, message } from 'antd';
import type { UserListDto } from '../../types';
import { userService } from '../../services/userService';

interface UserFormProps {
  visible: boolean;
  editingUser: UserListDto | null;
  onCancel: () => void;
  onSuccess: () => void;
}

const UserForm: React.FC<UserFormProps> = ({ visible, editingUser, onCancel, onSuccess }) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [roles, setRoles] = useState<{ id: string; name: string }[]>([]);
  const [departments, setDepartments] = useState<{ id: number; name: string }[]>([]);

  useEffect(() => {
    if (visible) {
      // TODO: 加载角色和部门选项
      loadOptions();
      
      if (editingUser) {
        // 编辑模式 - 加载用户详情
        loadUserDetail(editingUser.userId);
      } else {
        // 新增模式 - 重置表单
        form.resetFields();
      }
    }
  }, [visible, editingUser]);

  const loadOptions = async () => {
    // TODO: 实际调用API获取角色和部门
    setRoles([
      { id: 'role1', name: '管理员' },
      { id: 'role2', name: '普通用户' }
    ]);
    setDepartments([
      { id: 1, name: '研发部' },
      { id: 2, name: '市场部' }
    ]);
  };

  const loadUserDetail = async (userId: string) => {
    try {
      const user = await userService.getUser(userId);
      // TODO: 获取用户的角色ID列表
      form.setFieldsValue({
        userName: user.username,
        nickName: user.fullName,
        email: user.email,
        phoneNumber: user.phoneNumber,
        departmentId: user.departmentId,
        roleIds: [],
        status: user.isActive ? '0' : '1'
      });
    } catch (error) {
      message.error('加载用户信息失败');
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);

      if (editingUser) {
        // 更新
        const updateData = {
          email: values.email,
          fullName: values.nickName,
          phoneNumber: values.phoneNumber,
          departmentId: values.departmentId,
          roleIds: values.roleIds || [],
          isActive: values.status === '0'
        };
        await userService.updateUser(editingUser.userId, updateData);
        message.success('更新成功');
      } else {
        // 创建
        const createData = {
          username: values.userName,
          email: values.email,
          password: values.password || '123456',
          fullName: values.nickName,
          phoneNumber: values.phoneNumber,
          departmentId: values.departmentId,
          roleIds: values.roleIds || []
        };
        await userService.createUser(createData);
        message.success('创建成功');
      }

      onSuccess();
    } catch (error: any) {
      if (error.errorFields) {
        message.error('请填写必填项');
      } else {
        message.error(editingUser ? '更新失败' : '创建失败');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title={editingUser ? '编辑用户' : '新增用户'}
      open={visible}
      onCancel={onCancel}
      onOk={handleSubmit}
      confirmLoading={loading}
      width={600}
      destroyOnClose
    >
      <Form
        form={form}
        layout="vertical"
        initialValues={{ status: '0', sex: '0' }}
      >
        <Form.Item
          name="userName"
          label="用户名"
          rules={[
            { required: true, message: '请输入用户名' },
            { pattern: /^[a-zA-Z0-9_]{4,20}$/, message: '用户名4-20位字母数字下划线' }
          ]}
        >
          <Input placeholder="请输入用户名" disabled={!!editingUser} />
        </Form.Item>

        {!editingUser && (
          <Form.Item
            name="password"
            label="密码"
            rules={[
              { required: true, message: '请输入密码' },
              { min: 6, message: '密码至少6位' }
            ]}
          >
            <Input.Password placeholder="请输入密码" />
          </Form.Item>
        )}

        <Form.Item name="nickName" label="昵称">
          <Input placeholder="请输入昵称" />
        </Form.Item>

        <Form.Item
          name="email"
          label="邮箱"
          rules={[{ type: 'email', message: '邮箱格式不正确' }]}
        >
          <Input placeholder="请输入邮箱" />
        </Form.Item>

        <Form.Item
          name="phoneNumber"
          label="手机号"
          rules={[{ pattern: /^1[3-9]\d{9}$/, message: '手机号格式不正确' }]}
        >
          <Input placeholder="请输入手机号" />
        </Form.Item>

        <Form.Item name="departmentId" label="所属部门">
          <Select placeholder="请选择部门" allowClear>
            {departments.map(dept => (
              <Select.Option key={dept.id} value={dept.id}>
                {dept.name}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item name="roleIds" label="角色" rules={[{ required: true, message: '请选择角色' }]}>
          <Select mode="multiple" placeholder="请选择角色">
            {roles.map(role => (
              <Select.Option key={role.id} value={role.id}>
                {role.name}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item name="sex" label="性别">
          <Select>
            <Select.Option value="0">男</Select.Option>
            <Select.Option value="1">女</Select.Option>
            <Select.Option value="2">未知</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item name="status" label="状态">
          <Select>
            <Select.Option value="0">正常</Select.Option>
            <Select.Option value="1">停用</Select.Option>
          </Select>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default UserForm;

