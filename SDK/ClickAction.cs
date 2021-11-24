using System.Collections.Generic;


namespace XingeApp
{
    public class ClickAction
    {
        public static int TYPE_ACTIVITY = 1;
        public static int TYPE_URL = 2;
        public static int TYPE_INTENT = 3;

        private int m_actionType;
        private string m_url;
        private int m_confirmUrl;
        private string m_activity;
        private string m_intent;

        public void setActionType(int actionType) { m_actionType = actionType; }
        public void setActivity(string activity) { m_activity = activity; }
        public void setUrl(string url) { m_url = url; }
        public void setConfirmUrl(int confirmUrl) { m_confirmUrl = confirmUrl; }
        public void setIntent(string intent) { m_intent = intent; }

        public Dictionary<string,object> toJson()
        {
           var dict = new Dictionary<string, object>
           {
               { "action_type", m_actionType },
               { "activity", m_activity },
               { "intent", m_intent }
           };
           var browser = new Dictionary<string, object>
           {
               { "url", m_url },
               { "confirm", m_confirmUrl }
           };
           dict.Add("browser", browser);

            //string jsonData = JsonConvert.SerializeObject(dict);
            return dict;
        }


        public bool isValid()
        {
            if (m_actionType < TYPE_ACTIVITY || m_actionType > TYPE_INTENT)
                return false;
            if(m_actionType == TYPE_URL)
            {
                return m_url.Length != 0 && m_confirmUrl >= 0 && m_confirmUrl <= 1;
            }
            if(m_actionType == TYPE_INTENT)
            {
                return m_intent.Length != 0;
            }
            return true;
        }

        public ClickAction()
        {
            m_actionType = 1;
            m_activity = "";
            m_url = "";
            m_intent = "";
        }
    }
}
