import { useState, useCallback } from 'react';
import type { TablePaginationConfig } from 'antd';

type UseTableOptions<T> = {
  defaultPageSize?: number;
  defaultCurrent?: number;
  onLoad?: (params: TableParams) => Promise<{ items: T[]; totalCount: number }>;
};

type TableParams = {
  pageIndex: number;
  pageSize: number;
  [key: string]: unknown;
};

type UseTableResult<T> = {
  data: T[];
  loading: boolean;
  pagination: TablePaginationConfig;
  selectedRowKeys: React.Key[];
  searchParams: Record<string, unknown>;
  setSearchParams: (params: Record<string, unknown>) => void;
  setSelectedRowKeys: (keys: React.Key[]) => void;
  handleTableChange: (pagination: TablePaginationConfig) => void;
  reload: () => Promise<void>;
  clearSelection: () => void;
};

/**
 * 表格通用逻辑 Hook
 */
export function useTable<T extends Record<string, unknown>>(
  options: UseTableOptions<T> = {}
): UseTableResult<T> {
  const { defaultPageSize = 10, defaultCurrent = 1, onLoad } = options;

  const [data, setData] = useState<T[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [searchParams, setSearchParams] = useState<Record<string, unknown>>({});
  const [pagination, setPagination] = useState<TablePaginationConfig>({
    current: defaultCurrent,
    pageSize: defaultPageSize,
    total: 0,
    showSizeChanger: true,
    showQuickJumper: true,
    showTotal: (total) => `共 ${total} 条`,
  });

  const loadData = useCallback(async () => {
    if (!onLoad) return;

    setLoading(true);
    try {
      const result = await onLoad({
        pageIndex: pagination.current || 1,
        pageSize: pagination.pageSize || 10,
        ...searchParams,
      });

      setData(result.items);
      setPagination((prev) => ({
        ...prev,
        total: result.totalCount,
      }));
    } catch (error) {
      console.error('Load data failed:', error);
    } finally {
      setLoading(false);
    }
  }, [pagination.current, pagination.pageSize, searchParams, onLoad]);

  const handleTableChange = useCallback((newPagination: TablePaginationConfig) => {
    setPagination((prev) => ({
      ...prev,
      current: newPagination.current,
      pageSize: newPagination.pageSize,
    }));
  }, []);

  const reload = useCallback(async () => {
    await loadData();
  }, [loadData]);

  const clearSelection = useCallback(() => {
    setSelectedRowKeys([]);
  }, []);

  return {
    data,
    loading,
    pagination,
    selectedRowKeys,
    searchParams,
    setSearchParams,
    setSelectedRowKeys,
    handleTableChange,
    reload,
    clearSelection,
  };
}

