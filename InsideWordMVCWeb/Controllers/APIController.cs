using System.Web.Mvc;
using InsideWordMVCWeb.ViewModels.Shared;
using System;
using InsideWordResource;
using InsideWordProvider;
using InsideWordMVCWeb.Models.Utility;
using System.Net;
using HtmlAgilityPack;
using InsideWordMVCWeb.Models.WebProvider;
using System.Collections.Generic;
using InsideWordMVCWeb.ViewModels.API;
using InsideWordMVCWeb.Models.BusinessLogic;
using System.Web.Caching;
using System.Web;

namespace InsideWordMVCWeb.Controllers
{
    public partial class APIController : BugFixController
    {
        /// <summary>
        /// API control to help debug plugins on external systems.
        /// </summary>
        /// <param name="text">string with important debug info</param>
        /// <returns>a json to indicate if the message was received successfully</returns>
        [HttpPost]
        public virtual JsonResult LogInfo(string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && text.Length > 2)
            {
                text = Uri.UnescapeDataString(text);
                InsideWordWebLog.Instance.Buffer("APILOGINFO - " + HttpContext.Request.UserHostAddress, text);
            }
            ApiMsgVM returnMessage = new ApiMsgVM();
            return Json(returnMessage);
        }

        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult Ping()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        static protected long counter = 0;
        /// <summary>
        /// Test API control to help developers see if they can connect properly to our server with a post.
        /// </summary>
        /// <param name="start">value to start the incrementing value at</param>
        /// <returns>a json containing the incremented value of the global test counter.</returns>
        [HttpPost]
        public virtual JsonResult Incr(long? start)
        {
            counter = (start.HasValue) ? start.Value : counter + 1;

            return Json(counter.ToString());
        }

