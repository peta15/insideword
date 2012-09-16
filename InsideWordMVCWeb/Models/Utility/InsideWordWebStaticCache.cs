using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InsideWordMVCWeb.Models.Utility
{
    public class InsideWordWebStaticCache
    {
        public ActionResult NewArticleEditorView { get; set; }
        public string NavigationList { get; set; }

        public void ClearCache()
        {
            NewArticleEditorView = null;
            NavigationList = null;
        }

        //=========================================================
        // PRIVATE
        //=========================================================
        protected InsideWordWebStaticCache()
        {
            ClearCache();
        }

        //=========================================================
        // STATIC
        //=========================================================
        static protected InsideWordWebStaticCache _instance;
        static public InsideWordWebStaticCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InsideWordWebStaticCache();
                }
                return _instance;
            }
        }
    }
}
