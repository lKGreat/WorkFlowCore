/**
 * 登录请求（与后端 UsernameLoginInput 保持一致）
 */
export type LoginRequest = {
  userName: string;
  password: string;
  captchaUuid?: string;
  captchaCode?: string;
  rememberMe?: boolean;
};

/**
 * 登录响应（与后端 LoginResponse 保持一致）
 */
export type LoginResponse = {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: UserInfo;
};

/**
 * 用户信息（与后端 UserDto 保持一致）
 */
export type UserInfo = {
  id: number;
  userName: string;
  realName: string;
  email: string;
  phone: string;
  isEnabled: boolean;
};

/**
 * 二维码登录状态（与后端 QrCodeStatus 保持一致）
 */
export enum QrCodeLoginStatus {
  Pending = 'Pending',
  Scanned = 'Scanned',
  Confirmed = 'Confirmed',
  Expired = 'Expired',
  Cancelled = 'Cancelled',
}

/**
 * 二维码登录轮询响应（与后端 QrCodeLoginResult 保持一致）
 */
export type QrCodeLoginPollResponse = {
  status: QrCodeLoginStatus;
  userId?: string;
  accessToken?: string;
  refreshToken?: string;
  expiresIn?: number;
};

/**
 * 第三方登录提供商
 */
export enum ThirdPartyProvider {
  WeChat = 'WeChat',
  DingTalk = 'DingTalk',
  GitHub = 'GitHub',
}

/**
 * 第三方绑定请求
 */
export type ThirdPartyBindRequest = {
  provider: ThirdPartyProvider;
  code: string;
  state: string;
};

/**
 * 刷新令牌请求
 */
export type RefreshTokenRequest = {
  refreshToken: string;
};

/**
 * 刷新令牌响应
 */
export type RefreshTokenResponse = {
  token: string;
  refreshToken: string;
  expiresAt: string;
};
