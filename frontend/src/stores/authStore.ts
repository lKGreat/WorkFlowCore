import { create } from 'zustand';
import { persist } from 'zustand/middleware';

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
      name: 'auth-storage'
    }
  )
);

