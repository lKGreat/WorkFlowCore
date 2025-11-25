/**
 * 状态枚举定义
 */

// 用户状态
export enum UserStatus {
  NORMAL = '0',
  DISABLED = '1'
}

// 流程定义状态
export enum ProcessStatus {
  ENABLED = 1,
  DISABLED = 0
}

// 任务实例状态
export enum TaskStatus {
  PENDING = 'Pending',
  PROCESSING = 'Processing',
  COMPLETED = 'Completed',
  REJECTED = 'Rejected',
  CANCELLED = 'Cancelled'
}

// 流程实例状态
export enum ProcessInstanceStatus {
  RUNNING = 'Running',
  COMPLETED = 'Completed',
  SUSPENDED = 'Suspended',
  TERMINATED = 'Terminated'
}

// 审批结果
export enum ApprovalResult {
  APPROVED = 'Approved',
  REJECTED = 'Rejected',
  PENDING = 'Pending'
}

// 文件上传状态
export enum UploadStatus {
  UPLOADING = 'uploading',
  SUCCESS = 'success',
  ERROR = 'error',
  PAUSED = 'paused'
}

// 登录方式
export enum LoginType {
  PASSWORD = 'password',
  QR_CODE = 'qrcode',
  SMS = 'sms',
  THIRD_PARTY = 'thirdparty'
}

// 第三方平台类型
export enum ThirdPartyProvider {
  WECHAT = 'WeChat',
  QQ = 'QQ',
  ALIPAY = 'Alipay'
}

