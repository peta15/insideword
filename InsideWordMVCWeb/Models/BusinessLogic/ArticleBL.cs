using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Caching;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using InsideWordProvider;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.ViewModels.Admin;
using InsideWordMVCWeb.ViewModels.Article;
using InsideWordResource;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordAdvancedResource;
using System.Net.Mail;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using HtmlAgilityPack;
using InsideWordMVCWeb.ViewModels.Home;

namespace InsideWordMVCWeb.Models.BusinessLogic
{

    /// <summary>
    /// Summary description for ArticleBL
    /// </summary>
    public static class ArticleBL
    {
        private const string _htmlHeadLineKey = "headLineKey";
        private const int maxHeadlines = 16;
        static public string GetCachedTickerHeadlines()
        {
            string htmlHeadLine;
            if (HttpContext.Current.Cache[_htmlHeadLineKey] == null)
            {
                UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                DateTime expiration = DateTime.UtcNow.AddMinutes(1);
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class='items'>\n");
                Dictionary<long, string> headlines = ProviderArticle.GetNewestIdTitle(maxHeadlines);
                if (headlines.Count == 0)
                {
                    sb.Append("\t<div><a href='" + url.Action(MVC.Article.ArticleEdit(null, null)));
                    sb.Append("'>");
                    sb.Append("Click here to publish your thoughts, news, or info and be discovered");
                    sb.Append("</a></div>\n");
                }
                else
                {
                    foreach (KeyValuePair<long, string> pair in headlines)
                    {
                        sb.Append("\t<div><a href='" + url.Action(MVC.Article.ArticleDetails(pair.Key)));
                        sb.Append("'>");
                        sb.Append(pair.Value);
                        sb.Append("</a></div>\n");
                    }
                }
                sb.Append("</div>");
                htmlHeadLine = sb.ToString();
                HttpContext.Current.Cache.Add(_htmlHeadLineKey,
                                              htmlHeadLine,
                                              null,
                                              expiration,
                                              Cache.NoSlidingExpiration,
                                              CacheItemPriority.Low,
                                              null);
            }
            else
            {
                htmlHeadLine = (string)HttpContext.Current.Cache[_htmlHeadLineKey];
            }

            return htmlHeadLine;
        }

        private const int BATCH_LOAD_AMOUNT = 128;
        private const int MAX_BLURBS = 1024;
        private const string _rankedBlurbListKey = "rankedBlurbListKey";
        public const int BLURBS_PER_PAGE = 32;
        public const int MAX_BLURB_PAGE = MAX_BLURBS / BLURBS_PER_PAGE;
        public const int BLURB_CACHE_SECONDS = 600;
        public static List<BlurbVM> LoadCachedRankedBlurbs(ProviderCategory aCategory, int page)
        {
            string cacheKey = _rankedBlurbListKey + aCategory.Id.Value;
            List<BlurbVM> returnList = new List<BlurbVM>();
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                returnList = (List<BlurbVM>)HttpContext.Current.Cache[cacheKey];
            }

            int requestStartRange = page * BLURBS_PER_PAGE;
            int requestEndRange = Math.Min(requestStartRange + BLURBS_PER_PAGE, MAX_BLURBS);
            if (returnList.Count < requestEndRange && requestEndRange < MAX_BLURBS)
            {
                int loadAmount = Math.Max(BATCH_LOAD_AMOUNT, requestEndRange - returnList.Count);
                List<BlurbVM> blurbList = ProviderArticle.LoadRankedBlurbBy(aCategory, returnList.Count, loadAmount)
                                                         .ConvertAll<BlurbVM>(anArticle => new BlurbVM(anArticle));

                // Distinct destroys the score order so we can't use that function unfortunately : (
                foreach (BlurbVM aBlurb in blurbList)
                {
                    if (!returnList.Contains(aBlurb))
                    {
                        returnList.Add(aBlurb);
                    }
                }

                DateTime expiration = DateTime.UtcNow.AddSeconds(BLURB_CACHE_SECONDS);
                HttpContext.Current.Cache.Remove(cacheKey);
                HttpContext.Current.Cache.Add(cacheKey,
                                              returnList,
                                              null,
                                              expiration,
                                              Cache.NoSlidingExpiration,
                                              CacheItemPriority.Low,
                                              null);
            }
            int skip = Math.Min(requestStartRange, returnList.Count);
            int take = Math.Min(BLURBS_PER_PAGE, returnList.Count - skip);
            return returnList.GetRange(skip, take);
        }

