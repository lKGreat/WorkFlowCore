# WorkFlowCore MVP 启动指南

## 前置条件

- .NET 10.0 SDK
- Node.js 18+ (推荐使用 LTS 版本)
- SQLite (内置在 .NET 中)

## 后端启动步骤

### 1. 确保数据库迁移已应用

```bash
cd src/WorkFlowCore.API
dotnet ef database update
```

### 2. 初始化测试租户数据

使用 SQLite 客户端或工具执行 `database-init.sql` 脚本，或者手动插入测试租户:

```bash
# 使用 sqlite3 命令行工具
sqlite3 workflow.db < ../../database-init.sql
```

或者使用 DB Browser for SQLite 等图形界面工具执行脚本。

### 3. 启动后端服务

```bash
dotnet run --project src/WorkFlowCore.API
```

后端将在 `http://localhost:5000` 启动。

Swagger 文档地址: `http://localhost:5000/swagger`

## 前端启动步骤

### 1. 安装依赖

```bash
cd frontend
npm install
```

### 2. 启动前端开发服务器

```bash
npm run dev
```

前端将在 `http://localhost:5173` 启动（Vite 默认端口）。

## 测试流程

### 1. 访问前端应用

打开浏览器访问 `http://localhost:5173`

### 2. 创建流程定义

1. 点击"新建流程"按钮
2. 在流程设计器中添加节点和连线
3. 点击"保存流程"按钮
4. 填写流程信息:
   - 流程名称: 如 "请假审批流程"
   - 流程Key: 如 "leave-approval"
   - 流程描述: 可选
5. 点击确定保存

### 3. 查看流程列表

- 返回首页可以看到刚创建的流程
- 点击"编辑"可以修改流程
- 点击"版本历史"可以查看历史版本

### 4. 创建新版本

1. 点击"编辑"进入流程设计器
2. 修改流程内容
3. 点击"保存为新版本"按钮
4. 确认保存，系统将创建 V2 版本

### 5. 查看版本历史

1. 在流程列表中点击"版本历史"
2. 可以看到该流程的所有版本
3. 每个版本都可以单独编辑

### 6. 启动流程实例 (可选)

1. 切换到"流程实例"页面
2. 点击"启动工作流"
3. 输入流程定义Key（如 "leave-approval"）
4. 点击确定启动

## API 测试

### 使用 Swagger 测试

访问 `http://localhost:5000/swagger` 可以测试所有后端 API。

### 主要 API 端点

#### 流程定义 API

- `POST /api/processdefinitions` - 创建流程定义
- `GET /api/processdefinitions` - 分页查询流程定义
- `GET /api/processdefinitions/{id}` - 获取单个流程定义
- `PUT /api/processdefinitions/{id}?createNewVersion=true/false` - 更新流程定义
- `DELETE /api/processdefinitions/{id}` - 删除流程定义
- `GET /api/processdefinitions/by-key/{key}/latest` - 获取最新版本
- `GET /api/processdefinitions/by-key/{key}/versions` - 获取版本历史

#### 工作流 API

- `POST /api/workflow/start` - 启动工作流
- `GET /api/workflow/instance/{id}` - 获取工作流实例
- `POST /api/workflow/suspend/{id}` - 暂停工作流
- `POST /api/workflow/resume/{id}` - 恢复工作流
- `POST /api/workflow/terminate/{id}` - 终止工作流

## MVP 阶段说明

### 临时配置

1. **认证已禁用**: 当前版本已临时禁用 JWT 认证
2. **测试租户**: 使用固定的测试租户ID `00000000-0000-0000-0000-000000000001`
3. **CORS**: 允许所有来源（生产环境需要限制）

### 功能范围

当前 MVP 支持:

✅ 流程定义的增删改查
✅ 多版本管理（手动创建新版本）
✅ 版本历史查看
✅ 流程设计器（基于 React Flow）
✅ 流程实例管理（启动、暂停、恢复、终止）

暂不支持:

❌ 用户认证和授权
❌ 复杂的审批逻辑（需要在 Engine 层实现）
❌ 任务分配策略
❌ 流程实例详情查看
❌ 流程统计和报表

## 故障排查

### 后端无法启动

1. 检查 .NET SDK 版本: `dotnet --version`
2. 检查端口 5000 是否被占用
3. 检查数据库文件是否存在

### 前端无法启动

1. 检查 Node.js 版本: `node --version`
2. 确保依赖已安装: `npm install`
3. 检查端口 5173 是否被占用

### API 调用失败

1. 检查后端是否正常启动
2. 检查浏览器控制台的网络请求
3. 检查 CORS 配置
4. 确认测试租户已初始化

### 数据库问题

1. 删除 `workflow.db` 文件
2. 重新运行迁移: `dotnet ef database update`
3. 重新初始化测试租户

## 下一步开发建议

1. 实现用户认证和授权
2. 完善工作流引擎逻辑
3. 实现任务分配策略
4. 添加流程实例详情页
5. 实现审批任务处理界面
6. 添加流程统计和监控
7. 完善错误处理和日志
8. 添加单元测试和集成测试

