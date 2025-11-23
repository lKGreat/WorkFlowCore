import { create } from 'zustand';

export interface RouterMeta {
  title: string;
  icon?: string;
  noCache?: boolean;
  link?: string;
}

export interface RouterConfig {
  name: string;
  path: string;
  hidden?: boolean;
  redirect?: string;
  component: string;
  alwaysShow?: boolean;
  meta?: RouterMeta;
  children?: RouterConfig[];
}

interface RouterState {
  routes: RouterConfig[];
  addRoutes: RouterConfig[];
  
  setRoutes: (routes: RouterConfig[]) => void;
  clearRoutes: () => void;
}

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

