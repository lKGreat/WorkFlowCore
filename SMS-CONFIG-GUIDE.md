# 短信服务配置指南

本文档详细说明如何配置阿里云和腾讯云短信服务。

## 1. 阿里云短信服务

### 1.1 开通服务

1. 访问 [阿里云短信服务](https://www.aliyun.com/product/sms)
2. 登录并开通短信服务
3. 完成实名认证

### 1.2 创建签名

1. 进入短信服务控制台
2. 国内消息 → 签名管理 → 添加签名
3. 填写签名信息并提交审核

### 1.3 创建模板

创建以下模板：

**登录验证码模板**:
```
验证码${code}，您正在登录WorkFlowCore系统，5分钟内有效。
```

**注册验证码模板**:
```
验证码${code}，您正在注册WorkFlowCore账号，5分钟内有效。
```

**重置密码模板**:
```
验证码${code}，您正在重置密码，5分钟内有效。
```

**绑定手机模板**:
```
验证码${code}，您正在绑定手机号，5分钟内有效。
```

### 1.4 获取配置参数

1. 进入AccessKey管理
2. 创建AccessKey(如已有则直接使用)
3. 获取：
   - **AccessKeyId**: 访问密钥ID
   - **AccessKeySecret**: 访问密钥密钥

### 1.5 更新appsettings.json

```json
{
  "Sms": {
    "Provider": "Aliyun",
    "Aliyun": {
      "AccessKeyId": "你的AccessKeyId",
      "AccessKeySecret": "你的AccessKeySecret",
      "SignName": "你的签名名称"
    },
    "Templates": {
      "Login": "SMS_123456789",
      "Register": "SMS_987654321",
      "ResetPassword": "SMS_111111111",
      "BindPhone": "SMS_222222222"
    }
  }
}
```

### 1.6 集成SDK (可选)

如需使用官方SDK，安装NuGet包：
```bash
dotnet add package Aliyun.Acs.Core
dotnet add package Aliyun.Acs.Dysmsapi
```

---

## 2. 腾讯云短信服务

### 2.1 开通服务

1. 访问 [腾讯云短信](https://cloud.tencent.com/product/sms)
2. 登录并开通短信服务
3. 完成实名认证

### 2.2 创建应用

1. 进入短信控制台
2. 应用管理 → 创建应用
3. 记录应用的SdkAppId

### 2.3 创建签名

1. 国内短信 → 签名管理 → 创建签名
2. 填写签名信息并提交审核

### 2.4 创建模板

创建以下模板(参数顺序为{1}):

**登录验证码模板**:
```
{1}为您的登录验证码，请于5分钟内填写。如非本人操作，请忽略本短信。
```

**注册验证码模板**:
```
{1}为您的注册验证码，请于5分钟内填写。如非本人操作，请忽略本短信。
```

### 2.5 获取配置参数

1. 进入访问管理 → API密钥管理
2. 新建密钥或使用现有密钥
3. 获取：
   - **SecretId**: 密钥ID
   - **SecretKey**: 密钥Key
   - **SdkAppId**: 应用ID

### 2.6 更新appsettings.json

```json
{
  "Sms": {
    "Provider": "Tencent",
    "Tencent": {
      "SecretId": "你的SecretId",
      "SecretKey": "你的SecretKey",
      "SdkAppId": "你的SdkAppId",
      "SignName": "你的签名名称"
    },
    "Templates": {
      "Login": "123456",
      "Register": "654321",
      "ResetPassword": "111111",
      "BindPhone": "222222"
    }
  }
}
```

### 2.7 集成SDK (可选)

如需使用官方SDK，安装NuGet包：
```bash
dotnet add package TencentCloudSDK
```

---

## 3. Mock模式(开发测试)

如果还未申请短信服务，系统会自动使用Mock模式：

### 3.1 配置

在`appsettings.Development.json`中留空配置：
```json
{
  "Sms": {
    "Provider": "Aliyun",
    "Aliyun": {
      "AccessKeyId": "",
      "AccessKeySecret": "",
      "SignName": "WorkFlowCore"
    }
  }
}
```

### 3.2 行为

- 不会真实发送短信
- 验证码会输出到日志中
- 仍然会进行验证码缓存和校验

### 3.3 查看验证码

启动应用后查看控制台日志：
```
[Mock阿里云短信] 手机号: 13800138000, 验证码: 123456, 模板: SMS_LOGIN
```

---

## 4. 切换短信提供商

通过配置`Sms:Provider`切换：

```json
{
  "Sms": {
    "Provider": "Tencent"  // 可选: Aliyun, Tencent
  }
}
```

系统会自动使用对应的提供商发送短信。

---

## 5. 费用说明

### 5.1 阿里云

- 按条计费，0.045元/条起
- 首次开通赠送100条测试短信
- [详细价格](https://www.aliyun.com/price/product#/sms/detail)

### 5.2 腾讯云

- 按条计费，0.045元/条起
- 新用户赠送100条免费短信
- [详细价格](https://cloud.tencent.com/document/product/382/9556)

---

## 6. 常见问题

### 6.1 发送失败

**原因**:
- 签名或模板未审核通过
- AccessKey权限不足
- 短信余额不足
- 手机号格式错误

**解决**:
- 检查签名和模板状态
- 确认AccessKey有短信发送权限
- 充值短信套餐包
- 验证手机号格式(+86开头)

### 6.2 验证码收不到

**原因**:
- 手机号被运营商拦截
- 发送频率过高
- 手机号在黑名单中

**解决**:
- 使用不同手机号测试
- 设置发送频率限制(60秒/次)
- 联系客服移出黑名单

### 6.3 模板变量不匹配

**原因**: 模板参数名称或顺序不一致

**解决**: 确保代码中的参数与模板定义完全一致

---

## 7. 安全建议

1. **密钥安全**: 不要将密钥提交到Git仓库
2. **频率限制**: 限制同一手机号发送频率(如60秒/次)
3. **IP白名单**: 在云平台配置API调用IP白名单
4. **监控告警**: 配置短信发送量异常告警
5. **验证码过期**: 验证码5分钟过期，验证后立即失效

---

## 8. 使用示例

### 8.1 发送登录验证码

```bash
POST /api/auth/sms/send
Content-Type: application/json

{
  "phoneNumber": "13800138000",
  "type": 0
}
```

### 8.2 手机号登录

```bash
POST /api/auth/phone-login
Content-Type: application/json

{
  "phoneNumber": "13800138000",
  "code": "123456"
}
```

---

## 9. 相关链接

- [阿里云短信服务](https://www.aliyun.com/product/sms)
- [阿里云短信API文档](https://help.aliyun.com/document_detail/101414.html)
- [腾讯云短信服务](https://cloud.tencent.com/product/sms)
- [腾讯云短信API文档](https://cloud.tencent.com/document/product/382/38764)

