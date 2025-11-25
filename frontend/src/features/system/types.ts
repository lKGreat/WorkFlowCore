/**
 * 用户列表项
 */
export type UserListItem = {
  id: number;
  username: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  departmentId?: number;
  departmentName?: string;
  roles: string[];
  isActive: boolean;
  lastLoginTime?: string;
  createdAt: string;
};

/**
 * 用户列表DTO（兼容旧代码）
 */
export type UserListDto = {
  userId: string;
  userName: string;
  nickName: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  departmentName?: string;
  roles: string[];
  sex?: string;
  status: string;
  creationTime: string;
  lastLoginTime?: string;
};

/**
 * 创建用户输入（兼容旧代码）
 */
export type CreateUserInput = {
  userName: string;
  nickName?: string;
  password: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds: string[];
  sex?: string;
  status: string;
};

/**
 * 更新用户输入（兼容旧代码）
 */
export type UpdateUserInput = {
  userId: string;
  nickName?: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds: string[];
  sex?: string;
  status: string;
};

/**
 * 用户分页请求（兼容旧代码）
 */
export type UserPagedRequest = {
  pageIndex: number;
  pageSize: number;
  userName?: string;
  phoneNumber?: string;
  departmentId?: number;
  status?: string;
  beginTime?: string;
  endTime?: string;
};

/**
 * 角色列表项
 */
export type RoleListItem = {
  id: number;
  name: string;
  code: string;
  description?: string;
  permissions: string[];
  userCount: number;
  isDefault: boolean;
  createdAt: string;
};

/**
 * 创建用户请求
 */
export type CreateUserRequest = {
  username: string;
  email: string;
  password: string;
  fullName: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds: number[];
};

/**
 * 更新用户请求
 */
export type UpdateUserRequest = {
  email: string;
  fullName: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds: number[];
  isActive: boolean;
};

/**
 * 创建角色请求
 */
export type CreateRoleRequest = {
  name: string;
  code: string;
  description?: string;
  permissions: string[];
};

/**
 * 更新角色请求
 */
export type UpdateRoleRequest = {
  name: string;
  description?: string;
  permissions: string[];
};

/**
 * 用户表单数据
 */
export type UserFormData = CreateUserRequest | (UpdateUserRequest & { id: number });

/**
 * 角色表单数据
 */
export type RoleFormData = CreateRoleRequest | (UpdateRoleRequest & { id: number });
