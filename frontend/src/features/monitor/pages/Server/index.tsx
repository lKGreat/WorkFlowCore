import { useState, useEffect } from 'react';
import { Button, Card, Descriptions, Row, Col, Progress, Spin } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';
import { PageHeader } from '@/components/PageHeader';
import { serverMonitorService } from '../../services/serverMonitorService';
import type { ServerInfoDto } from '../../types';

export default function ServerMonitor() {
  const [data, setData] = useState<ServerInfoDto | null>(null);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    try {
      setLoading(true);
      const info = await serverMonitorService.getServerInfo();
      setData(info);
    } catch (error) {
      console.error('加载服务器信息失败', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const formatBytes = (bytes: number) => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  };

  if (loading || !data) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="服务器监控"
        subTitle="查看服务器运行状态"
        extra={
          <Button icon={<ReloadOutlined />} onClick={loadData}>
            刷新
          </Button>
        }
      />

      <Row gutter={[16, 16]}>
        {/* CPU 信息 */}
        <Col span={12}>
          <Card title="CPU 信息" bordered>
            <Descriptions column={1}>
              <Descriptions.Item label="CPU 名称">{data.cpu.name}</Descriptions.Item>
              <Descriptions.Item label="核心数">{data.cpu.coreCount}</Descriptions.Item>
              <Descriptions.Item label="已用">
                <Progress percent={data.cpu.used} status="active" />
              </Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>

        {/* 内存信息 */}
        <Col span={12}>
          <Card title="内存信息" bordered>
            <Descriptions column={1}>
              <Descriptions.Item label="总内存">{formatBytes(data.memory.total)}</Descriptions.Item>
              <Descriptions.Item label="已用内存">{formatBytes(data.memory.used)}</Descriptions.Item>
              <Descriptions.Item label="空闲内存">{formatBytes(data.memory.free)}</Descriptions.Item>
              <Descriptions.Item label="使用率">
                <Progress 
                  percent={Math.round(data.memory.usageRate * 100) / 100}
                  status={data.memory.usageRate > 80 ? 'exception' : 'active'}
                />
              </Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>

        {/* 系统信息 */}
        <Col span={24}>
          <Card title="系统信息" bordered>
            <Descriptions column={2}>
              <Descriptions.Item label="计算机名">{data.system.computerName}</Descriptions.Item>
              <Descriptions.Item label="计算机IP">{data.system.computerIp}</Descriptions.Item>
              <Descriptions.Item label="操作系统">{data.system.osName}</Descriptions.Item>
              <Descriptions.Item label="系统架构">{data.system.osArch}</Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>

        {/* 磁盘信息 */}
        <Col span={24}>
          <Card title="磁盘信息" bordered>
            {data.disks.map((disk, index) => (
              <Card key={index} type="inner" title={disk.dirName} style={{ marginBottom: 16 }}>
                <Descriptions column={2}>
                  <Descriptions.Item label="文件系统">{disk.sysTypeName}</Descriptions.Item>
                  <Descriptions.Item label="类型">{disk.typeName}</Descriptions.Item>
                  <Descriptions.Item label="总大小">{formatBytes(disk.total)}</Descriptions.Item>
                  <Descriptions.Item label="已用">{formatBytes(disk.used)}</Descriptions.Item>
                  <Descriptions.Item label="可用">{formatBytes(disk.free)}</Descriptions.Item>
                  <Descriptions.Item label="使用率">
                    <Progress 
                      percent={Math.round(disk.usageRate * 100) / 100}
                      status={disk.usageRate > 80 ? 'exception' : 'active'}
                    />
                  </Descriptions.Item>
                </Descriptions>
              </Card>
            ))}
          </Card>
        </Col>
      </Row>
    </div>
  );
}

