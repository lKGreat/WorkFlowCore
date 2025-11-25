/**
 * 登录日志DTO
 */
export type LoginLogDto = {
  infoId: number;
  userName: string;
  userId?: string;
  status: string;
  ipaddr: string;
  loginLocation: string;
  browser: string;
  os: string;
  msg: string;
  loginTime: string;
  clientId?: string;
};

/**
 * 登录日志查询DTO
 */
export type LoginLogQueryDto = {
  pageIndex?: number;
  pageSize?: number;
  userName?: string;
  userId?: string;
  status?: string;
  ipaddr?: string;
  beginTime?: string;
  endTime?: string;
};

/**
 * 操作日志DTO
 */
export type OperationLogDto = {
  id: number;
  operationModule: string;
  operationType: string;
  operationMethod: string;
  operationUrl: string;
  operationIp: string;
  operationLocation: string;
  operationParam?: string;
  operationResult?: string;
  operationTime: string;
  operationUser: string;
  operationUserId?: string;
  operationDuration: number;
  creationTime: string;
};

/**
 * 操作日志查询DTO
 */
export type OperationLogPagedRequest = {
  pageIndex?: number;
  pageSize?: number;
  operationModule?: string;
  operationType?: string;
  operationUser?: string;
  beginTime?: string;
  endTime?: string;
};

/**
 * 在线用户DTO
 */
export type OnlineUserDto = {
  userId: string;
  userName: string;
  nickName: string;
  deptName?: string;
  ipaddr: string;
  loginLocation?: string;
  browser?: string;
  os?: string;
  loginTime: string;
  token?: string;
};

/**
 * 服务器信息DTO
 */
export type ServerInfoDto = {
  cpu: CpuInfo;
  memory: MemoryInfo;
  disks: DiskInfo[];
  system: SystemInfo;
};

/**
 * CPU信息
 */
export type CpuInfo = {
  name: string;
  coreCount: number;
  used: number;
  free: number;
};

/**
 * 内存信息
 */
export type MemoryInfo = {
  total: number;
  used: number;
  free: number;
  usageRate: number;
};

/**
 * 磁盘信息
 */
export type DiskInfo = {
  dirName: string;
  sysTypeName: string;
  typeName: string;
  total: number;
  free: number;
  used: number;
  usageRate: number;
};

/**
 * 系统信息
 */
export type SystemInfo = {
  computerName: string;
  computerIp: string;
  osName: string;
  osArch: string;
  userDomain?: string;
  userName?: string;
};

/**
 * 定时任务DTO
 */
export type TaskDto = {
  id: number;
  taskName: string;
  taskGroup: string;
  invokeTarget: string;
  cronExpression: string;
  status: number;
  concurrent: number;
  misfirePolicy: number;
  remark?: string;
  creationTime?: string;
};

/**
 * 任务查询DTO
 */
export type TaskQueryDto = {
  pageIndex?: number;
  pageSize?: number;
  taskName?: string;
  taskGroup?: string;
  status?: number;
};

/**
 * 任务日志DTO
 */
export type TaskLogDto = {
  id: number;
  taskId: number;
  taskName: string;
  taskGroup: string;
  invokeTarget: string;
  status: number;
  logInfo?: string;
  exception?: string;
  startTime?: string;
  endTime?: string;
  duration?: number;
  creationTime: string;
};

/**
 * 任务日志查询DTO
 */
export type TaskLogQueryDto = {
  pageIndex?: number;
  pageSize?: number;
  taskId?: number;
  taskName?: string;
  status?: number;
  beginTime?: string;
  endTime?: string;
};

