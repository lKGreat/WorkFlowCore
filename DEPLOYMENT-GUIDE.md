# WorkFlowCore 部署指南

本文档说明如何部署和运行WorkFlowCore审批流系统。

## 1. 环境要求

### 后端
- **.NET SDK**: 10.0或更高版本
- **数据库**: SQLite (默认) 或 SQL Server / PostgreSQL / MySQL
- **操作系统**: Windows / Linux / macOS

### 前端
- **Node.js**: 18.0或更高版本
- **npm**: 9.0或更高版本

---

## 2. 快速开始

### 2.1 克隆项目

```bash
git clone https://github.com/your-repo/WorkFlowCore.git
cd WorkFlowCore
```

### 2.2 配置数据库

编辑 `src/WorkFlowCore.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=workflow.db"
  }
}
```

### 2.3 运行数据库迁移

```bash
cd src/WorkFlowCore.Infrastructure
dotnet ef database update --startup-project ../WorkFlowCore.API
```

### 2.4 启动后端

```bash
cd ../WorkFlowCore.API
dotnet run
```

后端API将运行在 `https://localhost:5001`

### 2.5 启动前端

打开新终端：

```bash
cd frontend
npm install
npm run dev
```

前端应用将运行在 `http://localhost:5173`

### 2.6 访问应用

浏览器打开: `http://localhost:5173/login`

**默认测试账号** (由DbInitializer创建):
- 用户名: `admin`
- 密码: 查看 `DbInitializer.cs`

---

## 3. 数据库迁移

### 3.1 创建新迁移

每次修改实体后需要创建迁移：

```bash
cd src/WorkFlowCore.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../WorkFlowCore.API
```

### 3.2 应用迁移

```bash
dotnet ef database update --startup-project ../WorkFlowCore.API
```

### 3.3 回滚迁移

```bash
dotnet ef database update PreviousMigrationName --startup-project ../WorkFlowCore.API
```

### 3.4 删除最后一个迁移

```bash
dotnet ef migrations remove --startup-project ../WorkFlowCore.API
```

---

## 4. 配置说明

### 4.1 JWT配置

```json
{
  "JwtSettings": {
    "SecretKey": "至少32位的密钥",
    "Issuer": "WorkFlowCoreAPI",
    "Audience": "WorkFlowCoreClients",
    "ExpirationMinutes": 1440
  }
}
```

### 4.2 雪花算法配置

```json
{
  "SnowflakeId": {
    "WorkerId": 1,
    "DatacenterId": 1
  }
}
```

**注意**: 分布式部署时每个节点的WorkerId必须不同(0-31)

### 4.3 CORS配置

开发环境默认允许所有来源，生产环境需修改：

```csharp
// WorkFlowCoreHttpApiModule.cs
policy.WithOrigins("https://yourdomain.com")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
```

### 4.4 文件存储配置

支持本地/阿里云OSS/腾讯云COS/AWS S3/Azure Blob:

```json
{
  "FileStorage": {
    "Provider": "Local",
    "Local": {
      "BasePath": "uploads",
      "MaxFileSize": 104857600
    }
  }
}
```

详见文件存储提供商管理界面。

---

## 5. 生产部署

### 5.1 发布后端

```bash
cd src/WorkFlowCore.API
dotnet publish -c Release -o publish
```

### 5.2 发布前端

```bash
cd frontend
npm run build
```

生成的文件在 `frontend/dist` 目录。

### 5.3 使用Docker部署 (推荐)

创建 `Dockerfile`:

```dockerfile
# 后端
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/WorkFlowCore.API/", "WorkFlowCore.API/"]
COPY ["src/WorkFlowCore.Application/", "WorkFlowCore.Application/"]
COPY ["src/WorkFlowCore.Domain/", "WorkFlowCore.Domain/"]
COPY ["src/WorkFlowCore.Infrastructure/", "WorkFlowCore.Infrastructure/"]
COPY ["src/WorkFlowCore.Engine/", "WorkFlowCore.Engine/"]

RUN dotnet restore "WorkFlowCore.API/WorkFlowCore.API.csproj"
RUN dotnet build "WorkFlowCore.API/WorkFlowCore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkFlowCore.API/WorkFlowCore.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkFlowCore.API.dll"]
```

构建镜像：

```bash
docker build -t workflowcore-api .
docker run -d -p 5000:80 workflowcore-api
```

### 5.4 使用Nginx部署前端

```nginx
server {
    listen 80;
    server_name yourdomain.com;
    root /var/www/workflowcore/frontend/dist;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

---

## 6. 环境变量

生产环境建议使用环境变量而非appsettings.json：

```bash
export ConnectionStrings__DefaultConnection="Your-Database-Connection"
export JwtSettings__SecretKey="Your-Secret-Key"
export Sms__Aliyun__AccessKeyId="Your-AccessKeyId"
export Sms__Aliyun__AccessKeySecret="Your-AccessKeySecret"
```

---

## 7. 健康检查

访问 `http://localhost:5000/health` 检查服务状态：

```json
{
  "status": "healthy",
  "timestamp": "2025-11-23T10:00:00Z"
}
```

---

## 8. Swagger API文档

开发环境自动启用Swagger文档：

访问: `https://localhost:5001/swagger`

---

## 9. 常见问题

### 9.1 数据库连接失败

**检查**:
- 连接字符串是否正确
- 数据库服务是否启动
- 防火墙是否允许连接

### 9.2 前端无法访问API

**检查**:
- CORS配置是否正确
- API地址是否正确 (查看`vite.config.ts`)
- 后端是否正常运行

### 9.3 JWT Token无效

**检查**:
- SecretKey是否一致
- Token是否过期
- 时钟是否同步

### 9.4 文件上传失败

**检查**:
- 文件大小是否超限
- 存储路径权限
- 存储提供商配置

---

## 10. 性能优化

### 10.1 使用Redis缓存

```bash
dotnet add package Volo.Abp.Caching.StackExchangeRedis
```

修改配置：

```json
{
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

### 10.2 使用真实数据库

生产环境建议使用 SQL Server / PostgreSQL / MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WorkFlowCore;User Id=sa;Password=YourPassword;"
  }
}
```

### 10.3 启用响应压缩

已在`WorkFlowCoreHttpApiModule`中配置。

### 10.4 使用CDN

将前端静态资源部署到CDN以加速访问。

---

## 11. 监控和日志

### 11.1 查看日志

日志输出到控制台，生产环境建议配置日志持久化：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 11.2 集成APM

可集成 Application Insights / Elastic APM / SkyWalking 等。

---

## 12. 安全检查清单

- [ ] 更改默认JWT密钥
- [ ] 配置HTTPS证书
- [ ] 限制CORS来源
- [ ] 配置API速率限制
- [ ] 启用SQL参数化查询(已默认)
- [ ] 配置安全HTTP头
- [ ] 定期备份数据库
- [ ] 更新依赖包到最新版本

---

## 13. 备份和恢复

### 13.1 备份SQLite数据库

```bash
cp src/WorkFlowCore.API/workflow.db backup/workflow_$(date +%Y%m%d).db
```

### 13.2 恢复数据库

```bash
cp backup/workflow_20251123.db src/WorkFlowCore.API/workflow.db
```

---

## 14. 相关链接

- [ABP Framework文档](https://docs.abp.io/)
- [Entity Framework Core文档](https://docs.microsoft.com/ef/core/)
- [React官方文档](https://react.dev/)
- [Ant Design组件库](https://ant.design/)
- [WorkflowCore引擎](https://github.com/danielgerlag/workflow-core)

