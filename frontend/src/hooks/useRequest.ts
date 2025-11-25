import { useState, useCallback } from 'react';
import { message } from 'antd';

type UseRequestOptions<T> = {
  manual?: boolean;
  onSuccess?: (data: T) => void;
  onError?: (error: Error) => void;
  successMessage?: string;
  errorMessage?: string;
};

type UseRequestResult<T, P extends unknown[]> = {
  data: T | undefined;
  loading: boolean;
  error: Error | null;
  run: (...args: P) => Promise<T | undefined>;
  mutate: (data: T) => void;
  refresh: () => Promise<T | undefined>;
};

/**
 * 通用请求 Hook
 * @param service 请求函数
 * @param options 配置选项
 */
export function useRequest<T, P extends unknown[]>(
  service: (...args: P) => Promise<T>,
  options: UseRequestOptions<T> = {}
): UseRequestResult<T, P> {
  const [data, setData] = useState<T | undefined>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [lastArgs, setLastArgs] = useState<P | null>(null);

  const run = useCallback(async (...args: P) => {
    setLoading(true);
    setError(null);
    setLastArgs(args);
    
    try {
      const result = await service(...args);
      setData(result);
      
      if (options.successMessage) {
        message.success(options.successMessage);
      }
      
      options.onSuccess?.(result);
      return result;
    } catch (err) {
      const error = err as Error;
      setError(error);
      
      if (options.errorMessage) {
        message.error(options.errorMessage);
      }
      
      options.onError?.(error);
      return undefined;
    } finally {
      setLoading(false);
    }
  }, [service, options]);

  const mutate = useCallback((newData: T) => {
    setData(newData);
  }, []);

  const refresh = useCallback(async () => {
    if (lastArgs) {
      return run(...lastArgs);
    }
    return undefined;
  }, [lastArgs, run]);

  return { data, loading, error, run, mutate, refresh };
}

