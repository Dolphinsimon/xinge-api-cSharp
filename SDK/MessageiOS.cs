using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace XingeApp
{
    public class MessageiOS
    {
        private int m_expireTime;
        private string m_sendTime;
        private List<TimeInterval> m_acceptTimes;
        private string m_type;
        private Dictionary<string, object> m_custom;
        private string m_raw;
        private string m_alertStr;
        private Dictionary<string,object> m_alertJo;
        private int m_badge;
        private string m_sound;
        private string m_category;
        private int m_loopInterval;
        private int m_loopTimes;
        private string m_title;
        private string m_subtitle;

        private int m_pushID;
        
        /// -1，表示角标不变；
        /// -2，表示角标自动加+；
        /// n，(n >= 0)表示自定义角标数值
        private int m_badgeType;

        public MessageiOS()
        {
            m_sendTime = "";
            m_acceptTimes = new List<TimeInterval>();
            m_raw = "";
            m_alertStr = "";
            m_badge = 0;
            m_sound = "";
            m_category = "";
            m_loopInterval = -1;
            m_loopTimes = -1;
            m_type = XGPushConstants.OrdinaryMessage;
            m_title = "";
            m_subtitle = "";
            m_pushID = 0;
            m_badgeType = -1;
        }

        public void setType(string type)
        {
            m_type = type;
        }

        public string getType()
        {
            return m_type;
        }

        public void setExpireTime(int expireTime)
        {
            m_expireTime = expireTime;
        }

        public int getExpireTime()
        {
            return m_expireTime;
        }

        public void setSendTime(string sendTime)
        {
            m_sendTime = sendTime;
        }

        public string getSendTime()
        {
            return m_sendTime;
        }


        public void addAcceptTime(TimeInterval acceptTime)
        {
            m_acceptTimes.Add(acceptTime);
        }

        public JArray acceptTimeToJsonArray()
        {
            var json = new JArray();
            foreach (var ti in m_acceptTimes)
            {
                var jtemp = JObject.FromObject(ti.toJson());
                json.Add(jtemp);
            }
            return json;
        }

        public void setCustom(Dictionary<string, object> custom)
        {
            m_custom = custom;
        }

        public void setRaw(string raw)
        {
            m_raw = raw;
        }

        public void setAlert(string alert)
        {
            m_alertStr = alert;
        }

        public void setAlert(Dictionary<string,object> alert)
        {
            m_alertJo = alert;
        }

        [Obsolete("方法已经停用，请使用setBadgeType")]
        public void setBadge(int badge)
        {
            m_badge = badge;
        }
        
        public void setTitle(string title) 
        {
            m_title = title;
        }
        
        public void setSubTitle(string title) 
        {
            m_subtitle = title;
        }

        public void setSound(string sound)
        {
            m_sound = sound;
        }

        public void setCategory(string category)
        {
            m_category = category;
        }

        public int getLoopInterval()
        {
            return m_loopInterval;
        }

        public void setLoopInterval(int loopInterval)
        {
            m_loopInterval = loopInterval;
        }

        public int getLoopTimes()
        {
            return m_loopTimes;
        }

        public void setLoopTimes(int loopTimes)
        {
            m_loopTimes = loopTimes;
        }

        public void setPushID(int pushid)
        {
            m_pushID = pushid;
        }
        public int getPushID()
        {
            return m_pushID;
        }

        /// <summery> 设置角标下发的逻辑
        /// <list type="int">
        /// <item>
        /// <term>-1</term>
        /// <description>表示角标不变;</description>
        /// </item>
        /// <item>
        /// <term>-2</term>
        /// <description>表示角标自动加1;</description>
        /// </item>
        /// <item>
        /// <term>n</term>
        /// <description>(n>=0)表示自定义下发角标数字.</description>
        /// </item>
        /// </list>
        /// </summery>
        public void setBadgeType(int type) 
        {
            m_badgeType = type;
        }

        public int badgeType() 
        {
            return m_badgeType;
        }

        public bool isValid()
        {
            if (m_raw.Length != 0)
                return true;
            if (m_expireTime < 0 || m_expireTime > 3 * 24 * 60 * 60)
                return false;
            if ( m_type != (XGPushConstants.OrdinaryMessage) && m_type != (XGPushConstants.SilentMessage) && m_type != "0")
                return false;
            foreach (var ti in m_acceptTimes)
            {
                if (!ti.isValid()) return false;
            }
            if (m_loopInterval > 0 && m_loopTimes > 0 && ((m_loopTimes - 1) * m_loopInterval + 1) > 15)
            {
                return false;
            }
            return true;
        }

        public object toJson()
        {
            if (m_raw.Length != 0)
                return m_raw;
            var dict = new Dictionary<string, object> { { "accept_time", acceptTimeToJsonArray() } };
            var aps = new Dictionary<string, object>();
            var iOS = new Dictionary<string, object>();

            if(m_type.Equals(XGPushConstants.OrdinaryMessage) || m_type == "0")
            {
                var alert = new Dictionary<string, object>();
                aps.Add("alert",alert);
                // aps.Add("badge",m_badge);
                aps.Add("badge_type", m_badgeType);
                if(m_sound.Length != 0)
                {
                    aps.Add("sound",m_sound);
                }
                if(m_category.Length != 0)
                {
                    aps.Add("category",m_category);
                }
                if (m_subtitle.Length != 0)
                {
                    iOS.Add("subtitle", m_subtitle);
                }
                if (m_title.Length != 0)
                {
                    dict.Add("title", m_title);
                }
                if (m_alertStr.Length != 0)
                {
                    dict.Add("content", m_alertStr);
                }

            } else if (m_type.Equals(XGPushConstants.SilentMessage)) {
                aps.Add("content-available", 1);
            }

            iOS.Add("aps",aps);
            if (m_custom != null)
            {
                foreach(var kvp in m_custom)
                {
                    iOS.Add(kvp.Key, kvp.Value);
                }
            }
            

            dict.Add("ios", iOS);
            
            return dict;
        }
    }

    
}
