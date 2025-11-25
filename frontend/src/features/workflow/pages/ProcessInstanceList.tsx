import { PageHeader } from '@/components/PageHeader';
import { ProcessInstanceTable } from '../components/ProcessInstanceTable';
import { useProcessInstances } from '../hooks/useProcessInstances';

export function ProcessInstanceList() {
  const {
    data,
    loading,
    pagination,
    handlePageChange,
    handleSuspend,
    handleResume,
    handleTerminate,
  } = useProcessInstances();

  return (
    <div style={{ padding: '24px' }}>
      <PageHeader title="流程实例列表" subTitle="查看和管理所有流程实例" />

      <ProcessInstanceTable
        data={data}
        loading={loading}
        pagination={pagination}
        onPageChange={handlePageChange}
        onSuspend={handleSuspend}
        onResume={handleResume}
        onTerminate={handleTerminate}
      />
    </div>
  );
}
