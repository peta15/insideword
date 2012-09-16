using System.Web.Mvc;
using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.ViewModels.Article;
using System.Collections.Generic;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.Models.BusinessLogic;
using JelleDruyts.Mollom.Client;
using InsideWordResource;

namespace InsideWordMVCWeb.Controllers
{
    public partial class ArticleController : BugFixController
    {
        // GET: /Article/5
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult ArticleDetails(long articleId)
        {
            ActionResult returnValue = null;

            if (!ProviderArticle.Exists(articleId))
            {
                returnValue = RedirectToAction(MVC.Error.Index(404));
            }
            else
            {
                ProviderArticle anArticle = new ProviderArticle(articleId);

                if (anArticle.IsPublished || ProviderCurrentMember.Instance.CanEdit(anArticle))
                {
                    // increment view count and don't update edit date when saving article
                    anArticle.ViewCount += 1;
                    anArticle.Save();

                    // Send a shadow vote
                    ProviderArticleVote shadowVote = new ProviderArticleVote();
                    shadowVote.IsShadowVote = true;
                    shadowVote.ArticleId = anArticle.Id.Value;
                    shadowVote.Save();

                    returnValue = View(new DetailsVM(anArticle, ProviderCurrentMember.Instance));
                }
                else
                {
                    returnValue = RedirectToAction(MVC.Error.Index(404));
                }
            }

            return returnValue;
        }

        // GET: /article/new
        // GET: /article/edit/5
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult ArticleEdit(long? articleId, string issuedKey)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            // If we have all of the required data then try an authentication with the issued key
            if (!currentMember.IsLoggedOn &&  !string.IsNullOrEmpty(issuedKey))
            {
                List<string> errorList = new List<string>();
                if (currentMember.Login(issuedKey, null, false, ref errorList) != ProviderCurrentMember.LoginEnum.success )
                {
                    // failed for whatever reason
                    var viewModel = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "failure",
                        Message = "Failed to login: ",
                        Details = errorList,
                        Title = ErrorStrings.TITLE_ERROR,
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index()),
                    };
                    
                    returnValue = View("Message", viewModel);
                }
            }

            if(returnValue == null)
            {
                ProviderArticle anArticle = null;
                if (articleId.HasValue)
                {
                    anArticle = new ProviderArticle(articleId.Value);
                }
                else
                {
                    anArticle = new ProviderArticle();
                }

                if (!currentMember.CanEdit(anArticle))
                {
                    returnValue = RedirectToAction(MVC.Error.Index(401));
                }
                else
                {
                    returnValue = PartialView(new ArticleEditVM(anArticle));
                }
            }

            return returnValue;
        }

        [ChildActionOnly]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult ArticleEditor(long? articleId)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            if (!articleId.HasValue)
            {
                if (!currentMember.IsLoggedOn)
                {
                    // Tried as hard as I could but we can't seem to use Outputcache for this controller
                    // so we will cache it our way with a static.
                    if (InsideWordWebStaticCache.Instance.NewArticleEditorView == null)
                    {
                        ArticleEditorVM viewModel = new ArticleEditorVM();
                        viewModel.Parse(ProviderCurrentMember.Instance, ProviderCategory.Root.Children());
                        InsideWordWebStaticCache.Instance.NewArticleEditorView = PartialView(viewModel);
                    }
                    returnValue = InsideWordWebStaticCache.Instance.NewArticleEditorView;
                }
                else
                {
                    ArticleEditorVM viewModel = new ArticleEditorVM();
                    viewModel.Parse(ProviderCurrentMember.Instance, ProviderCategory.Root.Children());
                    returnValue = PartialView(viewModel);
                }
            }
            else
            {
                ProviderArticle anArticle = new ProviderArticle(articleId.Value);
                ArticleEditorVM viewModel = new ArticleEditorVM();
                viewModel.Parse(anArticle, ProviderCurrentMember.Instance, ProviderCategory.Root.Children());
                returnValue = PartialView(viewModel);
            }

            return returnValue;
        }

        // POST: /article/editor
        // POST: /article/editor/5
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)] // set to false so that html can be submitted through tinyMCE
        public virtual ActionResult ArticleEditor(long? articleId, ArticleEditorVM model, string submitType)
        {
            model.SaveButton = submitType;

            PartialPostVM returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            ProviderArticle anArticle;
            if (articleId.HasValue)
            {
                anArticle = new ProviderArticle(articleId.Value);
            }
            else
            {
                anArticle = new ProviderArticle();
            }

            List<string> errorList = new List<string>();

            // Validate that the model is fine first and foremost to make sure we're not trying to work with bad data
            if (ModelState.IsValid)
            {
                ContentCheck result = null;
                if (!currentMember.IsSuperAdmin && anArticle.IsNew)
                {
                    string email = string.Empty;
                    string domain = string.Empty;

                    if (currentMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.Email))
                    {
                        email = currentMember.Emails[0].Email.Address;
                    }
                    else if (!string.IsNullOrWhiteSpace(model.ArticleEmail))
                    {
                        email = model.ArticleEmail;
                    }

                    if (currentMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.Domain))
                    {
                        domain = currentMember.Domains[0].Domain.AbsoluteUri;
                    }

                    // The mollom client crashes if passed in nbsp so strip those before sending it over
                    string cleanedArticleBody = HtmlParser.StripSpecialChars(model.ArticleBody);
                    MollomClient client = new MollomClient(InsideWordWebSettings.MollomPrivateKey, InsideWordWebSettings.MollomPublicKey);
                    result = client.CheckContent(model.Title, cleanedArticleBody,
                                                                currentMember.DisplayAdministrativeName,
                                                                email,
                                                                domain,
                                                                HttpContext.Request.UserHostAddress);
                }

                if (result != null && result.Classification == ContentClassification.Spam)
                {
                    ModelState.AddModelError("", "Your article has been blocked as spam.");
                }
                else if (result != null && result.Quality < InsideWordWebSettings.MollomArticleQuality)
                {
                    ModelState.AddModelError("", "The quality of your article is too low. Try improving things such as spelling and grammar.");
                }
                else if (!currentMember.CanEdit(anArticle))
                {
                    returnValue = new PartialPostVM
                    {
                        Action = PartialPostVM.ActionType.redirect,
                        Message = string.Empty,
                        Content = Url.Action(MVC.Error.Index(401))
                    };
                }
                else if (ArticleBL.Save(model, anArticle, ProviderCurrentMember.Instance, ref errorList) &&
                         (model.SaveState == ArticleEditorVM.SaveStates.DraftAndPreview || model.SaveState == ArticleEditorVM.SaveStates.Published))
                {
                    returnValue = new PartialPostVM
                    {
                        Action = PartialPostVM.ActionType.redirect,
                        Message = string.Empty,
                        Content = Url.Action(MVC.Article.ArticleDetails(anArticle.Id.Value))
                    };
                }
            }

            if (returnValue == null)
            {
                foreach (string error in errorList)
                {
                    ModelState.AddModelError("", error);
                }

                model.Refresh(anArticle, currentMember, ProviderCategory.Root.Children());
                returnValue = new PartialPostVM
                {
                    Action = PartialPostVM.ActionType.refresh,
                    Message = string.Empty,
                    Content = ControllerExtension.RenderPartialViewToString(this, MVC.Article.Views.ArticleEditor, (object)model)
                };
            }

            return Json(returnValue);
        }
    }
}
