export type ApiErrorInit = {
  status?: number
  errorCode?: string
  traceId?: string
  errors?: Record<string, string[]>
  cause?: unknown
}

/**
 * 统一 API 错误对象
 */
export class ApiError extends Error {
  status?: number
  errorCode?: string
  traceId?: string
  errors?: Record<string, string[]>

  constructor(message: string, init?: ApiErrorInit) {
    super(message)
    this.name = 'ApiError'
    this.status = init?.status
    this.errorCode = init?.errorCode
    this.traceId = init?.traceId
    this.errors = init?.errors
    if (init?.cause) {
      this.cause = init.cause
    }
  }
}

export const isApiError = (error: unknown): error is ApiError =>
  error instanceof ApiError
