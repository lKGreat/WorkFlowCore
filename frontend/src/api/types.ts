export type ApiResponse<T = unknown> = {
  success: boolean
  message: string
  data?: T
  errorCode?: string
  timestamp: number
  traceId?: string
  errors?: Record<string, string[]>
}