        public static JqGridResponse Process(JqGridEditArticleVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse;
            if (model.Oper.CompareTo("edit") == 0)
            {
                aResponse = Edit(model, currentMember);
            }
            else if (model.Oper.CompareTo("del") == 0)
            {
                aResponse = Delete(model, currentMember);
            }
            else
            {
                aResponse = new JqGridResponse();
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.OPERATION_UNKNOWN(model.Oper);
            }
            return aResponse;
        }

        public static JqGridResponse Edit(JqGridEditArticleVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse = new JqGridResponse();
            ProviderArticle anArticle = new ProviderArticle(model.Id);
            if (currentMember.CanEdit(anArticle))
            {
                anArticle.IgnoreFlags = model.IgnoreFlags;
                anArticle.IsHidden = model.IsHidden;
                anArticle.IsPublished = model.IsPublished;
                try
                {
                    anArticle.Save();
                    aResponse.Success = true;
                }
                catch (Exception caughtException)
                {
                    aResponse.Success = false;
                    aResponse.Message = ErrorStrings.OPERATION_FAILED;
                }
            }
            else
            {
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.OPERATION_NO_RIGHTS;
            }

            return aResponse;
        }

        public static JqGridResponse Delete(JqGridEditArticleVM model, ProviderCurrentMember currrentMember)
        {
            JqGridResponse aResponse = new JqGridResponse();
            ProviderArticle anArticle = new ProviderArticle(model.Id);
            if (currrentMember.CanEdit(anArticle))
            {
                if (anArticle.Delete())
                {
                    aResponse.Success = true;
                }
                else
                {
                    aResponse.Success = false;
                    aResponse.Message = ErrorStrings.OPERATION_FAILED;
                }
            }
            else
            {
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.OPERATION_NO_RIGHTS;
            }
            return aResponse;
        }

