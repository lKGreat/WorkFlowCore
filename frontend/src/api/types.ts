/**
 * API 统一响应结构
 */
export type ApiResponse<T = unknown> = {
  success: boolean
  message: string
  data?: T
  errorCode?: string
  timestamp: number
  traceId?: string
  errors?: Record<string, string[]>
}

/**
 * 分页响应结构（与后端 PagedResponse<T> 保持一致）
 */
export type PagedResponse<T> = {
  /** 数据列表 */
  items: T[]
  /** 总记录数 */
  totalCount: number
  /** 当前页码（从1开始） */
  pageIndex: number
  /** 每页大小 */
  pageSize: number
  /** 总页数（后端计算属性） */
  totalPages?: number
  /** 是否有上一页（后端计算属性） */
  hasPreviousPage?: boolean
  /** 是否有下一页（后端计算属性） */
  hasNextPage?: boolean
}

/**
 * 分页请求参数（与后端 PagedRequest 保持一致）
 */
export type PagedRequest = {
  /** 页码（从1开始，默认1） */
  pageIndex?: number
  /** 每页大小（默认10，上限100） */
  pageSize?: number
  /** 排序字段 */
  orderBy?: string
  /** 是否降序 */
  descending?: boolean
  /** 搜索关键词 */
  keyword?: string
}
