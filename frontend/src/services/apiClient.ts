import axios, { type AxiosError, type AxiosResponse } from 'axios';
import { message } from 'antd';

// API 基础 URL
const API_BASE_URL = 'http://localhost:5000/api';

// 创建 Axios 实例
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器
apiClient.interceptors.request.use(
  (config) => {
    // MVP 阶段：添加测试租户ID到请求头
    config.headers['X-Tenant-Id'] = '00000000-0000-0000-0000-000000000001';
    
    // 后续可以添加 token
    // const token = localStorage.getItem('token');
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`;
    // }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    // 如果响应包含 ApiResponse 格式，直接返回
    if (response.data && typeof response.data === 'object') {
      return response;
    }
    return response;
  },
  (error: AxiosError) => {
    // 处理错误
    if (error.response) {
      // 服务器返回错误状态码
      const status = error.response.status;
      const data = error.response.data as any;
      
      switch (status) {
        case 400:
          message.error(data?.message || '请求参数错误');
          break;
        case 401:
          message.error('未授权，请登录');
          // 可以跳转到登录页
          // window.location.href = '/login';
          break;
        case 403:
          message.error('没有权限访问');
          break;
        case 404:
          message.error(data?.message || '请求的资源不存在');
          break;
        case 500:
          message.error('服务器错误');
          break;
        default:
          message.error(data?.message || '请求失败');
      }
    } else if (error.request) {
      // 请求已发送但没有收到响应
      message.error('网络错误，请检查您的网络连接');
    } else {
      // 其他错误
      message.error('请求失败');
    }
    
    return Promise.reject(error);
  }
);

// ApiResponse 类型定义
export type ApiResponse<T = any> = {
  success: boolean;
  message?: string;
  data?: T;
  errorCode?: string;
  timestamp?: number;
};

// 封装请求方法
export const api = {
  get: <T = any>(url: string, params?: any) =>
    apiClient.get<ApiResponse<T>>(url, { params }).then((res) => res.data),
  
  post: <T = any>(url: string, data?: any) =>
    apiClient.post<ApiResponse<T>>(url, data).then((res) => res.data),
  
  put: <T = any>(url: string, data?: any) =>
    apiClient.put<ApiResponse<T>>(url, data).then((res) => res.data),
  
  delete: <T = any>(url: string) =>
    apiClient.delete<ApiResponse<T>>(url).then((res) => res.data),
};

export default apiClient;

