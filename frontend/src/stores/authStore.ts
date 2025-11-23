import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import { secureStorage } from '../utils/secureStorage';

interface UserInfo {
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
}

interface AuthState {
  token: string | null;
  userInfo: UserInfo | null;
  roles: string[];
  permissions: string[];
  
  setToken: (token: string | null) => void;
  setUserInfo: (userInfo: UserInfo | null) => void;
  setRoles: (roles: string[]) => void;
  setPermissions: (permissions: string[]) => void;
  logout: () => void;
}

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
      userInfo: null,
      roles: [],
      permissions: [],

      setToken: (token) => set({ token }),
      setUserInfo: (userInfo) => set({ userInfo }),
      setRoles: (roles) => set({ roles }),
      setPermissions: (permissions) => set({ permissions }),
      
      logout: () => set({
        token: null,
        userInfo: null,
        roles: [],
        permissions: []
      })
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => encryptedStorage)
    }
  )
);

