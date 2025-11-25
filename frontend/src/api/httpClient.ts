import axios, { AxiosError, type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios'
import { message } from 'antd'
import { ApiError } from './apiError'
import type { ApiResponse } from './types'
import { getToken } from '../utils/auth'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

const httpClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求计数器（用于全局 Loading）
let pendingRequests = 0

// 动态导入 uiStore（避免循环依赖）
let uiStoreModule: typeof import('../stores/uiStore') | null = null
async function getUiStore() {
  if (!uiStoreModule) {
    uiStoreModule = await import('../stores/uiStore')
  }
  return uiStoreModule.useUiStore.getState()
}

// 请求拦截器 - 添加Token + Loading计数
httpClient.interceptors.request.use(
  async (config) => {
    // 自动添加 Token
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // 增加请求计数（排除静默请求）
    if (!config.headers['X-Silent']) {
      pendingRequests++;
      if (pendingRequests === 1) {
        try {
          const uiStore = await getUiStore();
          uiStore.setGlobalLoading(true);
        } catch {
          // 如果 uiStore 加载失败，忽略
        }
      }
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
)

const showErrorMessage = (error: ApiError) => {
  const traceInfo = error.traceId ? `（TraceId: ${error.traceId}）` : ''
  message.error({
    content: `${error.message}${traceInfo}`,
    duration: 4
  })
}

const buildApiErrorFromResponse = (
  response: AxiosResponse<ApiResponse<unknown>> | undefined,
  fallbackMessage: string,
  cause?: unknown
): ApiError => {
  if (response?.data) {
    const payload = response.data
    return new ApiError(payload.message ?? fallbackMessage, {
      status: response.status,
      errorCode: payload.errorCode,
      traceId: payload.traceId,
      errors: payload.errors,
      cause
    })
  }

  return new ApiError(fallbackMessage, {
    status: response?.status,
    cause
  })
}

// 减少请求计数
async function decrementPending(config?: AxiosRequestConfig) {
  if (!config?.headers?.['X-Silent']) {
    pendingRequests = Math.max(0, pendingRequests - 1);
    if (pendingRequests === 0) {
      try {
        const uiStore = await getUiStore();
        uiStore.setGlobalLoading(false);
      } catch {
        // 如果 uiStore 加载失败，忽略
      }
    }
  }
}

// 响应拦截器
httpClient.interceptors.response.use(
  async (response) => {
    await decrementPending(response.config);
    return response;
  },
  async (error: AxiosError<ApiResponse<unknown>>) => {
    await decrementPending(error.config);
    
    // 401未授权 - 跳转登录
    if (error.response?.status === 401) {
      const { removeToken } = await import('../utils/auth');
      removeToken();
      window.location.href = '/login';
      return Promise.reject(new ApiError('未登录或登录已过期', { status: 401 }));
    }

    const apiError =
      error.code === AxiosError.ERR_NETWORK
        ? new ApiError('网络异常，请检查连接', { cause: error })
        : buildApiErrorFromResponse(error.response, '请求失败，请稍后重试', error)

    // 非静默请求显示错误提示
    if (!error.config?.headers?.['X-Silent']) {
      showErrorMessage(apiError)
    }
    
    return Promise.reject(apiError)
  }
)

export const request = async <T>(config: AxiosRequestConfig): Promise<T> => {
  try {
    const response = await httpClient.request<ApiResponse<T>>(config)
    const payload = response.data

    if (payload?.success) {
      return (payload.data ?? (undefined as T)) as T
    }

    const apiError = new ApiError(payload?.message ?? '业务处理失败', {
      status: response.status,
      errorCode: payload?.errorCode,
      traceId: payload?.traceId,
      errors: payload?.errors
    })

    showErrorMessage(apiError)
    throw apiError
  } catch (error) {
    if (error instanceof ApiError) {
      throw error
    }

    const unknownError = new ApiError('未知错误，请联系管理员', { cause: error })
    showErrorMessage(unknownError)
    throw unknownError
  }
}

/**
 * 静默请求（不触发全局 Loading 和错误提示）
 */
export async function silentRequest<T>(config: AxiosRequestConfig): Promise<T> {
  return request<T>({
    ...config,
    headers: { ...config.headers, 'X-Silent': 'true' }
  });
}

export { httpClient }
