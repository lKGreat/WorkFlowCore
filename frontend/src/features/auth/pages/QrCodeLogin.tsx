import React, { useState, useEffect } from 'react';
import { QRCode, Spin, Typography, Space } from 'antd';
import { CheckCircleOutlined, CloseCircleOutlined } from '@ant-design/icons';

const { Text } = Typography;

interface QrCodeInfo {
  uuid: string;
  qrContent: string;
  expireTime: string;
}

type QrCodeStatus = 'WaitScan' | 'Scanned' | 'Confirmed' | 'Expired' | 'Cancelled';

const QrCodeLogin: React.FC = () => {
  const [qrInfo, setQrInfo] = useState<QrCodeInfo>();
  const [status, setStatus] = useState<QrCodeStatus>('WaitScan');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    generateQrCode();
  }, []);

  useEffect(() => {
    if (!qrInfo) return;

    const interval = setInterval(async () => {
      try {
        const response = await fetch(`/auth/qrcode/poll?uuid=${qrInfo.uuid}`);
        const result = await response.json();

        if (result.success) {
          setStatus(result.data.status);

          if (result.data.status === 'Confirmed' && result.data.accessToken) {
            localStorage.setItem('token', result.data.accessToken);
            window.location.href = '/';
            clearInterval(interval);
          }

          if (result.data.status === 'Expired') {
            clearInterval(interval);
          }
        }
      } catch (error) {
        console.error('轮询失败', error);
      }
    }, 2000);

    return () => clearInterval(interval);
  }, [qrInfo]);

  const generateQrCode = async () => {
    setLoading(true);
    try {
      const response = await fetch(`/auth/qrcode/generate?deviceId=${Math.random().toString(36).substring(7)}`);
      const result = await response.json();

      if (result.success) {
        setQrInfo(result.data);
        setStatus('WaitScan');
      }
    } catch (error) {
      console.error('生成二维码失败', error);
    } finally {
      setLoading(false);
    }
  };

  const renderStatus = () => {
    switch (status) {
      case 'WaitScan':
        return (
          <Space direction="vertical" align="center">
            <Text>请使用手机APP扫码</Text>
          </Space>
        );
      case 'Scanned':
        return (
          <Space direction="vertical" align="center">
            <CheckCircleOutlined style={{ fontSize: '48px', color: '#52c41a' }} />
            <Text>已扫描,请在手机上确认</Text>
          </Space>
        );
      case 'Confirmed':
        return (
          <Space direction="vertical" align="center">
            <CheckCircleOutlined style={{ fontSize: '48px', color: '#52c41a' }} />
            <Text>登录成功</Text>
          </Space>
        );
      case 'Expired':
        return (
          <Space direction="vertical" align="center">
            <CloseCircleOutlined style={{ fontSize: '48px', color: '#ff4d4f' }} />
            <Text>二维码已过期</Text>
            <a onClick={generateQrCode}>点击刷新</a>
          </Space>
        );
      default:
        return null;
    }
  };

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '40px' }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ textAlign: 'center', padding: '20px' }}>
      {qrInfo && status === 'WaitScan' && (
        <QRCode value={qrInfo.qrContent} size={200} />
      )}
      <div style={{ marginTop: '20px' }}>
        {renderStatus()}
      </div>
    </div>
  );
};

export default QrCodeLogin;

