# WorkFlowCore 快速启动指南

## 🚀 5分钟快速启动

### 第一步: 启动后端 (终端1)

```bash
cd src/WorkFlowCore.API
dotnet run
```

等待看到:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

**API地址**: `https://localhost:5001`  
**Swagger文档**: `https://localhost:5001/swagger`

---

### 第二步: 启动前端 (终端2)

```bash
cd frontend
npm run dev
```

等待看到:
```
  VITE v7.2.4  ready in 1234 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

**应用地址**: `http://localhost:5173`

---

### 第三步: 登录系统

1. 浏览器打开 `http://localhost:5173/login`
2. 选择登录方式:

#### 方式1: 账号登录
- 用户名: `admin`
- 密码: 查看 `DbInitializer.cs` 中的初始化密码
- 验证码: 点击图片刷新

#### 方式2: 手机登录
- 手机号: `13800138000`
- 点击"发送验证码"
- 查看控制台日志获取验证码
```
[Mock阿里云短信] 手机号: 13800138000, 验证码: 123456
```

#### 方式3: 扫码登录
- 切换到"扫码登录"Tab
- 需要移动端APP配合(暂未实现)

#### 方式4: 第三方登录
- 点击底部图标 (微信/QQ/支付宝/Apple)
- 需要配置真实ClientID (当前为Mock)

---

## 🔧 配置说明

### 默认配置 (已可用)
- ✅ 数据库: SQLite (`workflow.db`)
- ✅ JWT: 预配置密钥
- ✅ 雪花算法: WorkerId=1, DatacenterId=1
- ✅ 缓存: 内存缓存
- ✅ 短信: Mock模式
- ✅ OAuth: Mock模式

### 生产环境配置 (需修改)

编辑 `src/WorkFlowCore.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Production-Database-Connection"
  },
  "JwtSettings": {
    "SecretKey": "Your-Secret-Key-At-Least-32-Characters-Long!"
  },
  "Sms": {
    "Provider": "Aliyun",
    "Aliyun": {
      "AccessKeyId": "your-accesskey-id",
      "AccessKeySecret": "your-accesskey-secret"
    }
  },
  "Authentication": {
    "WeChat": {
      "ClientId": "your-wechat-appid",
      "AppSecret": "your-wechat-appsecret"
    }
  }
}
```

详见:
- [短信配置](SMS-CONFIG-GUIDE.md)
- [OAuth配置](OAUTH-CONFIG-GUIDE.md)

---

## 📖 功能演示

### 1. 查看API文档
访问: `https://localhost:5001/swagger`

### 2. 测试登录接口
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "admin",
    "password": "Admin@123",
    "captchaUuid": "xxx",
    "captchaCode": "ABCD"
  }'
```

### 3. 查看数据库
```bash
# 使用SQLite客户端打开
sqlite3 src/WorkFlowCore.API/workflow.db

# 查看表
.tables

# 查看用户
SELECT * FROM AbpUsers;
```

### 4. 查看日志
控制台会显示详细的日志信息:
```
[Mock阿里云短信] 手机号: xxx, 验证码: xxx
[WorkFlowCore] 操作日志已记录
[ABP] 审计日志已保存
```

---

## 🎯 核心功能测试

### 测试清单

#### 认证功能
- [ ] 账号密码登录
- [ ] 图形验证码刷新
- [ ] 手机验证码登录
- [ ] 扫码登录(需移动端)
- [ ] 第三方登录(需配置)
- [ ] 退出登录

#### 流程管理
- [ ] 创建流程定义
- [ ] 流程设计器
- [ ] 部署流程
- [ ] 启动流程实例
- [ ] 查看流程实例

#### 文件管理
- [ ] 文件上传
- [ ] 分片上传
- [ ] 文件列表
- [ ] 文件下载
- [ ] 文件删除

#### 权限管理
- [ ] 用户信息查询 (/getInfo)
- [ ] 动态路由获取 (/getRouters)
- [ ] 菜单管理

---

## 🐛 常见问题

### 问题1: 后端启动失败
**错误**: `Unable to resolve service for type 'DbContextOptions'`

**解决**:
```bash
cd src/WorkFlowCore.Infrastructure
dotnet ef database update --startup-project ../WorkFlowCore.API
```

### 问题2: 前端连接API失败
**错误**: `Network Error` 或 `CORS Error`

**检查**:
- 后端是否正常运行
- `vite.config.ts`中的proxy配置
- CORS配置是否允许`localhost:5173`

### 问题3: 登录后Token无效
**原因**: JWT密钥不一致

**解决**: 确保`appsettings.json`中的SecretKey至少32位

### 问题4: 图形验证码不显示
**原因**: SkiaSharp字体缺失

**解决**: 
- Windows: 自动使用系统字体
- Linux: 安装字体包 `apt-get install fontconfig`

### 问题5: 短信验证码收不到
**原因**: 使用Mock模式

**解决**: 查看控制台日志获取验证码,或配置真实短信服务

---

## 📱 移动端集成

### 扫码登录流程

#### 移动端
```typescript
// 1. 扫描Web端二维码获取uuid
const qrData = JSON.parse(qrcode);

// 2. 调用扫描接口
POST /api/auth/qrcode/scan
{
  "uuid": qrData.uuid
}

// 3. 用户确认后调用确认接口
POST /api/auth/qrcode/confirm
{
  "uuid": qrData.uuid
}
```

#### Web端
```typescript
// 轮询二维码状态(每2秒)
GET /api/auth/qrcode/poll?uuid=xxx

// 状态变化: WaitScan → Scanned → Confirmed
// Confirmed时返回AccessToken
```

---

## 🎨 界面预览

### 登录页
- 现代化渐变背景
- 3种登录方式Tab切换
- 图形验证码实时刷新
- 第三方登录图标

### 管理后台
- 顶部导航栏 (Logo + 用户信息)
- 左侧菜单 (可折叠)
- 面包屑导航
- 内容区域

### 流程设计器
- 可视化拖拽设计
- 节点连线
- 属性配置
- 保存/部署

---

## 🔗 相关链接

- **ABP Framework**: https://abp.io/
- **React官方文档**: https://react.dev/
- **Ant Design**: https://ant.design/
- **WorkflowCore**: https://github.com/danielgerlag/workflow-core
- **React Flow**: https://reactflow.dev/

---

## 📞 技术支持

遇到问题? 请按以下顺序查找:

1. 查看 [DEPLOYMENT-GUIDE.md](DEPLOYMENT-GUIDE.md)
2. 查看 [FEATURES-COMPLETED.md](FEATURES-COMPLETED.md)
3. 查看Swagger API文档
4. 检查控制台日志
5. 提交GitHub Issue

---

**🎉 开始你的WorkFlowCore之旅吧!**

