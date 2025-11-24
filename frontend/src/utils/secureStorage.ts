import CryptoJS from 'crypto-js';

/**
 * 加密存储密钥
 * 生产环境应从环境变量读取，开发环境使用固定值
 */
const SECRET_KEY = import.meta.env.VITE_STORAGE_KEY || 'WorkFlowCore-Default-Secret-Key-2025';

/**
 * 加密的本地存储工具
 * 用于安全地存储敏感信息（如 Token）
 */
export const secureStorage = {
  /**
   * 加密并存储数据
   */
  setItem: (key: string, value: string): void => {
    try {
      const encrypted = CryptoJS.AES.encrypt(value, SECRET_KEY).toString();
      localStorage.setItem(key, encrypted);
    } catch (error) {
      console.error('SecureStorage setItem failed:', error);
      // 降级：无加密存储
      localStorage.setItem(key, value);
    }
  },

  /**
   * 获取并解密数据
   */
  getItem: (key: string): string | null => {
    try {
      const encrypted = localStorage.getItem(key);
      if (!encrypted) return null;

      // 尝试解密
      const bytes = CryptoJS.AES.decrypt(encrypted, SECRET_KEY);
      const decrypted = bytes.toString(CryptoJS.enc.Utf8);

      // 如果解密结果为空，可能是明文存储的旧数据
      if (!decrypted) {
        return encrypted; // 返回原始值（兼容旧数据）
      }

      return decrypted;
    } catch (error) {
      console.error('SecureStorage getItem failed:', error);
      // 降级：直接读取
      return localStorage.getItem(key);
    }
  },

  /**
   * 删除数据
   */
  removeItem: (key: string): void => {
    localStorage.removeItem(key);
  },

  /**
   * 清空所有数据
   */
  clear: (): void => {
    localStorage.clear();
  }
};

/**
 * 跨标签页同步机制
 * 监听其他标签页的 storage 变更，同步到当前标签页
 */
export const setupStorageSync = (onSyncCallback: (key: string, newValue: string | null) => void) => {
  const handleStorageChange = (event: StorageEvent) => {
    if (event.key && event.newValue !== null) {
      // 解密新值
      const decrypted = secureStorage.getItem(event.key);
      onSyncCallback(event.key, decrypted);
    } else if (event.key && event.newValue === null) {
      // 删除事件
      onSyncCallback(event.key, null);
    }
  };

  window.addEventListener('storage', handleStorageChange);

  // 返回清理函数
  return () => {
    window.removeEventListener('storage', handleStorageChange);
  };
};