        static public bool Save(ArticleEditorVM model, ProviderArticle anArticle, ProviderCurrentMember currentMember, ref List<string> errorList)
        {
            bool returnValue = false;

            ProviderMember owningMember = GetArticleOwner(model, anArticle, currentMember);

            // IsNowClaimedArticle indicates if an article's state changed from unclaimed to now claimed
            bool IsNowClaimedArticle = anArticle.MemberId == null && owningMember.Id.HasValue;
            anArticle.MemberId = owningMember.Id;

            if (AssociatePhotos(model.ArticleBody, anArticle, ref errorList))
            {
                if (owningMember.IsActive && !currentMember.IsLoggedOn)
                {
                    // The owner is an active member but the current member is not logged in?! We're not sure if the member was lazy
                    // and didn't bother to login or if this is a malicious person, so we will set IsPublished to false and treat
                    // the article as a draft just to be safe.
                    anArticle.IsPublished = false;
                }
                else if (model.SaveState == ArticleEditorVM.SaveStates.Published)
                {
                    anArticle.IsPublished = true;
                }
                else if (model.SaveState == ArticleEditorVM.SaveStates.DraftAndContinue ||
                         model.SaveState == ArticleEditorVM.SaveStates.DraftAndPreview)
                {
                    anArticle.IsPublished = false;
                }
                else
                {
                    // defensive programming
                    anArticle.IsPublished = false;
                }

                anArticle.Title = model.Title;
                anArticle.Blurb = model.Blurb;
                anArticle.RawText = model.ArticleBody;

                if (anArticle.IsNew)
                {
                    anArticle.CreateDate = DateTime.UtcNow;
                }
                anArticle.EditDate = DateTime.UtcNow;

                // remove previous categories before adding new ones
                anArticle.RemoveAllCategories();
                anArticle.AddCategory(model.ArticleCategoryId);

                // important that we fetch this info before we save because then the article is no longer new.
                bool isNew = anArticle.IsNew;

                anArticle.Save();

                // if the current member is not logged on then save it in their workspace
                if (!currentMember.IsLoggedOn)
                {
                    currentMember.CurrentWorkSpace.ArticleIdList.Add(anArticle.Id.Value);
                }

                // Send out an e-mail for the article only if it was claimed for the first time through e-mail
                // and the member is not active
                if (IsNowClaimedArticle && !string.IsNullOrEmpty(model.ArticleEmail) && !currentMember.IsActive)
                {
                    EmailManager.Instance.SendEditArticleEmail(new MailAddress(model.ArticleEmail), anArticle, owningMember);
                }
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Function to help determine the owner of an article. If sufficient information is
        /// provided and the owner does not exist in the system then they will be created.
        /// </summary>
        /// <param name="model">view model containing the data of the new article</param>
        /// <param name="currentMember">the current member using the site</param>
        /// <returns>Returns a ProviderMember who is the owner of the article</returns>
        static public ProviderMember GetArticleOwner(ArticleEditorVM model, ProviderArticle anArticle, ProviderCurrentMember currentMember)
        {
            ProviderMember owningMember;

            if (anArticle.MemberId.HasValue)
            {
                owningMember = new ProviderMember(anArticle.MemberId.Value);
            }
            else if (anArticle.IsNew || !string.IsNullOrEmpty(model.ArticleEmail))
            {
                // Have we been provided with an e-mail of the owner?
                if (!string.IsNullOrEmpty(model.ArticleEmail))
                {
                    MailAddress email = new MailAddress(model.ArticleEmail);
                    long? memberId = ProviderEmail.FindOwner(email, true);
                    if (memberId.HasValue)
                    {
                        // The owner already exists in our system so just retrieve them
                        owningMember = new ProviderMember(memberId.Value);
                    }
                    else
                    {
                        // the owner doesn't exists so create them
                        owningMember = new ProviderMember();
                        owningMember.CreateDate = DateTime.UtcNow;
                        owningMember.EditDate = DateTime.UtcNow;
                        owningMember.Save();

                        // attach the e-mail to this member
                        ProviderEmail anEmail = new ProviderEmail();
                        anEmail.MemberId = owningMember.Id.Value;
                        anEmail.IsValidated = false;
                        anEmail.CreateDate = DateTime.UtcNow;
                        anEmail.EditDate = DateTime.UtcNow;
                        anEmail.Email = email;
                        anEmail.Save();
                    }
                }
                else
                {
                    // no e-mail provided so just use whoever is currently logged on, whether they be anonymous or not
                    owningMember = currentMember;
                }
            }
            else
            {
                // this article has no owner so just return a blank member
                owningMember = new ProviderMember();
            }

            return owningMember;
        }

        static public bool AssociatePhotos(string articleText, ProviderArticle anArticle, ref List<string> errorList)
        {
            bool returnValue = true;
            //Detach all Photos from their relationship with the article.
            anArticle.RemoveAllPhotos();

            // Note: There is a bit of a performance hit since we load the article body into the HtmlParser and then later do it again in ProviderArticle Save function.
            // We should revisit this area when we need to do performance optimization.
            List<string> imageHosts = InsideWordWebSettings.ImageHosts;
            List<ImageInfo> imageList = HtmlParser.GetImageInfos(articleText, null);
            int orderedCount = 1;
            Uri srcUri = null;
            foreach (ImageInfo anImageInfo in imageList)
            {
                ProviderPhotoRecord aRecord = new ProviderPhotoRecord();
                if(!Uri.TryCreate(anImageInfo.Src, UriKind.Absolute, out srcUri))
                {
                    errorList.Add(IWStringUtility.SuffixedNumber(orderedCount) + " photo has a bad link. Delete it and recreate it.");
                    returnValue = false;
                }
                else
                {
                    if (!aRecord.Load(srcUri))
                    {
                        aRecord = new ProviderPhotoRecord(anImageInfo);
                        aRecord.Save();
                        List<ProviderPhotoRecord> fakeThumbnailList = ProviderPhotoRecord.CreateFakeThumbnails(aRecord);
                        foreach (ProviderPhotoRecord fakeThumbnail in fakeThumbnailList)
                        {
                            fakeThumbnail.Save();
                        }
                    }

                    anArticle.AddPhoto(aRecord.Id.Value);
                }
                orderedCount++;
            }

            return returnValue;
        }
    }
}