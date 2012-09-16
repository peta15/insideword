using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordMVCWeb.ViewModels.Group;
using InsideWordProvider;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    public static class GroupBL
    {
        static public bool Save(GroupRegisterVM model, ProviderGroup aGroup)
        {
            if (aGroup.IsNew)
            {
                aGroup.CreateDate = DateTime.UtcNow;
            }
            aGroup.Name = model.Name;
            aGroup.EditDate = DateTime.UtcNow;
            aGroup.Save();
            return true;
        }
    }
}