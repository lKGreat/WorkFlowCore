/**
 * 登录请求
 */
export type LoginRequest = {
  username: string;
  password: string;
  captchaCode?: string;
};

/**
 * 登录响应
 */
export type LoginResponse = {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserInfo;
};

/**
 * 用户信息
 */
export type UserInfo = {
  id: number;
  username: string;
  email: string;
  fullName: string;
  roles: string[];
  permissions: string[];
};

/**
 * 二维码登录状态
 */
export enum QrCodeLoginStatus {
  Waiting = 'Waiting',
  Scanning = 'Scanning',
  Confirmed = 'Confirmed',
  Expired = 'Expired',
  Cancelled = 'Cancelled',
}

/**
 * 二维码登录轮询响应
 */
export type QrCodeLoginPollResponse = {
  status: QrCodeLoginStatus;
  accessToken?: string;
  refreshToken?: string;
  user?: UserInfo;
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
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
};
