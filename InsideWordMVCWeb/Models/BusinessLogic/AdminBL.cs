using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordMVCWeb.ViewModels.Admin;
using InsideWordProvider;
using InsideWordMVCWeb.Models.Utility;
using log4net.Repository;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    static public class AdminBL
    {
        static public bool Save(SettingsManagementVM model)
        {
            InsideWordSettingsDictionary.Instance.DefaultCategoryId = model.DefaultCategoryId;
            InsideWordSettingsDictionary.Instance.LogThresholdName = model.LogThresholdName;
            InsideWordSettingsDictionary.Instance.LogEmailThresholdName = model.LogEmailThresholdName;
            InsideWordWebLog.Instance.IWThreshold = InsideWordWebLog.Instance.Log.Logger.Repository.LevelMap[model.LogThresholdName];
            InsideWordWebLog.Instance.IWSmtpThreshold = InsideWordWebLog.Instance.Log.Logger.Repository.LevelMap[model.LogEmailThresholdName];
            return true;
        }
    }
}