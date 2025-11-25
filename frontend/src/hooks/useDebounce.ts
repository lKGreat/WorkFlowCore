import { useState, useEffect } from 'react';

/**
 * 防抖 Hook
 * @param value 需要防抖的值
 * @param delay 延迟时间（毫秒）
 */
export function useDebounce<T>(value: T, delay: number = 500): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(timer);
    };
  }, [value, delay]);

  return debouncedValue;
}

/**
 * 防抖函数 Hook
 * @param fn 需要防抖的函数
 * @param delay 延迟时间（毫秒）
 */
export function useDebounceFn<T extends (...args: unknown[]) => unknown>(
  fn: T,
  delay: number = 500
): T {
  const [timer, setTimer] = useState<NodeJS.Timeout | null>(null);

  return ((...args: unknown[]) => {
    if (timer) {
      clearTimeout(timer);
    }

    const newTimer = setTimeout(() => {
      fn(...args);
    }, delay);

    setTimer(newTimer);
  }) as T;
}

