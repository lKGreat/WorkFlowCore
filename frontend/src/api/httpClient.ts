import axios, { AxiosError, type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios'
import { message } from 'antd'
import { ApiError } from './apiError'
import type { ApiResponse } from './types'
import { getToken } from '../utils/auth'
import { useAuthStore } from '../stores/authStore'
import type { RefreshTokenResponse } from '../features/auth/types'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

// Token 刷新状态
let isRefreshing = false
let failedQueue: Array<{
  resolve: (value: unknown) => void
  reject: (reason?: unknown) => void
}> = []

const processQueue = (error: Error | null, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })
  failedQueue = []
}

const httpClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求计数器（用于全局 Loading）
let pendingRequests = 0

// 预加载 uiStore（避免循环依赖，同时提前缓存）
let uiStoreGetter: (() => ReturnType<typeof import('../stores/uiStore').useUiStore.getState>) | null = null

// 初始化 uiStore 获取器
function initUiStore() {
  if (!uiStoreGetter) {
    import('../stores/uiStore').then((module) => {
      uiStoreGetter = () => module.useUiStore.getState()
    }).catch(() => {
      // 初始化失败，忽略
    })
  }
}

// 立即初始化
initUiStore()

// 同步获取 uiStore（如果已初始化）
function getUiStore() {
  return uiStoreGetter?.()
}

// 请求拦截器 - 添加Token + Loading计数
httpClient.interceptors.request.use(
  (config) => {
    // 自动添加 Token
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // 增加请求计数（排除静默请求）
    if (!config.headers['X-Silent']) {
      pendingRequests++;
      if (pendingRequests === 1) {
        const uiStore = getUiStore();
        uiStore?.setGlobalLoading(true);
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
function decrementPending(config?: AxiosRequestConfig) {
  if (!config?.headers?.['X-Silent']) {
    pendingRequests = Math.max(0, pendingRequests - 1);
    if (pendingRequests === 0) {
      const uiStore = getUiStore();
      uiStore?.setGlobalLoading(false);
    }
  }
}

// 响应拦截器
httpClient.interceptors.response.use(
  (response) => {
    decrementPending(response.config);
    return response;
  },
  async (error: AxiosError<ApiResponse<unknown>>) => {
    decrementPending(error.config);
    
    const originalRequest = error.config
    
    // 401未授权 - 尝试刷新Token
    if (error.response?.status === 401 && originalRequest) {
      // 如果是刷新Token接口本身失败，直接跳转登录
      if (originalRequest.url?.includes('/refresh')) {
        const { removeToken } = await import('../utils/auth');
        const authStore = useAuthStore.getState()
        removeToken();
        authStore.logout();
        window.location.href = '/login';
        return Promise.reject(new ApiError('登录已过期', { status: 401 }));
      }

      // 如果正在刷新Token，将请求加入队列
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        }).then(() => {
          // Token刷新完成后，重试原请求
          return httpClient.request(originalRequest)
        }).catch((err) => {
          return Promise.reject(err)
        })
      }

      // 开始刷新Token
      isRefreshing = true
      const authStore = useAuthStore.getState()
      const refreshToken = authStore.refreshToken

      if (!refreshToken) {
        // 没有refreshToken，直接跳转登录
        isRefreshing = false
        const { removeToken } = await import('../utils/auth');
        removeToken();
        authStore.logout();
        window.location.href = '/login';
        return Promise.reject(new ApiError('登录已过期', { status: 401 }));
      }

      try {
        // 调用刷新Token接口
        const response = await httpClient.post<ApiResponse<RefreshTokenResponse>>(
          '/api/Auth/refresh',
          { refreshToken }
        )
        
        if (response.data.success && response.data.data) {
          const { token, refreshToken: newRefreshToken } = response.data.data
          
          // 更新Token
          authStore.setToken(token)
          authStore.setRefreshToken(newRefreshToken)
          
          // 更新原请求的Authorization头
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${token}`
          }
          
          // 处理队列中的请求
          processQueue(null, token)
          isRefreshing = false
          
          // 重试原请求
          return httpClient.request(originalRequest)
        } else {
          throw new Error('刷新Token失败')
        }
      } catch (refreshError) {
        // 刷新失败，清除状态并跳转登录
        processQueue(refreshError as Error, null)
        isRefreshing = false
        
        const { removeToken } = await import('../utils/auth');
        removeToken();
        authStore.logout();
        window.location.href = '/login';
        
        return Promise.reject(new ApiError('登录已过期', { status: 401 }))
      }
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
