using System;
using System.Web;
using System.Web.Mvc;

// For extensions to work properly they must not be in a namespace

    /// <summary>
    /// The start of a flexible programmable title
    /// Add model data: <%: Html.Title("{0} {1} Is Alive!", Model.User.FirstName, Model.User.LastName) %> 
    /// Change 
    /// </summary>
    public static class HtmlTitle
    {
        const string baseTitle = "InsideWord » {0}";
        const string defaultTitle = "InsideWord";

        public static string Title(this HtmlHelper html, string titleValue)
        {
            return Title(html, titleValue, null);
        }

        public static string Title(this HtmlHelper html, string titleValue, params string[] titleFormats)
        {
            string titleValueFormatted;

            //get title value
            if (string.IsNullOrEmpty(titleValue))
            {
                titleValueFormatted = defaultTitle;
            }
            else
            {
                //format the title
                if (titleFormats != null && titleFormats.Length > 0)
                {
                    titleValue = string.Format(titleValue, titleFormats);
                }

                //format title value, combine base with title 
                titleValueFormatted = string.Format(baseTitle, titleValue);
            }

            return "<title>" + HttpUtility.HtmlEncode(titleValueFormatted) + "</title>";
        }
    }