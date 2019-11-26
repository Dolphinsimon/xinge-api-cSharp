# xinge-api-C#
[![NuGet version (XingeApp)](https://img.shields.io/nuget/v/XingeApp.svg?style=flat-square)](https://www.nuget.org/packages/XingeApp/)
## 概述
[信鸽](http://xg.qq.com) 是腾讯云提供的一款支持**百亿级**消息的移动App推送平台，开发者可以调用C# SDK访问信鸽推送服务。

## 引用SDK
方法一：请到[信鸽官网](http://xg.qq.com/xg/ctr_index/download)下载最新版本的包，使用时添加XingeApp.dll依赖即可。

方法二：克隆本git仓库，此项目是使用Visual Studio Code for Mac IDE开发的，其中SDK文件夹下是封装的源码，SDK.Test文件夹下是封装的源码的UT.

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