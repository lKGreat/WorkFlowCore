import { request } from '@/api';
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
    return request<LoginResponse>({
      method: 'POST',
      url: `${BASE_URL}/login`,
      data,
    });
  },

  /**
   * 登出
   */
  async logout(): Promise<void> {
    return request<void>({
      method: 'POST',
      url: `${BASE_URL}/logout`,
    });
  },

  /**
   * 生成二维码
   */
  async generateQrCode(): Promise<{ qrCodeId: string; qrCodeUrl: string }> {
    return request<{ qrCodeId: string; qrCodeUrl: string }>({
      method: 'POST',
      url: `${BASE_URL}/qrcode/generate`,
    });
  },

  /**
   * 轮询二维码登录状态
   */
  async pollQrCodeStatus(qrCodeId: string): Promise<QrCodeLoginPollResponse> {
    return request<QrCodeLoginPollResponse>({
      method: 'GET',
      url: `${BASE_URL}/qrcode/poll/${qrCodeId}`,
    });
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
    return request<LoginResponse>({
      method: 'POST',
      url: `${BASE_URL}/third-party/bind`,
      data,
    });
  },

  /**
   * 刷新令牌
   */
  async refreshToken(data: RefreshTokenRequest): Promise<RefreshTokenResponse> {
    return request<RefreshTokenResponse>({
      method: 'POST',
      url: `${BASE_URL}/refresh`,
      data,
    });
  },

  /**
   * 获取当前用户信息
   */
  async getCurrentUser(): Promise<LoginResponse['user']> {
    return request<LoginResponse['user']>({
      method: 'GET',
      url: `${BASE_URL}/me`,
    });
  },
};
