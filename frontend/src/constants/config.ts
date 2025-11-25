/**
 * 配置常量
 */

// API 配置
export const API_CONFIG = {
  BASE_URL: import.meta.env.VITE_API_BASE_URL ?? '/api',
  TIMEOUT: 15000,
  RETRY_COUNT: 3,
  RETRY_DELAY: 1000
} as const;

// 分页配置
export const PAGINATION_CONFIG = {
  DEFAULT_PAGE_SIZE: 10,
  PAGE_SIZE_OPTIONS: ['10', '20', '50', '100'],
  SHOW_SIZE_CHANGER: true,
  SHOW_QUICK_JUMPER: true
} as const;

// 文件上传配置
export const UPLOAD_CONFIG = {
  MAX_FILE_SIZE: 100 * 1024 * 1024, // 100MB
  CHUNK_SIZE: 2 * 1024 * 1024, // 2MB
  ALLOWED_FILE_TYPES: [
    'image/jpeg',
    'image/png',
    'image/gif',
    'application/pdf',
    'application/msword',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
  ],
  MAX_FILES_COUNT: 10
} as const;

// 本地存储 Key
export const STORAGE_KEYS = {
  TOKEN: 'auth-token',
  USER_INFO: 'user-info',
  THEME: 'theme',
  LANGUAGE: 'language'
} as const;

// 时间格式
export const DATE_FORMAT = {
  DATE: 'YYYY-MM-DD',
  TIME: 'HH:mm:ss',
  DATETIME: 'YYYY-MM-DD HH:mm:ss',
  TIMESTAMP: 'YYYY-MM-DD HH:mm:ss.SSS'
} as const;

// 防抖/节流延迟
export const DEBOUNCE_DELAY = 500; // 毫秒
export const THROTTLE_DELAY = 300; // 毫秒

// 路由路径
export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  DESIGNER: '/designer',
  INSTANCES: '/instances',
  SYSTEM_USER: '/system/user',
  SYSTEM_ROLE: '/system/role',
  FILE_UPLOAD: '/file-upload'
} as const;

