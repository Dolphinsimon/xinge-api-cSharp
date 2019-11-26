using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace XingeApp
{
    
    /// <summery> 常量均为 API 硬编码，不可修改
    /// </summery>
    static class XGPushConstants 
    {
        public static string OrdinaryMessage = "notify";
        public static string SilentMessage = "message";
    }
    public class XingeApp:IXingeApp
    {
        public static string RestapiPushsingledevice = "http://openapi.xg.qq.com/v2/push/single_device";
        public static string RestapiPushsingleaccount = "http://openapi.xg.qq.com/v2/push/single_account";
        public static string RestapiPushaccountlist = "http://openapi.xg.qq.com/v2/push/account_list";
        public static string RestapiPushalldevice = "http://openapi.xg.qq.com/v2/push/all_device";
        public static string RestapiPushtags = "http://openapi.xg.qq.com/v2/push/tags_device";
        public static string RestapiCreatemultipush = "http://openapi.xg.qq.com/v2/push/create_multipush";
        public static string RestapiPushaccountlistmultiple = "http://openapi.xg.qq.com/v2/push/account_list_multiple";
        public static string RestapiPushdevicelistmultiple = "http://openapi.xg.qq.com/v2/push/device_list_multiple";
        public static string RestapiQuerypushstatus = "http://openapi.xg.qq.com/v2/push/get_msg_status";
        public static string RestapiQuerydevicecount = "http://openapi.xg.qq.com/v2/application/get_app_device_num";
        public static string RestapiQuerytags = "http://openapi.xg.qq.com/v2/tags/query_app_tags";
        public static string RestapiCanceltimingpush = "http://openapi.xg.qq.com/v2/push/cancel_timing_task";
        public static string RestapiBatchsettag = "http://openapi.xg.qq.com/v2/tags/batch_set";
        public static string RestapiBatchdeltag = "http://openapi.xg.qq.com/v2/tags/batch_del";
        public static string RestapiQuerytokentags = "http://openapi.xg.qq.com/v2/tags/query_token_tags";
        public static string RestapiQuerytagtokennum = "http://openapi.xg.qq.com/v2/tags/query_tag_token_num";
        public static string RestapiQueryinfooftoken = "http://openapi.xg.qq.com/v2/application/get_app_token_info";
        public static string RestapiQuerytokensofaccount = "http://openapi.xg.qq.com/v2/application/get_app_account_tokens";
        public static string RestapiDeletetokenofaccount = "http://openapi.xg.qq.com/v2/application/del_app_account_tokens";
        public static string RestapiDeletealltokensofaccount = "http://openapi.xg.qq.com/v2/application/del_app_account_all_tokens";

        private static string _xgPushServierHost = "https://openapi.xg.qq.com";
        private static string _xgPushAppPath = "/v3/push/app";
        ///<summery> 此枚举只有在iOS平台上使用，对应于App的所处的环境
        ///</summery>
        public enum PushEnvironmentofiOS {
            Product = 1,
            Develop = 2
        }

        public static long IOsMinId = 2200000000L;

        private long m_xgPushAppAccessKey;
        private string m_xgPushAppSecretKey;
        //V3版本新增APP ID 字段，用来标识应用的ID
        private string m_xgPushAppId;

        private HttpClient m_client;

        /// <summery>对于V3版本的接口，信鸽服务器要求必须添加应用标识，即APPID，可以在前端网页的应用配置中查询
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// </summery>
        public XingeApp(string appId, long accessId, string secretKey,IHttpClientFactory clientFactory)
        {
            this.m_xgPushAppId        = appId;
            this.m_xgPushAppAccessKey = accessId;
            this.m_xgPushAppSecretKey = secretKey;
            m_client = clientFactory.CreateClient();
            m_client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic ", Base64AuthStringOfXgPush());
        }


        protected string StringToMd5(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                builder.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return builder.ToString();
        }

        protected Boolean IsValidToken(string token)
        {
            if (this.m_xgPushAppAccessKey > IOsMinId)
                return token.Length == 64;
            else
                return (token.Length == 40 || token.Length == 64);
        }

        protected Boolean IsValidMessageType(Message msg)
        {
            if (this.m_xgPushAppAccessKey < IOsMinId)
                return true;
            else
                return false;
        }

        protected Boolean IsValidMessageType(MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (this.m_xgPushAppAccessKey >= IOsMinId && (environment == PushEnvironmentofiOS.Product || environment == PushEnvironmentofiOS.Develop))
                return true;
            else
                return false;
        }

        protected Dictionary<string, object> InitParams()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            // DateTime Epoch = new DateTime(1970, 1, 1);
            // long timestamp = (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            return param;
        }

        protected JArray ToJArray(List<string> strList)
        {
            JArray ja = new JArray();
            foreach (string str in strList)
            {
                ja.Add(new JValue(str));
            }
            return ja;
        }

        private string GenerateSign(string method, string url, Dictionary<string, object> param)
        {
            string paramStr = "";
            string md5Str = "";
            var dicSort = from objDic in param orderby objDic.Key select objDic;
            foreach (KeyValuePair<string, object> kvp in dicSort)
            {
                paramStr += kvp.Key + "=" + kvp.Value.ToString();
            }
            Uri u = new Uri(url);
            md5Str = method + u.Host + u.AbsolutePath + paramStr + this.m_xgPushAppSecretKey;
            md5Str = HttpUtility.UrlDecode(md5Str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(md5Str));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                
                builder.Append(encryptedBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private string Base64AuthStringOfXgPush() {
            byte[] bytes = Encoding.ASCII.GetBytes(this.m_xgPushAppId + ":" + this.m_xgPushAppSecretKey);
            return Convert.ToBase64String(bytes);
        }

        private async Task<string> CallRestful(string url, Dictionary<string, object> param)
        {
            string temp = "";
            string sign = GenerateSign("GET", url, param);
            if (sign.Length == 0)
                return "generate sign error";
            param.Add("sign", sign);
            foreach (KeyValuePair<string, object> kvp in param)
            {
                temp += kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value.ToString()) + "&";
            }

            try
            {
                temp = url + "?" + temp.Remove(temp.Length - 1, 1);
                var request=new HttpRequestMessage(HttpMethod.Get, temp);
                request.Content.Headers.ContentType=new MediaTypeHeaderValue("application/json");
                var response = await m_client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                throw new HttpRequestException(response.StatusCode.ToString());
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        protected async Task<string> RequestXgServerV3(string host, string path, Dictionary<string, object> param) 
        {

            string url = host + path;
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(
                JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");

            var response = await m_client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            throw new HttpRequestException(response.StatusCode.ToString());
        }

        //==========================================简易接口api=====================================================

        /// <summery> 推送普通消息给指定的设备,限Android系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "token"> 接收消息的设备标识 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task< string> PushTokenAndroid(string appId, long accessId, string secretKey, string title, string content, string token)
        {
            Message message = new Message();
            message.setType(XGPushConstants.OrdinaryMessage);
            message.setTitle(title);
            message.setContent(content);

            string ret = await PushSingleDevice(token, message);
            return (ret);
        }

        
        /// <summery>//推送普通消息给指定的设备,限iOS系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "token"> 接收消息的设备标识 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushTokeniOS(string appId, long accessId, string secretKey, string content, string token, PushEnvironmentofiOS environment)
        {
            MessageiOS message = new MessageiOS();
            message.setAlert(content);

            string ret =await PushSingleDevice(token, message, environment);
            return (ret);
        }

        /// <summery>推送普通消息给指定的账号,限Android系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAccountAndroid(string appId, long accessId, string secretKey, string title, string content, string account)
        {
            Message message = new Message();
            message.setType(XGPushConstants.OrdinaryMessage);
            message.setTitle(title);
            message.setContent(content);
            string ret = await PushSingleAccount(account, message);
            return (ret);
        }

        
        /// <summery>//推送普通消息给指定的账号,限iOS系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAccountiOS(string appId, long accessId, string secretKey, string content, string account, PushEnvironmentofiOS environment)
        {
            MessageiOS message = new MessageiOS();
            message.setAlert(content);

            string ret = await PushSingleAccount(account, message, environment);
            return (ret);
        }

        /// <summery>推送普通消息给全部的设备,限Android系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAllAndroid(string appId, long accessId, string secretKey, string title, string content)
        {
            Message message = new Message();
            message.setType(XGPushConstants.OrdinaryMessage);
            message.setTitle(title);
            message.setContent(content);
            string ret = await PushAllDevice(message);
            return (ret);
        }

        /// <summery>推送普通消息给全部的设备,限iOS系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAlliOS(string appId, long accessId, string secretKey, string content, PushEnvironmentofiOS environment)
        {
            MessageiOS message = new MessageiOS();
            message.setAlert(content);

            string ret = await PushAllDevice(message, environment);
            return (ret);
        }

        
        /// <summery>//推送普通消息给绑定标签的设备,限Android系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "title"> 消息标题 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "tag"> 接收设备标识绑定的标签 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushTagAndroid(string appId, long accessId, string secretKey, string title, string content, string tag)
        {
            Message message = new Message();
            message.setType(XGPushConstants.OrdinaryMessage);
            message.setTitle(title);
            message.setContent(content);

            List<string> tagList = new List<string>();
            tagList.Add(tag);
            string ret = await PushTags(tagList, "OR", message);
            return (ret);
        }

        /// <summery>推送普通消息给绑定标签的设备,限iOS系统使用
        /// <param name = "appId"> V3版本接口中对应用的标识 </param>
        /// <param name = "accessId"> V2版本接口中系统自动生成的标识 </param>
        /// <param name = "secretKey"> 用于API调用的秘钥 </param>
        /// <param name = "content"> 消息内容 </param>
        /// <param name = "tag"> 接收设备标识绑定的标签 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushTagiOS(string appId, long accessId, string secretKey, string content, string tag, PushEnvironmentofiOS environment)
        {
            MessageiOS message = new MessageiOS();
            message.setAlert(content);

            List<string> tagList = new List<string>();
            tagList.Add(tag);
            string ret = await PushTags(tagList, "OR", message, environment);
            return (ret);
        }

        // ====================================详细接口api==========================================================

        /// <summery> 推送消息给指定的设备, 限Android系统使用
        /// <param name = "deviceToken"> 接收消息的设备标识 </param>
        /// <param name = "message"> Android消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushSingleDevice(string deviceToken, Message message)
        {
            if (!IsValidMessageType(message))
                return "message type error!";
            if (!message.isValid())
                return "message is invalid!";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "token");
            param.Add("platform", "android");

            List <string> tokenList = new List<string>();
            tokenList.Add(deviceToken);

            param.Add("token_list", ToJArray(tokenList));
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("multi_pkg", message.getMultiPkg());
            // param.Add("device_token", devicetoken);
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            // string ret = callRestful(XingeApp.RESTAPI_PUSHSINGLEDEVICE, param);
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给多个设备, 限 Android 系统使用
        /// <param name = "deviceTokens"> 接收消息的设备标识列表 </param>
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushMultipleDevices(List<string> deviceTokens, Message message)
        {
            if (!IsValidMessageType(message))
                return "message type error!";
            if (!message.isValid())
                return "message is invalid!";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "token_list");
            param.Add("platform", "android");
            param.Add("token_list", ToJArray(deviceTokens));
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("multi_pkg", message.getMultiPkg());
            // param.Add("device_token", devicetoken);
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            // string ret = callRestful(XingeApp.RESTAPI_PUSHSINGLEDEVICE, param);
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给指定设备, 限 iOS 系统使用
        /// <param name = "deviceToken"> 接收消息的设备标识 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushSingleDevice(string deviceToken, MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (!IsValidMessageType(message, environment))
            {
                return "{'ret_code':-1,'err_msg':'message type or environment error!'}";
            }
            if (!message.isValid())
            {
                return "{'ret_code':-1,'err_msg':'message invalid!'}";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "token");
            param.Add("platform", "ios");

            List <string> tokenList = new List<string>();
            tokenList.Add(deviceToken);
            param.Add("token_list", ToJArray(tokenList));

            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            // param.Add("device_token", deviceToken);
            param.Add("message_type", "notify");
            
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            param.Add("environment", environment);
            if (message.getLoopInterval() > 0 && message.getLoopTimes() > 0)
            {
                param.Add("loop_interval", message.getLoopInterval());
                param.Add("loop_times", message.getLoopTimes());
            }
            System.Console.WriteLine(param);
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给多个设备, 限 iOS 系统使用
        /// <param name = "deviceTokens"> 接收消息的设备标识列表 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushMultipleDevices(List<string> deviceTokens, MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (!IsValidMessageType(message, environment))
            {
                return "{'ret_code':-1,'err_msg':'message type or environment error!'}";
            }
            if (!message.isValid())
            {
                return "{'ret_code':-1,'err_msg':'message invalid!'}";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "token_list");
            param.Add("platform", "ios");
            param.Add("token_list", ToJArray(deviceTokens));
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            // param.Add("device_token", deviceToken);
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            param.Add("environment", environment);
            if (message.getLoopInterval() > 0 && message.getLoopTimes() > 0)
            {
                param.Add("loop_interval", message.getLoopInterval());
                param.Add("loop_times", message.getLoopTimes());
            }
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给绑定账号的设备, 限 Android 系统使用
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushSingleAccount(string account, Message message)
        {
            if (!IsValidMessageType(message))
                return "message type error!";
            if (!message.isValid())
                return "message is invalid!";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "account");
            param.Add("platform", "android");

            List <string> accountList = new List<string>();
            accountList.Add(account);

            param.Add("account_list", ToJArray(accountList));
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("multi_pkg", message.getMultiPkg());
            // param.Add("account", account);
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给绑定账号的设备, 限 iOS 系统使用
        /// <param name = "account"> 接收设备标识绑定的账号 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushSingleAccount(string account, MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (!IsValidMessageType(message, environment))
            {
                return "{'ret_code':-1,'err_msg':'message type or environment error!'}";
            }
            if (!message.isValid())
            {
                return "{'ret_code':-1,'err_msg':'message invalid!'}";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "account");
            param.Add("platform", "ios");

            List <string> accountList = new List<string>();
            accountList.Add(account);

            param.Add("account_list", ToJArray(accountList));
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            // param.Add("account", account);
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            param.Add("environment", environment);
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给绑定账号的设备, 限 Android 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的账号列表 </param>
        /// <param name = "message"> Android 消息结构体,注意：第一次推送时，message中的pushID是填写0，若需要再次推送同样的文本，需要根据返回的push_id填写 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAccountList(List<string> accountList, Message message)
        {
            if (!IsValidMessageType(message))
                return "message type error!";
            if (!message.isValid())
                return "message is invalid!";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "account_list");
            param.Add("platform", "android");
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("multi_pkg", message.getMultiPkg());
            param.Add("account_list", ToJArray(accountList));
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("push_id", message.getPushID().ToString());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给绑定账号的设备, 限 iOS 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的账号列表 </param>
        /// <param name = "message"> iOS 消息结构体，注意：第一次推送时，message中的pushID是填写0，若需要再次推送同样的文本，需要根据返回的push_id填写 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAccountList(List<string> accountList, MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (!IsValidMessageType(message, environment))
            {
                return "{'ret_code':-1,'err_msg':'message type or environment error!'}";
            }
            if (!message.isValid())
            {
                return "{'ret_code':-1,'err_msg':'message invalid!'}";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "account_list");
            param.Add("platform", "ios");
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("account_list", ToJArray(accountList));
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("push_id", message.getPushID().ToString());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            param.Add("environment", environment);
            string ret =await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给全部设备, 限 Android 系统使用
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAllDevice(Message message)
        {
            if (!IsValidMessageType(message))
                return "message type error!";
            if (!message.isValid())
                return "message is invalid!";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "all");
            param.Add("platform", "android");
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("multi_pkg", message.getMultiPkg());
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            if (message.getLoopInterval() > 0 && message.getLoopTimes() > 0)
            {
                param.Add("loop_interval", message.getLoopInterval());
                param.Add("loop_times", message.getLoopTimes());
            }
            string ret =await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给全部设备, 限 iOS 系统使用
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushAllDevice(MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (!IsValidMessageType(message, environment))
            {
                return "{'ret_code':-1,'err_msg':'message type or environment error!'}";
            }
            if (!message.isValid())
            {
                return "{'ret_code':-1,'err_msg':'message invalid!'}";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "all");
            param.Add("platform", "ios");
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("message_type", message.getType());
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            param.Add("environment", environment);
            if (message.getLoopInterval() > 0 && message.getLoopTimes() > 0)
            {
                param.Add("loop_interval", message.getLoopInterval());
                param.Add("loop_times", message.getLoopTimes());
            }
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给绑定标签的设备, 限 Android 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的标签列表 </param>
        /// <param name = "tagOp"> 标签集合需要进行的逻辑集合运算标识 </param>
        /// <param name = "message"> Android 消息结构体 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushTags(List<string> tagList, string tagOp, Message message)
        {
            if (!IsValidMessageType(message))
                return "message type error!";
            if (!message.isValid() || tagList.Count == 0 || (!tagOp.Equals("AND") && !tagOp.Equals("OR")))
            {
                return "paramas invalid!";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "tag");
            param.Add("platform", "android");
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("multi_pkg", message.getMultiPkg());
            param.Add("message_type", message.getType());
            Dictionary <string, object> tagListParam = new Dictionary<string, object>();
            tagListParam.Add("tags",ToJArray(tagList));
            tagListParam.Add("op", tagOp);
            param.Add("tag_list", tagListParam);
            // param.Add("tag_list", toJArray(tagList));
            // param.Add("tags_op", tagOp);
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            if (message.getLoopInterval() > 0 && message.getLoopTimes() > 0)
            {
                param.Add("loop_interval", message.getLoopInterval());
                param.Add("loop_times", message.getLoopTimes());
            }
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        /// <summery> 推送消息给绑定标签的设备, 限 iOS 系统使用
        /// <param name = "accountList"> 接收设备标识绑定的标签列表 </param>
        /// <param name = "tagOp"> 标签集合需要进行的逻辑集合运算标识 </param>
        /// <param name = "message"> iOS 消息结构体 </param>
        /// <param name = "environment"> 指定推送环境 </param>
        /// <returns> 推送结果描述 </returns>
        /// </summery>
        public async Task<string> PushTags(List<string> tagList, string tagOp, MessageiOS message, PushEnvironmentofiOS environment)
        {
            if (!IsValidMessageType(message, environment))
            {
                return "{'ret_code':-1,'err_msg':'message type or environment error!'}";
            }
            if (!message.isValid())
            {
                return "{'ret_code':-1,'err_msg':'message invalid!'}";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("audience_type", "tag");
            param.Add("platform", "ios");
            // param.Add("access_id", this.xgPushAppAccessKey);
            param.Add("expire_time", message.getExpireTime());
            param.Add("send_time", message.getSendTime());
            param.Add("message_type", message.getType());
            Dictionary <string, object> tagListParam = new Dictionary<string, object>();
            tagListParam.Add("tags",ToJArray(tagList));
            tagListParam.Add("op", tagOp);
            param.Add("tag_list", tagListParam);
            // param.Add("tag_list", tagList);
            // param.Add("tags_op", tagOp);
            param.Add("message", message.toJson());
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            param.Add("environment", environment);
            if (message.getLoopInterval() > 0 && message.getLoopTimes() > 0)
            {
                param.Add("loop_interval", message.getLoopInterval());
                param.Add("loop_times", message.getLoopTimes());
            }
            string ret = await RequestXgServerV3(XingeApp._xgPushServierHost, XingeApp._xgPushAppPath, param);
            return ret;
        }

        //查询群发消息状态
        public async Task<string> QueryPushStatus(List<string> pushIdList)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            JArray ja = new JArray();
            foreach (string pushId in pushIdList)
            {
                JObject jo = new JObject();
                jo.Add("push_id", pushId);
                ja.Add(jo);
            }
            param.Add("push_ids", ja.ToString());
            string ret = await CallRestful(XingeApp.RestapiQuerypushstatus, param);
            return ret;
        }

        //查询消息覆盖的设备数
        private async Task<string> QueryDeviceCount()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            string ret = await CallRestful(XingeApp.RestapiQuerydevicecount, param);
            return ret;
        }

        /**
        * 查询应用当前所有的tags
        *
        * @param start 从哪个index开始
        * @param limit 限制结果数量，最多取多少个tag
        */
        public async Task<string> QueryTags(int start, int limit)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("start", start);
            param.Add("limit", limit);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            string ret = await CallRestful(XingeApp.RestapiQuerytags, param);
            return ret;
        }

        /**
        * 查询应用所有的tags，如果超过100个，取前100个
        *
        */
        public async Task<string> QueryTags()
        {
            return await QueryTags(0, 100);
        }

        /**
        * 查询带有指定tag的设备数量
        *
        * @param tag 指定的标签
        */
        public async Task<string> QueryTagTokenNum(string tag)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("tag", tag);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            string ret = await CallRestful(XingeApp.RestapiQuerytagtokennum, param);
            return ret;
        }

        /**
        * 查询设备下所有的tag
        *
        * @param deviceToken 目标设备token
        */
        public async Task<string> QueryTokenTags(string deviceToken)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("device_token", deviceToken);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

            string ret = await CallRestful(XingeApp.RestapiQuerytokentags, param);
            return ret;
        }

        /**
        * 取消尚未推送的定时任务
        *
        * @param pushId 各类推送任务返回的push_id
        */
        public async Task<string> CancelTimingPush(string pushId)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("push_id", pushId);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

            string ret = await CallRestful(XingeApp.RestapiCanceltimingpush, param);
            return ret;
        }

        //批量为token设置标签
        public async Task<string> BatchSetTag(List<TagTokenPair> tagTokenPairs)
        {
            foreach (TagTokenPair pair in tagTokenPairs)
            {
                if(!this.IsValidToken(pair.token))
                {
                    return string.Format("{\"ret_code\":-1,\"err_msg\":\"invalid token %s\"}", pair.token);
                }
            }
            Dictionary<string, object> param = this.InitParams();
            List<List<string>> tagTokenPair = new List<List<string>>();
            foreach (TagTokenPair pair in tagTokenPairs)
            {
                List<string> singleTagToken = new List<string>();
                singleTagToken.Add(pair.tag);
                singleTagToken.Add(pair.token);;
                tagTokenPair.Add(singleTagToken);
            }
            var json = JsonConvert.SerializeObject(tagTokenPair);
            param.Add("tag_token_list", json);
            string ret = await CallRestful(XingeApp.RestapiBatchsettag, param);
            return ret;
        }

        //批量为token删除标签
        public async Task<string> BatchDelTag(List<TagTokenPair> tagTokenPairs)
        {
            foreach (TagTokenPair pair in tagTokenPairs)
            {
                if (!this.IsValidToken(pair.token))
                {
                    return string.Format("{\"ret_code\":-1,\"err_msg\":\"invalid token %s\"}", pair.token);
                }
            }
            Dictionary<string, object> param = this.InitParams();
            List<List<string>> tagTokenPair = new List<List<string>>();
            foreach (TagTokenPair pair in tagTokenPairs)
            {
                List<string> singleTagToken = new List<string>();
                singleTagToken.Add(pair.tag);
                singleTagToken.Add(pair.token);
                tagTokenPair.Add(singleTagToken);
            }
            var json = JsonConvert.SerializeObject(tagTokenPair);
            param.Add("tag_token_list", json);
            string ret = await CallRestful(XingeApp.RestapiBatchdeltag, param);
            return ret;
        }

        /**
        * 查询token相关的信息，包括最近一次活跃时间，离线消息数等
        *
        * @param deviceToken 目标设备token
        */
        private async Task<string> QueryInfoOfToken(string deviceToken)
        {
            Dictionary < string, object > param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("device_token", deviceToken);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

            string ret = await CallRestful(XingeApp.RestapiQueryinfooftoken, param);
            return ret;
        }

        /**
        * 查询账号绑定的token
        *
        * @param account 目标账号
        */
        private async Task<string> QueryTokensOfAccount(string account)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("account", account);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

            string ret = await  CallRestful(XingeApp.RestapiQuerytokensofaccount, param);
            return ret;
        }

        /**
        * 删除指定账号和token的绑定关系（token仍然有效）
        *
        * @param account 目标账号
        * @param deviceToken 目标设备token
        */
        public async Task<string> DeleteTokenOfAccount(String account, String deviceToken)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("account", account);
            param.Add("device_token", deviceToken);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

            string ret = await CallRestful(XingeApp.RestapiDeletetokenofaccount, param);
            return ret;
        }

        /**
         * 删除指定账号绑定的所有token（token仍然有效）
         *
         * @param account 目标账号
         */
        private async Task<string> DeleteAllTokensOfAccount(String account)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("access_id", this.m_xgPushAppAccessKey);
            param.Add("account", account);
            param.Add("timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

            string ret = await CallRestful(XingeApp.RestapiDeletealltokensofaccount, param);
            return ret;
        }
    }
}
