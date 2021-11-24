using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace XingeApp
{
    public class Message
    {
        private string m_title;
        private string m_content;
        private int m_expireTime;
        private string m_sendTime;
        private List<TimeInterval> m_acceptTimes;
        private string m_type;
        private int m_multiPkg;
        private Style m_style;
        private ClickAction m_action;
        private Dictionary<string, object> m_custom;
        private string m_raw;
        private int m_loopInterval;
        private int m_loopTimes;
        private int m_pushID;

        public Message()
        {
            m_title = "";
            m_content = "";
            m_sendTime = "";
            m_acceptTimes = new List<TimeInterval>();
            m_multiPkg = 0;
            m_raw = "";
            m_loopInterval = -1;
            m_loopTimes = -1;
            m_action = new ClickAction();
            m_style = new Style(0);
            m_pushID = 0;
        }

        public void setTitle(string title)
        {
            m_title = title;
        }
        public void setContent(string content)
        {
            m_content = content;
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
            foreach (var jtemp in m_acceptTimes.Select(ti => JObject.FromObject(ti.toJson())))
            {
                json.Add(jtemp);
            }
            return json;
        }
        public void setType(string type)
        {
            m_type = type;
        }
        public string getType()
        {
            return m_type;
        }
        public void setMultiPkg(int multiPkg)
        {
            m_multiPkg = multiPkg;
        }
        public int getMultiPkg()
        {
            return m_multiPkg;
        }
        public void setStyle(Style style)
        {
            m_style = style;
        }
        public void setAction(ClickAction action)
        {
            m_action = action;
        }
        public void setCustom(Dictionary<string, object> custom)
        {
            m_custom = custom;
        }
        public void setRaw(string raw)
        {
            m_raw = raw;
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

        public bool isValid()
        {
            if (m_raw.Length != 0)
                return true;
            if (m_type != (XGPushConstants.OrdinaryMessage) && m_type != (XGPushConstants.SilentMessage) && m_type != "1" && m_type != "2")
                return false;
            if (m_multiPkg < 0 || m_multiPkg > 1)
                return false;
            if (m_type == (XGPushConstants.OrdinaryMessage) || m_type != "1")
            {
                if (!m_style.isValid()) return false;
                if (!m_action.isValid()) return false;
            }
            if (m_expireTime < 0 || m_expireTime > 3 * 24 * 60 * 60)
                return false;
            if (m_acceptTimes.Any(ti => !ti.isValid()))
            {
                return false;
            }
            return m_loopInterval <= 0 || m_loopTimes <= 0 || ((m_loopTimes - 1) * m_loopInterval + 1) <= 15;
        }

        public object toJson()
        {
            if (m_raw.Length != 0)
                return m_raw;
            var dict = new Dictionary<string, object>();
            var message = new Dictionary<string, object>();
            
            dict.Add("title", m_title);
            dict.Add("content", m_content);
            dict.Add("accept_time", acceptTimeToJsonArray());
                
            if (m_type.Equals(XGPushConstants.OrdinaryMessage))
            {
                
                message.Add("builder_id", m_style.getBuilderId());
                message.Add("ring", m_style.getRing());
                message.Add("vibrate", m_style.getVibrate());
                message.Add("clearable", m_style.getClearable());
                message.Add("n_id", m_style.getNId());
                message.Add("ring_raw", m_style.getRingRaw());
                message.Add("lights", m_style.getLights());
                message.Add("icon_type", m_style.getIconType());
                message.Add("icon_res", m_style.getIconRes());
                message.Add("style_id", m_style.getStyleId());
                message.Add("small_icon", m_style.getSmallIcon());
                message.Add("action", m_action.toJson());
            }
            else if(m_type.Equals(XGPushConstants.SilentMessage))
            {
                //
            }
            
            if (m_custom != null)
            {
                foreach(var kvp in m_custom)
                {
                    message.Add(kvp.Key, kvp.Value);
                }
            }

            dict.Add("android", message);

            return dict;
        }
    }

    
}
