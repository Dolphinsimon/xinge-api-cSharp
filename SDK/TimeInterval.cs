using System.Collections.Generic;


namespace XingeApp
{
    public class TimeInterval
    {
        private int m_startHour;
        private int m_startMin;
        private int m_endHour;
        private int m_endMin;

        public TimeInterval(int startHour, int startMin, int endHour, int endMin)
        {
            m_startHour = startHour;
            m_startMin = startMin;
            m_endHour = endHour;
            m_endMin = endMin;
        }

        public bool isValid()
        {
            return m_startHour >= 0 && m_startHour <= 23 &&
                   m_startMin >= 0 && m_startMin <= 59 &&
                   m_endHour >= 0 && m_endHour <= 23 &&
                   m_endMin >= 0 && m_endMin <= 59;
        }

        public Dictionary<string,object> toJson()
        {
            var dict = new Dictionary<string, object>();
            var start = new Dictionary<string, object>();
            var end = new Dictionary<string, object>();
            start.Add("hour",m_startHour.ToString());
            start.Add("min",m_startMin.ToString());
            end.Add("hour",m_endHour.ToString());
            end.Add("min",m_endMin.ToString());
            dict.Add("start",start);
            dict.Add("end",end);

            return dict;
        }
    }
}
