const TOKEN_KEY = 'WFC-Token';

/**
 * 获取Token
 */
export const getToken = (): string | null => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * 设置Token
 */
export const setToken = (token: string): void => {
  localStorage.setItem(TOKEN_KEY, token);
};

/**
 * 移除Token
 */
export const removeToken = (): void => {
  localStorage.removeItem(TOKEN_KEY);
};

