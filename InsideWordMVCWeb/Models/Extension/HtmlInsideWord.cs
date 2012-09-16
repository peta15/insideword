using System;
using System.Text;
using System.Web.Mvc;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordResource;

// For extensions to work properly they must not be in a namespace

    public static class HtmlInsideWord
    {
        public static MvcHtmlString NewsTicker(this HtmlHelper html)
        {
            return MvcHtmlString.Create("<div id=\"newsTicker\" class=\"newsTicker\">\n" + ArticleBL.GetCachedTickerHeadlines() + "\n</div>");
        }

        public static string NavigationList(this HtmlHelper html)
        {
            string list = CategoryBL.NavigationList(html);
            return list != String.Empty ? "<ul class='sf-menu sf-vertical sf-js-enabled sf-shadow'>" + list + "</ul>" : String.Empty;
        }

        /// <summary>
        /// Evaluates a member view model and creates an appropriate link to that member's profile.
        /// </summary>
        /// <param name="html">html helper</param>
        /// <param name="viewModel">member we wish to create a profile link for</param>
        /// <param name="currentMember">member we wish to create a profile/account link for</param>
        /// <returns>returns either an anchor tag or span tag depending on whether the member is anonymous/exists</returns>
        public static MvcHtmlString ProfileLink(this HtmlHelper html, MemberVM viewModel, CurrentMemberVM currentMember, int cutOff = 0)
        {
            MvcHtmlString returnValue = null;

            string linkText;
            if (viewModel == null || viewModel.IsAnonymous)
            {
                linkText = "Anonymous";
                if (cutOff > 0)
                {
                    linkText = IWStringUtility.TruncateClean(linkText, cutOff);
                }
                returnValue = MvcHtmlString.Create("<span>" + linkText + "</span>");
            }
            else if (currentMember != null && viewModel.Id == currentMember.Id)
            {
                if (currentMember.IsActive)
                {
                    linkText = "You";
                    if (cutOff > 0)
                    {
                        linkText = IWStringUtility.TruncateClean(linkText, cutOff);
                    }
                    returnValue = html.ActionLink(linkText, MVC.Member.Profile(viewModel.Id.Value, null));
                }
                else
                {
                    linkText = "Activate profile";
                    if (cutOff > 0)
                    {
                        linkText = IWStringUtility.TruncateClean(linkText, cutOff);
                    }
                    returnValue = html.ActionLink(linkText, MVC.Member.Profile(viewModel.Id.Value, null));
                }
            }
            else
            {
                linkText = viewModel.DisplayName;
                if (cutOff > 0)
                {
                    linkText = IWStringUtility.TruncateClean(linkText, cutOff);
                }
                returnValue = html.ActionLink(linkText, MVC.Member.Profile(viewModel.Id.Value, null));
            }

            return returnValue;
        }

        /// <summary>
        /// Evaluates a member view model and creates an appropriate link to that member's Profile and Account.
        /// </summary>
        /// <param name="html">html helper</param>
        /// <param name="currentMember">member we wish to create a profile/account link for</param>
        /// <returns>returns either an anchor tag or span tag depending on whether the member is anonymous/exists</returns>
        public static MvcHtmlString CombinedProfileAccountLink(this HtmlHelper html, CurrentMemberVM currentMember)
        {
            MvcHtmlString returnValue = null;
            if (currentMember.IsActive)
            {
                returnValue = MvcHtmlString.Create("<span>My " + html.ActionLink("Profile", MVC.Member.Profile(currentMember.Id.Value, null)).ToString()
                                                        + " | "
                                                        + html.ActionLink("Settings", MVC.Member.Account(currentMember.Id.Value, null)).ToString()
                                                        + "</span>");
            }
            else if (currentMember.Id.HasValue)
            {
                returnValue = html.ActionLink("Activate profile", MVC.Member.ChangePassword(null));
            }
            else
            {
                returnValue = MvcHtmlString.Create("<span>Anonymous</span>");
            }

            return returnValue;
        }

        /// <summary>
        /// Function returns an img tag for a given Photo view model
        /// </summary>
        /// <param name="html">html helper</param>
        /// <param name="model">photo we wish to create the img tag for</param>
        /// <returns>returns an img tag</returns>
        public static MvcHtmlString Photo(this HtmlHelper html, ImageInfo model)
        {
            StringBuilder sb = new StringBuilder();
            if (model != null)
            {
                sb.Append("<img src='" + model.Src);
                sb.Append("' width='" + model.Width);
                sb.Append("' height='" + model.Height);
                sb.Append("' alt='" + model.Alt + "' />");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Function returns html tags for a photo with a caption
        /// </summary>
        /// <param name="html">html helper</param>
        /// <param name="model">photo we wish to create the captioned img for</param>
        /// <returns>returns several nested div and img tags</returns>
        public static MvcHtmlString CaptionedPhoto(this HtmlHelper html, ImageInfo model)
        {
            StringBuilder sb = new StringBuilder();
            if (model != null)
            {
                sb.Append("<div class='left'>");
                sb.Append(html.Photo(model));
                sb.Append("<div class='caption'>");
                sb.Append("<div class='left'>" + model.Alt + "</div>");
                sb.Append("<div class='cls'> </div></div></div>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }