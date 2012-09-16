/// <summary>
/// Summary description for Settings
/// </summary>
using System.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace InsideWordMVCWeb.Models.Utility
{
    public static class InsideWordWebSettings
    {
        static public List<string> ImageHosts
        {
            get { return ConfigurationManager.AppSettings["ImageHosts"].Split(',').ToList(); }
        }

        static public string MollomPrivateKey
        {
            get { return ConfigurationManager.AppSettings["MollomPrivateKey"].ToString(); }
        }

        static public string MollomPublicKey
        {
            get { return ConfigurationManager.AppSettings["MollomPublicKey"].ToString(); }
        }

        static public double MollomArticleQuality
        {
            get { return double.Parse(ConfigurationManager.AppSettings["MollomArticleQuality"].ToString()); }
        }

        static public double MollomCommentQuality
        {
            get { return double.Parse(ConfigurationManager.AppSettings["MollomCommentQuality"].ToString()); }
        }

        static public string HostName
        {
            get { return ConfigurationManager.AppSettings["HostName"].ToString(); }
        }
    }
}
