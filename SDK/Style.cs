﻿namespace XingeApp
{
    public class Style
    {
        private int m_builderId;
        private int m_ring;
        private int m_vibrate;
        private int m_clearable;
        private int m_nId;
        private int m_lights;
        private int m_iconType;
        private int m_styleId;
        private string m_ringRaw;
        private string m_iconRes;
        private string m_smallIcon;

        public Style(int builderId)
        {
            m_builderId = builderId;
            m_ring = 0;
            m_vibrate = 0;
            m_clearable = 1;
            m_nId = 0;
            m_lights = 1;
            m_iconType = 0;
            m_styleId = 1;
        }

        public Style(int builderId, int ring, int vibrate, int clearable, int nId, int lights, int iconType, int styleId)
        {
            m_builderId = builderId;
            m_ring = ring;
            m_vibrate = vibrate;
            m_clearable = clearable;
            m_nId = nId;
            m_lights = lights;
            m_iconType = iconType;
            m_styleId = styleId;
        }

        public Style(int builderId, int ring, int vibrate, int clearable, int nId)
        {
            m_builderId = builderId;
            m_ring = ring;
            m_vibrate = vibrate;
            m_clearable = clearable;
            m_nId = nId;
        }

        public int getBuilderId()
        {
            return m_builderId;
        }
        public int getRing()
        {
            return m_ring;
        }
        public int getVibrate()
        {
            return m_vibrate;
        }
        public int getClearable()
        {
            return m_clearable;
        }
        public int getNId()
        {
            return m_nId;
        }
        public int getLights()
        {
            return m_lights;
        }
        public int getIconType()
        {
            return m_iconType;
        }
        public int getStyleId()
        {
            return m_styleId;
        }
        public string getRingRaw()
        {
            return m_ringRaw;
        }
        public string getIconRes()
        {
            return m_iconRes;
        }
        public string getSmallIcon()
        {
            return m_smallIcon;
        }

        public bool isValid()
        {
            if (m_ring < 0 || m_ring > 1) return false;
            if (m_vibrate < 0 || m_vibrate > 1) return false;
            if (m_clearable < 0 || m_clearable > 1) return false;
            if (m_lights < 0 || m_lights > 1) return false;
            if (m_iconType < 0 || m_iconType > 1) return false;
            if (m_styleId < 0 || m_styleId > 1) return false;

            return true;
        }
    }
}
