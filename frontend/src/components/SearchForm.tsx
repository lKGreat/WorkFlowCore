import { Form, Input, Button, Space } from 'antd';
import type { FormInstance } from 'antd';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons';
import type { ReactNode } from 'react';

type SearchFormProps = {
  onSearch: (values: Record<string, unknown>) => void;
  onReset?: () => void;
  children?: ReactNode;
  form?: FormInstance;
  loading?: boolean;
};

/**
 * 通用搜索表单组件
 */
export function SearchForm({ onSearch, onReset, children, form: externalForm, loading = false }: SearchFormProps) {
  const [internalForm] = Form.useForm();
  const form = externalForm || internalForm;

  const handleFinish = (values: Record<string, unknown>) => {
    onSearch(values);
  };

  const handleReset = () => {
    form.resetFields();
    if (onReset) {
      onReset();
    } else {
      onSearch({});
    }
  };

  return (
    <Form
      form={form}
      layout="inline"
      onFinish={handleFinish}
      style={{ marginBottom: 16 }}
    >
      {children}
      <Form.Item>
        <Space>
          <Button
            type="primary"
            htmlType="submit"
            icon={<SearchOutlined />}
            loading={loading}
          >
            搜索
          </Button>
          <Button
            icon={<ReloadOutlined />}
            onClick={handleReset}
          >
            重置
          </Button>
        </Space>
      </Form.Item>
    </Form>
  );
}

/**
 * 搜索表单项 - 文本输入
 */
export function SearchFormItem({ name, label, placeholder }: { name: string; label: string; placeholder?: string }) {
  return (
    <Form.Item name={name} label={label}>
      <Input placeholder={placeholder || `请输入${label}`} allowClear />
    </Form.Item>
  );
}

