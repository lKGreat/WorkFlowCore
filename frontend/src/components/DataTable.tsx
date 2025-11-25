import { Table } from 'antd';
import type { TableProps } from 'antd';
import type { ColumnsType } from 'antd/es/table';

type DataTableProps<T> = TableProps<T> & {
  columns: ColumnsType<T>;
  dataSource: T[];
  loading?: boolean;
  rowKey?: string | ((record: T) => string);
  pagination?: TableProps<T>['pagination'];
};

/**
 * 通用数据表格组件
 * 基于 Ant Design Table 的封装
 */
export function DataTable<T extends Record<string, unknown>>({
  columns,
  dataSource,
  loading = false,
  rowKey = 'id',
  pagination,
  ...restProps
}: DataTableProps<T>) {
  const defaultPagination = {
    showSizeChanger: true,
    showQuickJumper: true,
    showTotal: (total: number) => `共 ${total} 条`,
    pageSizeOptions: ['10', '20', '50', '100'],
    ...pagination
  };

  return (
    <Table<T>
      columns={columns}
      dataSource={dataSource}
      loading={loading}
      rowKey={rowKey}
      pagination={defaultPagination}
      bordered
      {...restProps}
    />
  );
}

