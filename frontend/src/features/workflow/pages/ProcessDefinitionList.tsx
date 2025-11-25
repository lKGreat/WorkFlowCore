import { Button } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '@/components/PageHeader';
import { ProcessTable } from '../components/ProcessTable';
import { useProcessList } from '../hooks/useProcessList';

export function ProcessDefinitionList() {
  const navigate = useNavigate();
  const { data, loading, pagination, handlePageChange, handleDelete } = useProcessList();

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader
        title="流程定义管理"
        extra={
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => navigate('/designer')}
          >
            新建流程
          </Button>
        }
      />

      <ProcessTable
        data={data}
        loading={loading}
        pagination={pagination}
        onPageChange={handlePageChange}
        onDelete={handleDelete}
      />
    </div>
  );
}
