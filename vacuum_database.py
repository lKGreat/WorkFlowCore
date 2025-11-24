#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
数据库收缩脚本
执行 VACUUM 操作优化数据库文件大小
"""
import sqlite3
import sys
import os

sys.stdout.reconfigure(encoding='utf-8')

# 数据库路径
DB_PATH = r'D:\Code\WorkFlowCore\WorkFlowCore\src\WorkFlowCore.API\workflow_dev.db'

def get_database_size():
    """获取数据库文件大小"""
    if os.path.exists(DB_PATH):
        size_bytes = os.path.getsize(DB_PATH)
        size_mb = size_bytes / (1024 * 1024)
        return size_bytes, size_mb
    return 0, 0

def vacuum_database():
    """收缩数据库"""
    print("=" * 60)
    print("数据库收缩脚本")
    print("=" * 60)
    
    if not os.path.exists(DB_PATH):
        print(f"错误: 数据库文件不存在: {DB_PATH}")
        return
    
    # 获取收缩前的大小
    size_before_bytes, size_before_mb = get_database_size()
    print(f"\n收缩前数据库大小: {size_before_mb:.2f} MB ({size_before_bytes:,} 字节)")
    
    try:
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        
        print("\n正在执行 VACUUM 操作...")
        cursor.execute("VACUUM")
        
        # 提交更改
        conn.commit()
        conn.close()
        
        # 获取收缩后的大小
        size_after_bytes, size_after_mb = get_database_size()
        print(f"收缩后数据库大小: {size_after_mb:.2f} MB ({size_after_bytes:,} 字节)")
        
        # 计算节省的空间
        saved_bytes = size_before_bytes - size_after_bytes
        saved_mb = saved_bytes / (1024 * 1024)
        saved_percent = (saved_bytes / size_before_bytes * 100) if size_before_bytes > 0 else 0
        
        print(f"\n节省空间: {saved_mb:.2f} MB ({saved_bytes:,} 字节, {saved_percent:.1f}%)")
        print("\n✓ 数据库收缩完成！")
        
    except Exception as e:
        print(f"\n错误: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    vacuum_database()

