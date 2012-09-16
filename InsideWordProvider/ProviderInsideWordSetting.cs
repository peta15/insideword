using System;
using System.Linq;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Collections.Generic;
using System.Text;

namespace InsideWordProvider
{
    internal class ProviderInsideWordSetting : Provider
    {
        protected InsideWordSetting _entityInsideWordSetting;

        public ProviderInsideWordSetting() : base() { }
        public ProviderInsideWordSetting(long id) : base(id) { }

        public string SettingValue
        {
            get { return _entityInsideWordSetting.SettingValue; }
            set { _entityInsideWordSetting.SettingValue = value; }
        }

        public string SettingKey
        {
            get { return _entityInsideWordSetting.SettingKey; }
            set { _entityInsideWordSetting.SettingKey = value; }
        }

        public override bool Load(long id)
        {
            InsideWordSetting entitySetting = DbCtx.Instance.InsideWordSettings
                                            .Where(aSetting => aSetting.Id == id)
                                            .FirstOrDefault();
            return Load(entitySetting);
        }

        public bool Load(string key)
        {
            InsideWordSetting entitySetting = DbCtx.Instance.InsideWordSettings
                                            .Where(aSetting => aSetting.SettingKey.CompareTo(key) == 0)
                                            .SingleOrDefault();
            return Load(entitySetting);
        }

        public bool LoadOrCreate(string key, string value)
        {
            bool returnValue = Load(key);
            if (!returnValue)
            {
                SettingKey = key;
                SettingValue = value;
                Save();
            }
            return returnValue;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityInsideWordSetting.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityInsideWordSetting.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityInsideWordSetting.SystemEditDate);
            sb.Append("\n\tSettingKey =\t" + _entityInsideWordSetting.SettingKey);
            sb.Append("\n\tSettingValue =\t" + _entityInsideWordSetting.SettingValue);
            sb.Append("\n");

            return sb.ToString();
        }

        /* TODO: revisit all copy functions
        public override bool Copy(Provider untyped)
        {
            
        }
        */


        //=========================================================
        // PRIVATE
        //=========================================================
        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityInsideWordSetting; }
            set { _entityInsideWordSetting = (InsideWordSetting)value; }
        }

        protected override void EntityClear()
        {
            _entityInsideWordSetting = new InsideWordSetting();
            _entityInsideWordSetting.Id = -1; ;
            _entityInsideWordSetting.SystemCreateDate = new DateTime();
            _entityInsideWordSetting.SystemEditDate = new DateTime();
            _entityInsideWordSetting.SettingKey = string.Empty;
            _entityInsideWordSetting.SettingValue = string.Empty;
        }

        //=========================================================
        // STATIC
        //=========================================================
        static public long? Exists(string settingsKey)
        {
            long? returnValue = null;
            if (!string.IsNullOrEmpty(settingsKey))
            {
                DbCtx.Instance.InsideWordSettings
                                .Where(aSetting => aSetting.SettingKey == settingsKey)
                                .Select(aSetting => aSetting.Id)
                                .DefaultIfEmpty(-1)
                                .First();
                if (returnValue == -1)
                {
                    returnValue = null;
                }
            }
            return returnValue;
        }
    }
}