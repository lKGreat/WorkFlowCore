import React, { useState, useEffect } from 'react';
import { Form, Input, Button, Tabs, Space, message, Card, Divider, type TabsProps } from 'antd';
import {
  UserOutlined,
  LockOutlined,
  MobileOutlined,
  WechatOutlined,
  QqOutlined,
  AlipayOutlined,
  AppleOutlined
} from '@ant-design/icons';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuthStore } from '../../../stores/authStore';
import { request } from '../../../api';
import QrCodeLogin from './QrCodeLogin';
import './Login.css';

type CaptchaInfo = {
  uuid: string;
  imageBase64: string;
  expireTime: string;
};

type LoginResult = {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: {
    id: number;
    userName: string;
    realName: string;
    email: string;
    phone: string;
    isEnabled: boolean;
  };
};

const LoginPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'username' | 'phone' | 'qrcode'>('username');
  const [usernameForm] = Form.useForm();
  const [phoneForm] = Form.useForm();
  const [captcha, setCaptcha] = useState<CaptchaInfo>();
  const [loading, setLoading] = useState(false);
  const [smsLoading, setSmsLoading] = useState(false);
  const [countdown, setCountdown] = useState(0);
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const authStore = useAuthStore();

  const fetchCaptcha = async () => {
    try {
      const captchaInfo = await request<CaptchaInfo>({
        method: 'GET',
        url: '/api/auth/captcha',
      });
      setCaptcha(captchaInfo);
    } catch (error) {
      console.error('获取验证码失败:', error);
    }
  };

  const handleUsernameLogin = async (values: { username: string; password: string; captchaCode: string }) => {
    setLoading(true);
    try {
      const result = await request<LoginResult>({
        method: 'POST',
        url: '/api/auth/login',
        data: {
          userName: values.username,
          password: values.password,
          captchaUuid: captcha?.uuid || '',
          captchaCode: values.captchaCode || '',
          rememberMe: false
        },
      });

      if (result.token) {
        authStore.setToken(result.token);
        message.success('登录成功');
        const redirect = searchParams.get('redirect') || '/';
        navigate(redirect);
      } else {
        message.error('登录失败');
        fetchCaptcha();
      }
    } catch {
      fetchCaptcha();
    } finally {
      setLoading(false);
    }
  };

  const handleSendSmsCode = async () => {
    const phone = phoneForm.getFieldValue('phone');
    if (!phone) {
      message.warning('请输入手机号');
      return;
    }

    setSmsLoading(true);
    try {
      await request<void>({
        method: 'POST',
        url: '/api/auth/sms/send',
        data: {
          phoneNumber: phone,
          type: 0 // Login
        },
      });

      message.success('验证码已发送');
      setCountdown(60);
    } catch {
      // 错误已由 request 统一处理
    } finally {
      setSmsLoading(false);
    }
  };

  const handlePhoneLogin = async (values: { phone: string; smsCode: string }) => {
    setLoading(true);
    try {
      const result = await request<LoginResult>({
        method: 'POST',
        url: '/api/auth/phone-login',
        data: {
          phoneNumber: values.phone,
          code: values.smsCode
        },
      });

      if (result.token) {
        authStore.setToken(result.token);
        message.success('登录成功');
        const redirect = searchParams.get('redirect') || '/';
        navigate(redirect);
      } else {
        message.error('登录失败');
      }
    } catch {
      // 错误已由 request 统一处理
    } finally {
      setLoading(false);
    }
  };

  const handleThirdPartyLogin = async (provider: string) => {
    try {
      const authUrl = await request<string>({
        method: 'GET',
        url: `/api/auth/oauth/${provider}/authorize`,
        params: {
          redirectUrl: `${window.location.origin}/auth/callback`
        },
      });
      if (authUrl) {
        window.location.href = authUrl;
      } else {
        message.error('获取授权链接失败');
      }
    } catch {
      // 错误已由 request 统一处理
    }
  };

  const tabItems: TabsProps['items'] = [
    {
      key: 'username',
      label: '账号登录',
      children: (
        <Form
          form={usernameForm}
          name="username_login"
          onFinish={handleUsernameLogin}
          autoComplete="off"
        >
          <Form.Item
            name="username"
            rules={[{ required: true, message: '请输入用户名' }]}
          >
            <Input
              prefix={<UserOutlined />}
              placeholder="用户名"
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
            <Space style={{ width: '100%' }}>
              <Form.Item
                name="captchaCode"
                noStyle
                rules={[{ required: true, message: '请输入验证码' }]}
              >
                <Input placeholder="验证码" size="large" style={{ width: '200px' }} />
              </Form.Item>
              {captcha && (
                <img
                  src={captcha.imageBase64}
                  alt="验证码"
                  onClick={fetchCaptcha}
                  style={{ height: '40px', cursor: 'pointer', border: '1px solid #d9d9d9', borderRadius: '2px' }}
                />
              )}
            </Space>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              登录
            </Button>
          </Form.Item>
        </Form>
      ),
    },
    {
      key: 'phone',
      label: '手机号登录',
      children: (
        <Form
          form={phoneForm}
          name="phone_login"
          onFinish={handlePhoneLogin}
          autoComplete="off"
        >
          <Form.Item
            name="phone"
            rules={[
              { required: true, message: '请输入手机号' },
              { pattern: /^1[3-9]\d{9}$/, message: '手机号格式不正确' },
            ]}
          >
            <Input
              prefix={<MobileOutlined />}
              placeholder="手机号"
              size="large"
            />
          </Form.Item>

          <Form.Item>
            <Space style={{ width: '100%' }}>
              <Form.Item
                name="smsCode"
                noStyle
                rules={[{ required: true, message: '请输入验证码' }]}
              >
                <Input placeholder="短信验证码" size="large" style={{ width: '200px' }} />
              </Form.Item>
              <Button
                size="large"
                onClick={handleSendSmsCode}
                loading={smsLoading}
                disabled={countdown > 0}
              >
                {countdown > 0 ? `${countdown}秒后重试` : '发送验证码'}
              </Button>
            </Space>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              登录
            </Button>
          </Form.Item>
        </Form>
      ),
    },
    {
      key: 'qrcode',
      label: '扫码登录',
      children: <QrCodeLogin />,
    },
  ];

  // 获取图形验证码
  useEffect(() => {
    if (activeTab === 'username') {
      fetchCaptcha();
    }
  }, [activeTab]);

  // 倒计时
  useEffect(() => {
    if (countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [countdown]);

  return (
    <div className="login-container">
      <Card className="login-card" title="工作流管理系统">
        <Tabs
          activeKey={activeTab}
          onChange={(key) => setActiveTab(key as 'username' | 'phone' | 'qrcode')}
          centered
          items={tabItems}
        />

        <Divider>第三方登录</Divider>
        <Space size="large" style={{ width: '100%', justifyContent: 'center' }}>
          <Button
            type="text"
            icon={<WechatOutlined style={{ fontSize: '32px', color: '#07C160' }} />}
            onClick={() => handleThirdPartyLogin('wechat')}
          />
          <Button
            type="text"
            icon={<QqOutlined style={{ fontSize: '32px', color: '#12B7F5' }} />}
            onClick={() => handleThirdPartyLogin('qq')}
          />
          <Button
            type="text"
            icon={<AlipayOutlined style={{ fontSize: '32px', color: '#1677FF' }} />}
            onClick={() => handleThirdPartyLogin('alipay')}
          />
          <Button
            type="text"
            icon={<AppleOutlined style={{ fontSize: '32px', color: '#000' }} />}
            onClick={() => handleThirdPartyLogin('apple')}
          />
        </Space>
      </Card>
    </div>
  );
};

export default LoginPage;
