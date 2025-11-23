export interface UserListDto {
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
}

export interface CreateUserInput {
  userName: string;
  nickName?: string;
  password: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds: string[];
  sex?: string;
  status: string;
}

export interface UpdateUserInput {
  userId: string;
  nickName?: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  roleIds: string[];
  sex?: string;
  status: string;
}

export interface UserPagedRequest {
  pageIndex: number;
  pageSize: number;
  userName?: string;
  phoneNumber?: string;
  departmentId?: number;
  status?: string;
  beginTime?: string;
  endTime?: string;
}

