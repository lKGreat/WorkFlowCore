import { httpClient } from '@/api/httpClient';
import type {
  LoginRequest,
  LoginResponse,
  QrCodeLoginPollResponse,
  ThirdPartyBindRequest,
  RefreshTokenRequest,
  RefreshTokenResponse,
} from '../types';

const BASE_URL = '/api/Auth';

/**
 * 认证服务
 */
export const authService = {
  /**
   * 用户名密码登录
   */
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await httpClient.post<LoginResponse>(`${BASE_URL}/login`, data);
    return response.data;
  },

  /**
   * 登出
   */
  async logout(): Promise<void> {
    await httpClient.post(`${BASE_URL}/logout`);
  },

  /**
   * 生成二维码
   */
  async generateQrCode(): Promise<{ qrCodeId: string; qrCodeUrl: string }> {
    const response = await httpClient.post<{ qrCodeId: string; qrCodeUrl: string }>(
      `${BASE_URL}/qrcode/generate`
    );
    return response.data;
  },

  /**
   * 轮询二维码登录状态
   */
  async pollQrCodeStatus(qrCodeId: string): Promise<QrCodeLoginPollResponse> {
    const response = await httpClient.get<QrCodeLoginPollResponse>(
      `${BASE_URL}/qrcode/poll/${qrCodeId}`
    );
    return response.data;
  },

  /**
   * 第三方登录重定向
   */
  getThirdPartyLoginUrl(provider: string): string {
    return `${BASE_URL}/third-party/${provider}`;
  },

  /**
   * 第三方登录回调绑定
   */
  async bindThirdParty(data: ThirdPartyBindRequest): Promise<LoginResponse> {
    const response = await httpClient.post<LoginResponse>(
      `${BASE_URL}/third-party/bind`,
      data
    );
    return response.data;
  },

  /**
   * 刷新令牌
   */
  async refreshToken(data: RefreshTokenRequest): Promise<RefreshTokenResponse> {
    const response = await httpClient.post<RefreshTokenResponse>(
      `${BASE_URL}/refresh`,
      data
    );
    return response.data;
  },

  /**
   * 获取当前用户信息
   */
  async getCurrentUser(): Promise<LoginResponse['user']> {
    const response = await httpClient.get<LoginResponse['user']>(`${BASE_URL}/me`);
    return response.data;
  },
};
