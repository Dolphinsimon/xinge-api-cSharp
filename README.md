# xinge-api-C#
[![NuGet version (XingeApp)](https://img.shields.io/nuget/v/XingeApp.svg?style=flat-square)](https://www.nuget.org/packages/XingeApp/)
## 概述
[信鸽](http://xg.qq.com) 是腾讯云提供的一款支持**百亿级**消息的移动App推送平台，开发者可以调用C# SDK访问信鸽推送服务。

## 引用SDK
直接安装nuget包：[XingeApp](https://www.nuget.org/packages/XingeApp)

从1.1.1版本开始，HTTP请求开始改用IHttpClientFactory实现高效的HTTP连接管理，并且基于Task实现了异步方法。

在ASP.NETCore项目内可通过依赖注入方式使用：

1.在Startup.cs的ConfigureServices内添加 
```C#
services.AddHttpClient();
services.AddSingleton<IXingeApp, XingeApp.XingeApp>();
```
2.在你的使用的Service或Controller的构造函数内注入IXingeApp接口；

此外对于部分未传入AppId/AccessKey/SecretKey的方法，实现类内部通过appsettings.json读取相关配置，配置结构如下：
```json
"XingeApp": {
    "AppId": "000000",
    "AccessKey": "00000000000",
    "SecretKey": "0000000000000000000000"
  }
```
## 接口说明
信鸽提供的主要推送和查询接口包括3种

### 创建推送任务
- pushSingleDevice 推送消息给单个设备
- pushSingleAccount 推送消息给单个账号
- pushAccountList 推送消息给多个账号
- pushAllDevice 推送消息给单个app的所有设备
- pushTags 推送消息给tags指定的设备
- createMultipush创建大批量推送消息(C# SDK 1.2.0+ 不在支持)
- pushAccountListMultiple推送消息给大批量账号(可多次)(C# SDK 1.2.0+ 不在支持)
- pushDeviceListMultiple推送消息给大批量设备(可多次)(C# SDK 1.2.0+ 不在支持)

### 异步查询推送状态
- queryPushStatus查询群发消息发送状态
- cancelTimingPush取消尚未推送的定时消息

### 查询/更改账户和标签
- queryTags 查询应用的tags
- BatchSetTag 批量为token设置标签
- BatchDelTag 批量为token删除标签
- queryTokenTags 查询token的tags
- queryTagTokenNum 查询tag下token的数目
- deleteTokenOfAccount 删除account绑定的token