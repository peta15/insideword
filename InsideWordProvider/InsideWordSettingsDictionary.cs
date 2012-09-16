using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsideWordProvider
{
    public class InsideWordSettingsDictionary
    {
        public bool Set(string key, string value)
        {
            bool returnValue = true;
            try
            {
                ProviderInsideWordSetting aSetting = new ProviderInsideWordSetting();
                aSetting.Load(key); // whether is successed or not it doesn't matter
                aSetting.SettingKey = key;
                aSetting.SettingValue = value;
                aSetting.Save();
            }
            catch(Exception ignoredException)
            {
                returnValue = false;
            }

            return returnValue;
        }

        public string Get(string key, string defaultString = null)
        {
            string returnValue = defaultString;
            ProviderInsideWordSetting aSetting = new ProviderInsideWordSetting();
            if (aSetting.Load(key))
            {
                returnValue = aSetting.SettingValue;
            }
            return returnValue;
        }

        public long? DefaultCategoryId
        {
            get
            {
                string value = Get("defaultCategoryId", null);
                long? returnValue = null;
                if(!string.IsNullOrWhiteSpace(value))
                {
                    returnValue = long.Parse(value);
                }
                return returnValue;
            }
            set { Set("defaultCategoryId", value.ToString()); }
        }

        public string LogThresholdName
        {
            get { return Get("logThresholdName", null); }
            set { Set("logThresholdName", value); }
        }

        public string LogEmailThresholdName
        {
            get { return Get("logEmailThresholdName", null); }
            set { Set("logEmailThresholdName", value); }
        }

        //=========================================================
        // PRIVATE
        //=========================================================
        protected InsideWordSettingsDictionary() { }

        //=========================================================
        // STATIC
        //=========================================================
        static protected InsideWordSettingsDictionary _instance;
        static public InsideWordSettingsDictionary Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new InsideWordSettingsDictionary();
                }
                return _instance;
            }
        }
    }
}
