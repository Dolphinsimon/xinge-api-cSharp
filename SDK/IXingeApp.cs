using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XingeApp
{
    public interface IXingeApp
    {
        /// <summery> 推送普通消息给指定的设备,限Android系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "token"> 接收消息的设备标识 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushTokenAndroid(string appID, long accessID, string secretKey, string title,
            string content, string token);

        /// <summery>//推送普通消息给指定的设备,限iOS系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "token"> 接收消息的设备标识 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushTokeniOS(string appID, long accessID, string secretKey, string content, string token,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery>推送普通消息给指定的账号,限Android系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAccountAndroid(string appID, long accessID, string secretKey, string title, string content,
            string account);


        /// <summery>//推送普通消息给指定的账号,限iOS系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAccountiOS(string appID, long accessID, string secretKey, string content, string account,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery>推送普通消息给全部的设备,限Android系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAllAndroid(string appID, long accessID, string secretKey, string title, string content);

        /// <summery>推送普通消息给全部的设备,限iOS系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAlliOS(string appID, long accessID, string secretKey, string content,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery>//推送普通消息给绑定标签的设备,限Android系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "tag"> 接收设备标识绑定的标签 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushTagAndroid(string appID, long accessID, string secretKey, string title, string content,
            string tag);

        /// <summery>推送普通消息给绑定标签的设备,限iOS系统使用
        /// <param name = "appID"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessID"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "tag"> 接收设备标识绑定的标签 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushTagiOS(string appID, long accessID, string secretKey, string content, string tag,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery> 推送消息给指定的设备, 限Android系统使用
        /// <param name = "deviceToken"> 接收消息的设备标识 </param>
        /// <param name = "message"> Android消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushSingleDevice(string deviceToken, Message message);

        /// <summery> 推送消息给多个设备, 限 Android 系统使用
        /// <param name = "deviceTokens"> 接收消息的设备标识列表 </param>
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushMultipleDevices(List<string> deviceTokens, Message message);

        /// <summery> 推送消息给指定设备, 限 iOS 系统使用
        /// <param name = "deviceToken"> 接收消息的设备标识 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushSingleDevice(string deviceToken, MessageiOS message,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery> 推送消息给多个设备, 限 iOS 系统使用
        /// <param name = "deviceTokens"> 接收消息的设备标识列表 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushMultipleDevices(List<string> deviceTokens, MessageiOS message,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery> 推送消息给绑定账号的设备, 限 Android 系统使用
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushSingleAccount(string account, Message message);

        /// <summery> 推送消息给绑定账号的设备, 限 iOS 系统使用
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushSingleAccount(string account, MessageiOS message, XingeApp.PushEnvironmentofiOS environment);

        /// <summery> 推送消息给绑定账号的设备, 限 Android 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的账号列表 </param>
        /// <param name = "message"> Android 消息结构体,注意：第一次推送时，message中的pushID是填写0，若需要再次推送同样的文本，需要根据返回的push_id填写 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAccountList(List<string> accountList, Message message);

        /// <summery> 推送消息给绑定账号的设备, 限 iOS 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的账号列表 </param>
        /// <param name = "message"> iOS 消息结构体，注意：第一次推送时，message中的pushID是填写0，若需要再次推送同样的文本，需要根据返回的push_id填写 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAccountList(List<string> accountList, MessageiOS message,
            XingeApp.PushEnvironmentofiOS environment);

        /// <summery> 推送消息给全部设备, 限 Android 系统使用
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAllDevice(Message message);

        /// <summery> 推送消息给全部设备, 限 iOS 系统使用
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushAllDevice(MessageiOS message, XingeApp.PushEnvironmentofiOS environment);

        /// <summery> 推送消息给绑定标签的设备, 限 Android 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的标签列表 </param>
        /// <param name = "tagOp"> 标签集合需要进行的逻辑集合运算标识 </param>
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task< string> PushTags(List<string> tagList, string tagOp, Message message);

        /// <summery> 推送消息给绑定标签的设备, 限 iOS 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的标签列表 </param>
        /// <param name = "tagOp"> 标签集合需要进行的逻辑集合运算标识 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        Task<string> PushTags(List<string> tagList, string tagOp, MessageiOS message,
            XingeApp.PushEnvironmentofiOS environment);

        //查询群发消息状态
        Task<string> QueryPushStatus(List<string> pushIdList);

        /**
        * 查询应用当前所有的tags
        *
        * @param start 从哪个index开始
        * @param limit 限制结果数量，最多取多少个tag
        */
        Task<string> QueryTags(int start, int limit);

        /**
        * 查询带有指定tag的设备数量
        *
        * @param tag 指定的标签
        */
        Task<string> QueryTagTokenNum(string tag);

        /**
        * 查询设备下所有的tag
        *
        * @param deviceToken 目标设备token
        */
        Task<string> QueryTokenTags(string deviceToken);

        /**
        * 取消尚未推送的定时任务
        *
        * @param pushId 各类推送任务返回的push_id
        */
        Task<string> CancelTimingPush(string pushId);

        //批量为token设置标签
        Task<string> BatchSetTag(List<TagTokenPair> tagTokenPairs);

        //批量为token删除标签
        Task<string> BatchDelTag(List<TagTokenPair> tagTokenPairs);

        /**
        * 删除指定账号和token的绑定关系（token仍然有效）
        *
        * @param account 目标账号
        * @param deviceToken 目标设备token
        */
        Task<string> DeleteTokenOfAccount(string account, string deviceToken);

    }
}