        [HttpPost]
        public virtual JsonResult DomainIdentificationRequest(string domainAddress)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "DomainIdentificationRequest(" + domainAddress + ")");
            ApiMsgVM returnMessage = new ApiMsgVM(1);

            Uri domainUri = null;

            if (!IWStringUtility.TryUrlDecode(domainAddress, out domainAddress) ||
                !Uri.TryCreate(domainAddress, UriKind.Absolute, out domainUri))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = domainAddress + " is an invalid uri";
            }
            else
            {
                ProviderDomain aDomain = new ProviderDomain();
                ProviderIssuedKey issuedKey = new ProviderIssuedKey();
                ProviderMember aMember = new ProviderMember();

                if (aDomain.Load(domainUri.AbsoluteUri))
                {
                    aMember.Load(aDomain.MemberId);
                }
                else
                {
                    // Domain doesn't exist already so create it and a member
                    aMember.CreateDate = DateTime.UtcNow;
                    aMember.EditDate = DateTime.UtcNow;
                    aMember.Save();

                    aDomain.CreateDate = DateTime.UtcNow;
                    aDomain.EditDate = DateTime.UtcNow;
                    aDomain.Domain = domainUri;
                    aDomain.IsValidated = false;
                    aDomain.MemberId = aMember.Id.Value;
                    aDomain.Save();
                }

                issuedKey.LoadOrCreate(aMember.Id.Value, domainUri.AbsoluteUri, true, 1, false);

                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                returnMessage.StatusMessage = "Success";
                returnMessage.Content = issuedKey.IssuedKey;
            }
            InsideWordWebLog.Instance.Buffer(from, "Done DomainIdentificationRequest - " + returnMessage);
            return Json(returnMessage);
        }

        [HttpPost]
        public virtual JsonResult DelayedVisitRequest(string pageAddress, int secDelay)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "DelayedVisitRequest(" + pageAddress + ", " + secDelay + ")");
            ApiMsgVM returnMessage = new ApiMsgVM
            {
                StatusCode = (int)ApiMsgVM.StatusEnum.success,
                StatusMessage = "Successfully queued the visit request",
                Content = String.Empty
            };

            Uri pageUri = null;
            if (!IWStringUtility.TryUrlDecode(pageAddress, out pageAddress) ||
                !Uri.TryCreate(pageAddress, UriKind.Absolute, out pageUri))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = pageAddress + " is an invalid uri";
            }
            else
            {
                AsyncVisitManager.Add(pageUri, secDelay);
            }
            InsideWordWebLog.Instance.Buffer(from, "Done DelayedVisitRequest - " + returnMessage);
            return Json(returnMessage);
        }

        [HttpPost]
        public virtual JsonResult DomainIdentification(string domainAddress, string subFolder)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "DomainIdentification(" + domainAddress + ", " + subFolder + ")");
            ApiMsgVM returnMessage = new ApiMsgVM((int)ApiMsgVM.StatusEnum.failure);

            string subFolderDecoded = null;
            IWStringUtility.TryUrlDecode(subFolder, out subFolderDecoded, "");

            Uri domainUri = null;
            Uri pathUri = null;
            if (!IWStringUtility.TryUrlDecode(domainAddress, out domainAddress) ||
                !Uri.TryCreate(domainAddress, UriKind.Absolute, out domainUri))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = domainAddress + " is an invalid uri";
            }
            else if (!IWStringUtility.TryUriConcat(domainUri, subFolderDecoded, out pathUri))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = domainUri.AbsoluteUri + " and " + subFolder + " form an invalid uri";
            }
            else
            {
                ProviderDomain aDomain = new ProviderDomain();
                ProviderIssuedKey issuedKey = new ProviderIssuedKey();
                ProviderMember aMember = new ProviderMember();

                if (!aDomain.Load(domainUri.AbsoluteUri))
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = domainUri.AbsoluteUri
                                                +" does not exist in our system. Use "
                                                +Url.Action(MVC.API.DomainIdentificationRequest())
                                                +" to request a key and identify yourself first.";
                }
                else if(!aMember.Load(aDomain.MemberId))
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = "The member associated with this domain, "
                                                +domainUri.AbsoluteUri
                                                +", does not exist. Contact support to resolve this issue.";
                }
                else if (!issuedKey.LoadBy(aMember.Id.Value, domainUri.AbsoluteUri, true, 1))
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = "Your issued key has been used up already or was never issued. Use "
                                                + Url.Action(MVC.API.DomainIdentificationRequest())
                                                + " to request a new key for identification.";
                }
                else
                {
                    // all the data is good and we're ready to check if the key has been placed in the correct uri.
                    bool isFetchSuccess = false;
                    string htmlPage = null;
                    HtmlDocument htmlDoc = new HtmlDocument();
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            // TODO: DOS attack is possible here by sending us to a page with a gig of data.
                            // put some sort of precautionary check here to avoid loading too much data.
                            htmlPage = client.DownloadString(pathUri.AbsoluteUri);
                        }
                        htmlDoc.LoadHtml(htmlPage);
                        isFetchSuccess = true;
                    }
                    catch (Exception caughtException)
                    {
                        returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                        returnMessage.StatusMessage = "Failed to read the webpage at " + pathUri.AbsoluteUri;
                        isFetchSuccess = false;
                    }

                    if (isFetchSuccess)
                    {
                        HtmlNode embeddedIssuedKey = htmlDoc.GetElementbyId(issuedKey.IssuedKey);
                        if (embeddedIssuedKey == null ||
                            embeddedIssuedKey.Name.CompareTo("input") != 0)
                        {
                            returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                            returnMessage.StatusMessage = "Could not find hidden input tag with id containing the issued key at page " + pathUri.AbsoluteUri;
                        }
                        else
                        {
                            //we found it so let's validate the domain and return the issued keys
                            aDomain.IsValidated = true;
                            aDomain.EditDate = DateTime.UtcNow;
                            aDomain.Save();

                            returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                            returnMessage.StatusMessage = "You have been successfully validated. Here are the issued keys for this month and next months. Do not share these with anyone.";
                            returnMessage.Content = aMember.CurrentMonthIssuedKey.IssuedKey + "," + aMember.NextMonthIssuedKey.IssuedKey;

                            // decommission the issued key
                            issuedKey.TryDecommission();
                        }
                    }
                }
            }
            InsideWordWebLog.Instance.Buffer(from, "Done DomainIdentification - " + returnMessage);
            return Json(returnMessage);
        }

        [HttpPost]
        public virtual JsonResult Login(string issuedKey, string issuedKey2)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "Login(" + issuedKey + ", "+issuedKey2+")");
            ApiMsgVM returnMessage = new ApiMsgVM((int)ApiMsgVM.StatusEnum.failure);

            issuedKey = Uri.UnescapeDataString(issuedKey);

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            List<string> errorList = new List<string>();
            switch(currentMember.Login(issuedKey, null, false, ref errorList))
            {
                case ProviderCurrentMember.LoginEnum.success:
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                    returnMessage.StatusMessage = "Logged in successfully.";
                    break;

                case ProviderCurrentMember.LoginEnum.unknown:
                case ProviderCurrentMember.LoginEnum.invalid:
                case ProviderCurrentMember.LoginEnum.expired:
                    if( !IWStringUtility.TryUrlDecode(issuedKey2, out issuedKey2))
                    {
                        returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                        returnMessage.StatusMessage = "Failed to parse the second issued key";
                    }
                    else if (currentMember.Login(issuedKey2, null, false, ref errorList) == ProviderCurrentMember.LoginEnum.success)
                    {
                        returnMessage.StatusCode = 2;
                        returnMessage.StatusMessage = "Logged in with key change.";
                        returnMessage.Content = returnMessage.Content = currentMember.CurrentMonthIssuedKey.IssuedKey + "," + currentMember.NextMonthIssuedKey.IssuedKey;
                    }
                    else
                    {
                        returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                        returnMessage.StatusMessage = "Failed to login for the following reasons: "+string.Join(", ", errorList);
                    }
                    break;

                case ProviderCurrentMember.LoginEnum.banned:
                    returnMessage.StatusCode = 3;
                    returnMessage.StatusMessage = "Failed to login for the following reasons: "+string.Join(", ", errorList);
                    break;

                default:
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = "Failed to login for the following reasons: "+string.Join(", ", errorList);
                    break;
            }
            InsideWordWebLog.Instance.Buffer(from, "Done Login - " + returnMessage);
            return Json(returnMessage);
        }

        [HttpPost]
        public virtual JsonResult PublishArticle(APIArticleVM model)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "PublishArticle( "+model+" )");
            ApiMsgVM returnMessage = new ApiMsgVM( (int)ApiMsgVM.StatusEnum.failure );
            
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            if (!currentMember.IsLoggedOn)
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "You must be logged in to use this command. Use the API function " + Url.Action(MVC.API.Login()) + " to login.";
            }
            else if (!ModelState.IsValid)
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "Could not parse the data.";
            }
            else if (string.IsNullOrWhiteSpace(model.Title) ||
                     string.IsNullOrWhiteSpace(model.Text))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "title and text cannot be blank.";
            }
            else
            {
                ProviderArticle anArticle = null;
                ProviderAlternateArticleId altId = new ProviderAlternateArticleId();

                if (model.Id.HasValue)
                {
                    anArticle = new ProviderArticle(model.Id.Value);
                }
                else if (!string.IsNullOrWhiteSpace(model.AltId) &&
                        altId.Load(model.AltId, currentMember.Id.Value))
                {
                    anArticle = new ProviderArticle(altId.ArticleId);
                }
                else
                {
                    anArticle = new ProviderArticle();
                }

                if (!model.CategoryId.HasValue && anArticle.IsNew)
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = "New articles must have a Category Id";
                }
                else if (model.CategoryId.HasValue && !ProviderCategory.Exists(model.CategoryId.Value))
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = "The category id " + model.CategoryId + " does not exist.";
                }
                else if (!currentMember.CanEdit(anArticle))
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                    returnMessage.StatusMessage = "You do not have the rights to edit article with InsideWord id " + anArticle.Id;
                    if (string.IsNullOrWhiteSpace(model.AltId))
                    {
                        returnMessage.StatusMessage += " and alternate id " + model.AltId;
                    }
                }
                else
                {
                    if (anArticle.IsNew)
                    {
                        anArticle.CreateDate = DateTime.UtcNow;
                    }

                    // The edit date is controlled by InsideWord and not the ones using the API
                    // this is because edit date is actually rather important.
                    anArticle.EditDate = DateTime.UtcNow;

                    if (model.IsPublished.HasValue)
                    {
                        anArticle.IsPublished = model.IsPublished.Value;
                    }
                    else if (anArticle.IsNew)
                    {
                        anArticle.IsPublished = true;
                    }

                    anArticle.Blurb = model.Blurb;
                    anArticle.Title = model.Title;
                    anArticle.RawText = model.Text;
                    anArticle.MemberId = currentMember.Id.Value;

                    anArticle.RemoveAllCategories();
                    if (model.CategoryId.HasValue)
                    {
                        anArticle.AddCategory(model.CategoryId.Value);
                    }
                    else
                    {
                        anArticle.AddCategory(InsideWordSettingsDictionary.Instance.DefaultCategoryId.Value);
                    }

                    if (model.AlternateCategoryId.HasValue)
                    {
                        List<ProviderAlternateCategoryId> altCategoryList = currentMember.AlternateCategoryList;
                        ProviderAlternateCategoryId alternateCategory = null;
                        if (altCategoryList.Exists(altCat => altCat.AlternateId == model.AlternateCategoryId))
                        {
                            alternateCategory = altCategoryList.Find(altCat => altCat.AlternateId == model.AlternateCategoryId);
                        }
                        else
                        {
                            // Doesn't seem to exist. This must be a new category we don't have any info on yet because the update from the client hasn't occurred yet.
                            // Save this as an incomplete alternate category
                            alternateCategory = new ProviderAlternateCategoryId();
                            alternateCategory.AlternateId = model.AlternateCategoryId.Value;
                            alternateCategory.MemberId = currentMember.Id.Value;
                            alternateCategory.Save();
                        }
                        anArticle.RemoveAllAlternateCategories();
                        anArticle.AddAlternateCategory(alternateCategory.Id.Value);
                    }

                    // Associate the photos, but don't bother returning any errors.
                    List<string> errorList = new List<string>();
                    ArticleBL.AssociatePhotos(model.Text, anArticle, ref errorList);
                    if (errorList.Count > 0)
                    {
                        InsideWordWebLog.Instance.Log.Debug(string.Join(", ", errorList));
                    }

                    anArticle.Save();

                    // now that the article is saved check if the model came in with an alternate id and if it did then try and create and attach.
                    if (!string.IsNullOrWhiteSpace(model.AltId) &&
                        ProviderAlternateArticleId.Exists(model.AltId, currentMember.Id.Value) == null)
                    {
                        altId.MemberId = currentMember.Id.Value;
                        altId.AlternateId = model.AltId;
                        altId.ArticleId = anArticle.Id.Value;
                        altId.Save();
                    }

                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                    returnMessage.StatusMessage = "Article has been submitted successfully.";
                    returnMessage.Content = anArticle.Id.ToString();
                }
            }
            InsideWordWebLog.Instance.Buffer(from, "Done PublishArticle - " + returnMessage);
            return Json(returnMessage);
        }

        [HttpPost]
        public virtual JsonResult ChangeArticleState(long articleId, InsideWordProvider.ProviderArticle.ArticleState state)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "ChangeArticleState(" + articleId + ", " + state.ToString() + ")");
            ApiMsgVM returnMessage = new ApiMsgVM( (int)ApiMsgVM.StatusEnum.failure );

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            ProviderArticle anArticle = new ProviderArticle();
            if (!currentMember.IsLoggedOn)
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "You must be logged in to use this command. Use the API function " + Url.Action(MVC.API.Login()) + " to login.";
            }
            else if (!anArticle.Load(articleId))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "Could not change status. No article with id "+articleId;
                returnMessage.Content = String.Empty;
            }
            else if (!currentMember.CanEdit(anArticle))
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "Could not change status. Member does not have the rights to edit article " + articleId;
                returnMessage.Content = String.Empty;
            }
            else
            {
                bool returnMessageIsSet = false;
                anArticle.EditDate = DateTime.UtcNow;
                switch (state)
                {
                    case ProviderArticle.ArticleState.delete:
                        anArticle.IsHidden = true;
                        anArticle.IsPublished = false;
                        anArticle.Save();
                        break;
                    case ProviderArticle.ArticleState.flagged:
                        anArticle.IsHidden = true;
                        anArticle.IsPublished = false;
                        anArticle.Save();
                        break;
                    case ProviderArticle.ArticleState.hidden:
                        anArticle.IsHidden = true;
                        anArticle.IsPublished = false;
                        anArticle.Save();
                        break;
                    case ProviderArticle.ArticleState.draft:
                        anArticle.IsPublished = false;
                        anArticle.Save();
                        break;
                    case ProviderArticle.ArticleState.publish:
                        anArticle.IsPublished = true;
                        anArticle.Save();
                        break;
                    default:
                        returnMessageIsSet = true;
                        returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                        returnMessage.StatusMessage = "Unknown status update "+state+" for article.";
                        returnMessage.Content = String.Empty;
                        break;
                }

                if (!returnMessageIsSet)
                {
                    returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                    returnMessage.StatusMessage = "Successfully updated article " + articleId + " status'.";
                    returnMessage.Content = String.Empty;
                }
            }
            InsideWordWebLog.Instance.Buffer(from, "Done ChangeArticleState - " + returnMessage);
            return Json(returnMessage);
        }

        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        [OutputCache(Duration=1, VaryByParam="articleId")]
        public virtual JsonResult ArticleRank(long articleId)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "ArticleRank(" + articleId + ")");
            int? rank = null;
            if (HttpContext.Cache["_articleRankDictKey"] == null)
            {
                Dictionary<long, int> rankDict = ProviderArticleScore.GetRankDict(ProviderArticleScore.ScoreTypeEnum.SparrowRank);
                DateTime expiration = DateTime.UtcNow.AddHours(2);
                HttpContext.Cache.Add("_articleRankDictKey",
                                              rankDict,
                                              null,
                                              expiration,
                                              Cache.NoSlidingExpiration,
                                              CacheItemPriority.Low,
                                              null);
            }
            else
            {
                Dictionary<long, int> rankDict = HttpContext.Cache["_articleRankDictKey"] as Dictionary<long, int>;
                int temp = -1;
                if(rankDict.TryGetValue(articleId, out temp))
                {
                    rank = temp;
                }
            }
            ApiMsgVM returnMessage = new ApiMsgVM
            {
                StatusCode = (int)ApiMsgVM.StatusEnum.success,
                StatusMessage = "Success",
                Content = rank.ToString()
            };
            InsideWordWebLog.Instance.Buffer(from, "Done ArticleRank - " + returnMessage);
            return Json(returnMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This function returns a given category and all descendants in a nested JSON format
        /// </summary>
        /// GET: /api/category/5
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult Category(long? categoryId)
        {
            ProviderCategory aCategory = new ProviderCategory();
            if (!categoryId.HasValue || !aCategory.Load(categoryId.Value))
            {
                aCategory = ProviderCategory.Root;
            }

            return Json(new CategoryVM(aCategory), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Api function that takes member data, updates their account information with it and then returns 
        /// back all their account info with any changes that may have been made by the server.
        /// </summary>
        /// GET: /api/account_sync
        [HttpPost]
        public virtual JsonResult AccountSync(MemberDataVM memberData)
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "AccountSync()");

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            MemberServerDataVM returnMessage = new MemberServerDataVM();

            if (!currentMember.IsLoggedOn)
            {
                returnMessage.StatusCode = (int)MemberServerDataVM.StatusEnum.failure;
                returnMessage.StatusMessage = "You must be logged in to use this command. Use the API function " + Url.Action(MVC.API.Login()) + " to login.";
            }
            else
            {
                ApiBL.UpdateAccount(currentMember, memberData);
                List<AlternateCategoryMapVM> Map = ApiBL.MapCategories(currentMember, memberData.MemberToIWMap);
                returnMessage = new MemberServerDataVM(currentMember, Map, ProviderCategory.Root);
                returnMessage.ProfileLink = Url.ActionAbsolute(MVC.Member.Profile(currentMember.Id.Value, null));
                returnMessage.AccountLink = Url.ActionAbsolute(MVC.Member.Account(currentMember.Id.Value, null));
            }

            InsideWordWebLog.Instance.Buffer(from, "Done AccountSync");

            return Json(returnMessage);
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult ProfileLink()
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "ProfileLink()");
            ApiMsgVM returnMessage = new ApiMsgVM
            {
                StatusCode = (int)ApiMsgVM.StatusEnum.failure,
                StatusMessage = String.Empty,
                Content = String.Empty
            };

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (!currentMember.IsLoggedOn)
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "You must be logged in to use this command. Use the API function " + Url.Action(MVC.API.Login()) + " to login.";
            }
            else
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                returnMessage.Content = Url.ActionAbsolute(MVC.Member.Profile(currentMember.Id.Value, null));
            }

            InsideWordWebLog.Instance.Buffer(from, "Done ProfileLink - " + returnMessage);
            return Json(returnMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        [OutputCache(Duration = 3600, VaryByParam = "none")]
        public virtual JsonResult DefaultCategoryId()
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "DefaultCategoryId()");
            ApiMsgVM returnMessage = new ApiMsgVM
            {
                StatusCode = (int)ApiMsgVM.StatusEnum.success,
                StatusMessage = "Success",
                Content = InsideWordSettingsDictionary.Instance.DefaultCategoryId.ToString()
            };
            InsideWordWebLog.Instance.Buffer(from, "Done DefaultCategoryId - " + returnMessage);
            return Json(returnMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult IssuedKeyRequest()
        {
            string from = "APILOGINFO - " + HttpContext.Request.UserHostAddress;
            InsideWordWebLog.Instance.Buffer(from, "IssuedKeyRequest()");
            ApiMsgVM returnMessage = new ApiMsgVM((int)ApiMsgVM.StatusEnum.failure);

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (!currentMember.IsLoggedOn)
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.failure;
                returnMessage.StatusMessage = "You must be logged in to use this command. Use the API function " + Url.Action(MVC.API.Login()) + " to login.";
            }
            else
            {
                returnMessage.StatusCode = (int)ApiMsgVM.StatusEnum.success;
                returnMessage.StatusMessage = "You have been successfully validated. Here are the issued keys for this month and next months. Do not share these with anyone.";
                returnMessage.Content = currentMember.CurrentMonthIssuedKey.IssuedKey + "," + currentMember.NextMonthIssuedKey.IssuedKey;
            }
            InsideWordWebLog.Instance.Buffer(from, "Done IssuedKeyRequest - " + returnMessage);
            return Json(returnMessage, JsonRequestBehavior.AllowGet);
        }
    }
}
