-- 数据库初始化脚本 (SQLite)
-- 插入测试租户数据

-- 插入测试租户
INSERT OR IGNORE INTO Tenants (Id, Name, Code, ContactEmail, IsActive, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    '测试租户',
    'test-tenant',
    'test@example.com',
    1,
    0,
    NULL,
    datetime('now'),
    datetime('now')
);

-- 验证数据
SELECT * FROM Tenants WHERE Id = '00000000-0000-0000-0000-000000000001';

