using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using InsideWordProvider;
using InsideWordMVCWeb.ViewModels.Admin;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    /// <summary>
    /// Summary description for CategoryUtility
    /// </summary>
    public static class CategoryBL
    {
        static public string NavigationList(HtmlHelper html)
        {
            string htmlNavList;

            if (InsideWordWebStaticCache.Instance.NavigationList == null)
            {
                StringBuilder sb = new StringBuilder("");
                _BuildNavListHelper(html, ProviderCategory.Root.Children(), ref sb, 0);
                htmlNavList = InsideWordWebStaticCache.Instance.NavigationList = sb.ToString();
            }
            else
            {
                htmlNavList = InsideWordWebStaticCache.Instance.NavigationList;
            }

            return htmlNavList;
        }

        static private void _BuildNavListHelper(HtmlHelper html, List<ProviderCategory> categoryList, ref StringBuilder sb, int depth)
        {
            string indent = new string('\t', depth);

            foreach (ProviderCategory aCategory in categoryList)
            {
                sb.Append(indent);
                sb.Append("<li>");
                sb.Append(html.ActionLink(aCategory.Title, MVC.Home.Index(aCategory.Id, null))+"\n");
                if (aCategory.Children().Count > 0)
                {
                    sb.Append(indent + "<ul>\n");
                    _BuildNavListHelper(html, aCategory.Children(), ref sb, depth + 1);
                    sb.Append(indent + "</ul>\n");
                }
                sb.Append(indent + "</li>\n");
            }
        }

        public static bool Save(CategoryEditVM model, ProviderCurrentMember currentMember)
        {
            bool returnValue = false;
            ProviderCategory aCategory;
            if (ProviderCategory.Exists(model.Id))
            {
                aCategory = new ProviderCategory(model.Id);
            }
            else
            {
                aCategory = new ProviderCategory();
            }

            if (currentMember.CanEdit(aCategory))
            {
                aCategory.Title = model.Title;
                aCategory.EditDate = DateTime.UtcNow;
                aCategory.IsHidden = model.IsHidden;
                aCategory.ParentId = model.ParentId;
                try
                {
                    aCategory.Save();
                    InsideWordWebStaticCache.Instance.ClearCache();
                    returnValue = true;
                }
                catch (Exception caughtException)
                {
                    returnValue = false;
                }
            }

            return returnValue;
        }

        public static bool Delete(IList<NumBitVM> model, ProviderCurrentMember currentMember)
        {
            bool returnStatus = true;
            foreach (NumBitVM pair in model)
            {
                if (pair.Bit && ProviderCategory.Exists(pair.Num))
                {
                    ProviderCategory aCategory = new ProviderCategory(pair.Num);
                    if (currentMember.CanEdit(aCategory))
                    {
                        returnStatus &= aCategory.Delete();
                    }
                }
            }
            InsideWordWebStaticCache.Instance.ClearCache();
            return returnStatus;
        }
    }
}