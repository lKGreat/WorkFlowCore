import axios, { AxiosError, type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios'
import { message } from 'antd'
import { ApiError } from './apiError'
import type { ApiResponse } from './types'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

const httpClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json'
  }
})

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

httpClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiResponse<unknown>>) => {
    const apiError =
      error.code === AxiosError.ERR_NETWORK
        ? new ApiError('网络异常，请检查连接', { cause: error })
        : buildApiErrorFromResponse(error.response, '请求失败，请稍后重试', error)

    showErrorMessage(apiError)
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

export { httpClient }
