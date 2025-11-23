# 第三方登录配置指南

本文档详细说明如何配置各种第三方OAuth登录。

## 1. 微信开放平台配置

### 1.1 创建应用

1. 访问 [微信开放平台](https://open.weixin.qq.com/)
2. 登录并进入管理中心
3. 创建网站应用或移动应用
4. 填写应用信息并提交审核

### 1.2 获取配置参数

审核通过后获取：
- **AppID**: 应用唯一标识
- **AppSecret**: 应用密钥

### 1.3 配置回调地址

在应用设置中配置授权回调域名：
```
https://yourdomain.com/api/auth/oauth/wechat/callback
```

### 1.4 更新appsettings.json

```json
"Authentication": {
  "WeChat": {
    "ClientId": "你的微信AppID",
    "AppId": "你的微信AppID",
    "AppSecret": "你的微信AppSecret"
  }
}
```

---

## 2. QQ互联配置

### 2.1 创建应用

1. 访问 [QQ互联平台](https://connect.qq.com/)
2. 登录并创建应用
3. 填写应用信息

### 2.2 获取配置参数

- **AppID**: 应用ID
- **AppKey**: 应用密钥

### 2.3 配置回调地址

```
https://yourdomain.com/api/auth/oauth/qq/callback
```

### 2.4 更新appsettings.json

```json
"Authentication": {
  "QQ": {
    "ClientId": "你的QQ AppID",
    "AppId": "你的QQ AppID",
    "AppKey": "你的QQ AppKey"
  }
}
```

---

## 3. 支付宝开放平台配置

### 3.1 创建应用

1. 访问 [支付宝开放平台](https://open.alipay.com/)
2. 登录并创建应用
3. 配置应用信息

### 3.2 生成密钥

使用支付宝密钥生成工具生成RSA2密钥对：
- **应用私钥**: 保存到本地，用于签名
- **应用公钥**: 上传到支付宝平台

### 3.3 获取配置参数

- **AppID**: 应用ID
- **应用私钥**: 本地保存的私钥

### 3.4 配置回调地址

```
https://yourdomain.com/api/auth/oauth/alipay/callback
```

### 3.5 更新appsettings.json

```json
"Authentication": {
  "Alipay": {
    "AppId": "你的支付宝AppID",
    "PrivateKey": "你的应用私钥(Base64格式)"
  }
}
```

---

## 4. Apple Sign In 配置

### 4.1 创建应用

1. 访问 [Apple Developer](https://developer.apple.com/)
2. 登录并进入Certificates, Identifiers & Profiles
3. 创建App ID并启用Sign In with Apple
4. 创建Service ID

### 4.2 获取配置参数

- **ClientID**: Service ID
- **TeamID**: 开发者团队ID
- **KeyID**: 密钥ID
- **私钥文件**: 下载的.p8文件内容

### 4.3 配置回调地址

在Service ID中配置：
```
https://yourdomain.com/api/auth/oauth/apple/callback
```

### 4.4 更新appsettings.json

```json
"Authentication": {
  "Apple": {
    "ClientId": "你的Service ID",
    "TeamId": "你的Team ID",
    "KeyId": "你的Key ID",
    "PrivateKey": "你的私钥文件内容"
  }
}
```

---

## 5. 测试配置

### 5.1 启动应用

```bash
cd src/WorkFlowCore.API
dotnet run
```

### 5.2 测试授权流程

访问以下URL获取授权链接：
```
GET /api/auth/oauth/wechat/authorize?redirectUrl=http://localhost:3000/auth/callback
GET /api/auth/oauth/qq/authorize?redirectUrl=http://localhost:3000/auth/callback
GET /api/auth/oauth/alipay/authorize?redirectUrl=http://localhost:3000/auth/callback
GET /api/auth/oauth/apple/authorize?redirectUrl=http://localhost:3000/auth/callback
```

### 5.3 测试回调处理

用户授权后会回调到：
```
GET /api/auth/oauth/{provider}/callback?code={code}&state={state}
```

---

## 6. 常见问题

### 6.1 redirect_uri参数错误

**原因**: 回调地址未在第三方平台配置  
**解决**: 确保回调地址与平台配置完全一致(包括协议、域名、路径)

### 6.2 签名错误

**原因**: 密钥配置错误  
**解决**: 检查AppSecret/AppKey/PrivateKey是否正确

### 6.3 无法获取用户信息

**原因**: scope权限不足  
**解决**: 在授权URL中添加必要的scope参数

---

## 7. 安全建议

1. **密钥管理**: 不要将密钥提交到版本控制系统
2. **HTTPS**: 生产环境必须使用HTTPS
3. **State参数**: 使用随机state防止CSRF攻击
4. **Token刷新**: 定期刷新AccessToken
5. **权限最小化**: 只申请必要的用户信息权限

---

## 8. 开发环境Mock模式

如果还未申请第三方应用，可以使用Mock模式测试：

在`appsettings.Development.json`中留空配置：
```json
"Authentication": {
  "WeChat": {
    "ClientId": "",
    "AppId": "",
    "AppSecret": ""
  }
}
```

系统会自动使用Mock模式，生成测试用的OAuth URL和回调数据。

---

## 9. 相关链接

- [微信开放平台](https://open.weixin.qq.com/)
- [QQ互联平台](https://connect.qq.com/)
- [支付宝开放平台](https://open.alipay.com/)
- [Apple Developer](https://developer.apple.com/)
- [OAuth 2.0 RFC](https://tools.ietf.org/html/rfc6749)

