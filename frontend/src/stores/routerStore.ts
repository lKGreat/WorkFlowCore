import { create } from 'zustand';
import type { RouterConfig } from '../types/router';

type RouterState = {
  routes: RouterConfig[];
  addRoutes: RouterConfig[];
  
  setRoutes: (routes: RouterConfig[]) => void;
  clearRoutes: () => void;
};

export const useRouterStore = create<RouterState>((set) => ({
  routes: [],
  addRoutes: [],
  
  setRoutes: (routes) => set({
    addRoutes: routes,
    routes: routes
  }),
  
  clearRoutes: () => set({
    routes: [],
    addRoutes: []
  })
}));

