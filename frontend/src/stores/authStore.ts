import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import { secureStorage } from '../utils/secureStorage';

type UserInfo = {
  userId: string;
  userName: string;
  nickName: string;
  avatar?: string;
  email?: string;
  phoneNumber?: string;
  departmentId?: number;
  departmentName?: string;
  sex?: string;
  status: string;
};

type AuthState = {
  token: string | null;
  refreshToken: string | null;
  userInfo: UserInfo | null;
  
  setToken: (token: string | null) => void;
  setRefreshToken: (refreshToken: string | null) => void;
  setUserInfo: (userInfo: UserInfo | null) => void;
  logout: () => void;
};

/**
 * 自定义加密存储引擎
 * 将 Zustand 的 persist 存储引擎替换为加密版本
 */
const encryptedStorage = {
  getItem: (name: string): string | null => {
    return secureStorage.getItem(name);
  },
  setItem: (name: string, value: string): void => {
    secureStorage.setItem(name, value);
  },
  removeItem: (name: string): void => {
    secureStorage.removeItem(name);
  }
};

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      refreshToken: null,
      userInfo: null,

      setToken: (token) => set({ token }),
      setRefreshToken: (refreshToken) => set({ refreshToken }),
      setUserInfo: (userInfo) => set({ userInfo }),
      
      logout: () => set({
        token: null,
        refreshToken: null,
        userInfo: null
      })
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => encryptedStorage)
    }
  )
);

