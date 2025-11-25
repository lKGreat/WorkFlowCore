/**
 * 格式化工具函数
 */

/**
 * 格式化日期时间
 */
export function formatDateTime(
  date: string | Date | number,
  format: string = 'YYYY-MM-DD HH:mm:ss'
): string {
  if (!date) return '';
  
  const d = typeof date === 'string' || typeof date === 'number' 
    ? new Date(date) 
    : date;
  
  if (isNaN(d.getTime())) return '';
  
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  const hours = String(d.getHours()).padStart(2, '0');
  const minutes = String(d.getMinutes()).padStart(2, '0');
  const seconds = String(d.getSeconds()).padStart(2, '0');
  
  return format
    .replace('YYYY', String(year))
    .replace('MM', month)
    .replace('DD', day)
    .replace('HH', hours)
    .replace('mm', minutes)
    .replace('ss', seconds);
}

/**
 * 格式化日期
 */
export function formatDate(date: string | Date | number): string {
  return formatDateTime(date, 'YYYY-MM-DD');
}

/**
 * 格式化时间
 */
export function formatTime(date: string | Date | number): string {
  return formatDateTime(date, 'HH:mm:ss');
}

/**
 * 格式化文件大小
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B';
  
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  
  return `${(bytes / Math.pow(k, i)).toFixed(2)} ${sizes[i]}`;
}

/**
 * 格式化数字（千分位）
 */
export function formatNumber(num: number | string): string {
  const n = typeof num === 'string' ? parseFloat(num) : num;
  if (isNaN(n)) return '0';
  
  return n.toLocaleString('zh-CN');
}

/**
 * 格式化百分比
 */
export function formatPercent(value: number, decimals: number = 2): string {
  return `${(value * 100).toFixed(decimals)}%`;
}

/**
 * 格式化货币
 */
export function formatCurrency(amount: number, currency: string = '¥'): string {
  return `${currency}${formatNumber(amount.toFixed(2))}`;
}

/**
 * 相对时间格式化（xx前）
 */
export function formatRelativeTime(date: string | Date | number): string {
  const d = typeof date === 'string' || typeof date === 'number' 
    ? new Date(date) 
    : date;
  
  if (isNaN(d.getTime())) return '';
  
  const now = new Date();
  const diff = now.getTime() - d.getTime();
  const seconds = Math.floor(diff / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);
  
  if (seconds < 60) return '刚刚';
  if (minutes < 60) return `${minutes}分钟前`;
  if (hours < 24) return `${hours}小时前`;
  if (days < 7) return `${days}天前`;
  
  return formatDateTime(d, 'YYYY-MM-DD');
}

/**
 * 手机号脱敏
 */
export function maskPhone(phone: string): string {
  if (!phone || phone.length !== 11) return phone;
  return phone.replace(/(\d{3})\d{4}(\d{4})/, '$1****$2');
}

/**
 * 邮箱脱敏
 */
export function maskEmail(email: string): string {
  if (!email || !email.includes('@')) return email;
  const [name, domain] = email.split('@');
  if (name.length <= 2) return `*@${domain}`;
  return `${name[0]}****${name[name.length - 1]}@${domain}`;
}

/**
 * 身份证脱敏
 */
export function maskIdCard(idCard: string): string {
  if (!idCard || idCard.length < 15) return idCard;
  return idCard.replace(/(\d{6})\d+(\d{4})/, '$1********$2');
}

