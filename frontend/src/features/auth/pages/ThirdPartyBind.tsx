import React, { useState, useEffect } from 'react';
import { Form, Input, Button, Card, message, Space, Avatar } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useNavigate, useSearchParams } from 'react-router-dom';

const ThirdPartyBind: React.FC = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [thirdPartyInfo, setThirdPartyInfo] = useState<any>();

  const provider = searchParams.get('provider');
  const tempToken = searchParams.get('token');

  useEffect(() => {
    // TODO: 从tempToken解析第三方用户信息
    setThirdPartyInfo({
      avatar: 'https://example.com/avatar.jpg',
      nickname: '第三方用户'
    });
  }, [tempToken]);

  const handleBind = async (values: any) => {
    setLoading(true);
    try {
      // 先登录获取用户信息
      const loginResponse = await fetch('/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userName: values.username,
          password: values.password
        })
      });

      const loginResult = await loginResponse.json();
      if (!loginResult.success) {
        message.error('登录失败');
        return;
      }

      const token = loginResult.data.token;

      // 绑定第三方账号
      const bindResponse = await fetch(`/auth/oauth/${provider}/bind`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ tempToken })
      });

      const bindResult = await bindResponse.json();
      if (bindResult.success) {
        localStorage.setItem('token', token);
        message.success('绑定成功');
        navigate('/');
      } else {
        message.error(bindResult.message || '绑定失败');
      }
    } catch (error) {
      message.error('绑定失败,请稍后重试');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      minHeight: '100vh',
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
    }}>
      <Card title="绑定系统账号" style={{ width: 400 }}>
        {thirdPartyInfo && (
          <Space direction="vertical" align="center" style={{ width: '100%', marginBottom: '24px' }}>
            <Avatar size={64} src={thirdPartyInfo.avatar} />
            <div>{thirdPartyInfo.nickname}</div>
          </Space>
        )}

        <Form form={form} onFinish={handleBind} autoComplete="off">
          <Form.Item
            name="username"
            rules={[{ required: true, message: '请输入用户名' }]}
          >
            <Input
              prefix={<UserOutlined />}
              placeholder="系统用户名"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: '请输入密码' }]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="密码"
              size="large"
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              绑定并登录
            </Button>
          </Form.Item>

          <Form.Item>
            <Button type="link" onClick={() => navigate('/login')} block>
              返回登录
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default ThirdPartyBind;

