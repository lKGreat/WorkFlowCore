/**
 * 用户列表项（与后端 UserListDto 保持一致）
 */
export type UserListItem = {
  userId: string;
  userName: string;
  nickName: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  departmentName?: string;
  sex?: string;
  status: string;
  creationTime: string;
  lastLoginTime?: string;
  roles?: string[];
};

/**
 * 用户列表DTO（兼容旧代码）
 */
export type UserListDto = UserListItem;

/**
 * 创建用户输入（与后端 CreateUserInput 保持一致）
 */
export type CreateUserInput = {
  userName: string;
  nickName?: string;
  password: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds?: string[];
  sex?: string;
  status?: string;
};

/**
 * 更新用户输入（与后端 UpdateUserInput 保持一致）
 */
export type UpdateUserInput = {
  userId?: string;
  nickName?: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds?: string[];
  sex?: string;
  status?: string;
};

/**
 * 用户分页请求（与后端 UserPagedRequest 保持一致）
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
 * 角色列表项（与后端 RoleDto 保持一致）
 */
export type RoleListItem = {
  roleId: string;
  roleName: string;
  roleKey: string;
  roleSort: number;
  dataScope?: string;
  status: string;
  remark?: string;
  creationTime: string;
};

/**
 * 创建用户请求
 */
export type CreateUserRequest = CreateUserInput;

/**
 * 更新用户请求
 */
export type UpdateUserRequest = UpdateUserInput;

/**
 * 创建角色请求（与后端 CreateRoleInput 保持一致）
 */
export type CreateRoleRequest = {
  roleName: string;
  roleKey: string;
  roleSort?: number;
  dataScope?: string;
  status?: string;
  remark?: string;
  menuIds?: number[];
};

/**
 * 更新角色请求（与后端 UpdateRoleInput 保持一致）
 */
export type UpdateRoleRequest = {
  roleId?: string;
  roleName?: string;
  roleKey?: string;
  roleSort?: number;
  dataScope?: string;
  status?: string;
  remark?: string;
  menuIds?: number[];
};

/**
 * 用户表单数据
 */
export type UserFormData = CreateUserRequest | (UpdateUserRequest & { userId: string });

/**
 * 角色表单数据
 */
export type RoleFormData = CreateRoleRequest | (UpdateRoleRequest & { roleId: string });
