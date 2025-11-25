/**
 * 路由元数据
 */
export type RouterMeta = {
  title: string;
  icon?: string;
  noCache?: boolean;
  link?: string;
  titleKey?: string;
  isNew?: number;
  iconColor?: string;
  permi?: string;
};

/**
 * 后端路由数据结构
 */
export type BackendRoute = {
  name: string;
  path: string;
  hidden?: boolean;
  redirect?: string;
  component: string;
  alwaysShow?: boolean;
  query?: string;
  meta?: RouterMeta;
  children?: BackendRoute[];
};

/**
 * 路由配置
 */
export type RouterConfig = {
  name: string;
  path: string;
  hidden?: boolean;
  redirect?: string;
  component: string;
  alwaysShow?: boolean;
  meta?: RouterMeta;
  children?: RouterConfig[];
};

