using System;
using System.Web.Mvc;
using InsideWordMVCWeb.Models.Annotation;
using System.Collections.Generic;
using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.ViewModels.Admin;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordMVCWeb.ViewModels.Shared;

namespace InsideWordMVCWeb.Controllers
{
    public partial class AdminController : BugFixController
    {
        // GET: /Admin/
        // Returns the administrator dashboard
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.Administration)]
        public virtual ActionResult Index()
        {
            return View(new AdminIndexVM(ProviderCurrentMember.Instance));
        }

        // GET: /Admin/settings
        // Returns the administrator settings page
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.Administration)]
        public virtual ActionResult SettingsManagement()
        {
            return View(new SettingsManagementVM(InsideWordSettingsDictionary.Instance,
                                                 ProviderCategory.Root.Children(),
                                                 InsideWordWebLog.Instance));
        }

        // POST: /Admin/settings
        // Returns the administrator settings page
        [HttpPost]
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.Administration)]
        public virtual ActionResult SettingsManagement(SettingsManagementVM model)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            // Validate that the model is fine first and foremost to make sure we're not trying to work with bad data
            if (ModelState.IsValid)
            {
                if (!currentMember.CanEdit(InsideWordSettingsDictionary.Instance))
                {
                    returnValue = RedirectToAction(MVC.Error.Index(401));
                }
                else if (AdminBL.Save(model))
                {
                    returnValue = RedirectToAction(MVC.Admin.SettingsManagement());
                }
            }

            if (returnValue == null)
            {
                model.Refresh(ProviderCategory.Root.Children(), InsideWordWebLog.Instance);
                returnValue = View(model);
            }

            return returnValue;
        }

        // GET: /Admin/Member
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.MemberManagement)]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult Member()
        {
            return View();
        }

        // GET: /Admin/_GetJqGridMemberList
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.MemberManagement)]
        [AjaxOnly]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult _GetJqGridMemberList(MemberManagementFilterVM filter)
        {
            JqGridList<ProviderMember> jqGridList;
            if (ModelState.IsValid)
            {
                List<ProviderMember> memberList = ProviderMember.Load(filter);
                jqGridList = new JqGridList<ProviderMember>(filter,
                                                            memberList,
                                                            JqGridConverter.MemberManagement,
                                                            ProviderMember.Count(filter));
            }
            else
            {
                jqGridList = new JqGridList<ProviderMember>();
            }

            return Json(jqGridList, JsonRequestBehavior.AllowGet);
        }

        // POST: /Admin/_EditJqGridMember
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.MemberManagement)]
        [AjaxOnly]
        [HttpPost]
        public virtual JsonResult _EditJqGridMember(EditMemberManagementVM model)
        {
            JqGridResponse aResponse;
            if (ModelState.IsValid)
            {
                try
                {
                    aResponse = MemberBL.Process(model, ProviderCurrentMember.Instance);
                }
                catch (Exception caughtException)
                {
                    // DO NOT LOG THIS
                    aResponse = new JqGridResponse();
                    aResponse.Success = false;
                    aResponse.Message = caughtException.ToString();
                }
            }
            else
            {
                aResponse = new JqGridResponse();
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.INVALID_INPUT;
            }

            return Json(aResponse);
        }

        //
        // GET: /admin/category
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.CategoryManagement)]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult Category()
        {
            return View();
        }

        // POST: /admin/category
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.CategoryManagement)]
        [HttpPost]
        public virtual ActionResult Category(IList<NumBitVM> model)
        {
            if (ModelState.IsValid)
            {
                CategoryBL.Delete(model, ProviderCurrentMember.Instance);
                return RedirectToAction( MVC.Admin.Category() );
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete categories.");
                return View(model);
            }
        }

        // GET: /admin/category/edit/5
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.CategoryManagement)]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult CategoryEdit(long? categoryId)
        {
            ActionResult returnValue = null;

            if (categoryId.HasValue)
            {
                if (ProviderCategory.Exists(categoryId.Value))
                {
                    ProviderCategory aCategory = new ProviderCategory(categoryId.Value);
                    returnValue = View(new CategoryEditVM(aCategory));
                }
                else
                {
                    returnValue = RedirectToAction( MVC.Error.Index(404) );
                }
            }

            if (returnValue == null)
            {
                returnValue = View(new CategoryEditVM());
            }

            return returnValue;
        }

        
        // POST: /admin/category/edit/5
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.CategoryManagement)]
        [HttpPost]
        public virtual ActionResult CategoryEdit(CategoryEditVM model)
        {
            if (ModelState.IsValid)
            {
                CategoryBL.Save(model, ProviderCurrentMember.Instance);
                return RedirectToAction( MVC.Admin.Category() );
            }
            else
            {
                ModelState.AddModelError("", "Failed to edit category.");
                return View(model);
            }
        }

        // GET: /Admin/Conversation
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ConversationManagement)]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult Conversation()
        {
            return View();
        }

        // GET: /Admin/_GetJqGridCommentList
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ConversationManagement)]
        [AjaxOnly]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult _GetJqGridCommentList(CommentManagementFilterVM filter)
        {
            JqGridList<ProviderComment> jqGridList;
            if (ModelState.IsValid)
            {
                List<ProviderComment> commentList = ProviderComment.Load(filter);
                jqGridList = new JqGridList<ProviderComment>(filter,
                                                             commentList,
                                                             JqGridConverter.CommentManagement,
                                                             ProviderComment.Count(filter));
            }
            else
            {
                jqGridList = new JqGridList<ProviderComment>();
            }

            return Json(jqGridList, JsonRequestBehavior.AllowGet);
        }

        // POST: /Admin/_EditJqGridComment
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ConversationManagement)]
        [AjaxOnly]
        [HttpPost]
        public virtual ActionResult _EditJqGridComment(EditCommentManagementVM model)
        {
            JqGridResponse aResponse;
            if (ModelState.IsValid)
            {
                try
                {
                    aResponse = CommentBL.Process(model, ProviderCurrentMember.Instance);
                }
                catch (Exception caughtException)
                {
                    // DO NOT LOG THIS
                    aResponse = new JqGridResponse();
                    aResponse.Success = false;
                    aResponse.Message = caughtException.ToString();
                }
            }
            else
            {
                aResponse = new JqGridResponse();
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.INVALID_INPUT;
            }

            return Json(aResponse);
        }

        // GET: /Admin/Article
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ArticleManagement)]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult Article()
        {
            return View();
        }

        // GET: /Admin/_GetJqGridArticleList
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ArticleManagement)]
        [AjaxOnly]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult _GetJqGridArticleList(ArticleFilterVM filter)
        {
            JqGridList<ProviderArticle> jqGridList;
            if (ModelState.IsValid)
            {
                List<ProviderArticle> articleList = ProviderArticle.LoadBy(filter);
                jqGridList = new JqGridList<ProviderArticle>(filter,
                                                            articleList,
                                                            JqGridConverter.ArticleManagement,
                                                            ProviderArticle.Count(filter));
            }
            else
            {
                jqGridList = new JqGridList<ProviderArticle>();
            }

            return Json(jqGridList, JsonRequestBehavior.AllowGet);
        }

        // POST: /Admin/_EditJqGridArticle
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ArticleManagement)]
        [AjaxOnly]
        [HttpPost]
        public virtual JsonResult _EditJqGridArticle(JqGridEditArticleVM model)
        {
            JqGridResponse aResponse;
            if ( ModelState.IsValid )
            {
                try
                {
                    aResponse = ArticleBL.Process(model, ProviderCurrentMember.Instance);
                }
                catch (Exception caughtException)
                {
                    // DO NOT LOG THIS
                    aResponse = new JqGridResponse();
                    aResponse.Success = false;
                    aResponse.Message = caughtException.ToString();
                }
            }
            else
            {
                aResponse = new JqGridResponse();
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.INVALID_INPUT;
            }

            return Json(aResponse);
        }

        // GET: /Admin/RefreshArticle
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ArticleManagement)]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult RefreshArticle()
        {
            return PartialView();
        }

        // POST: /Admin/RefreshArticle
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ArticleManagement)]
        [AjaxOnly]
        [HttpPost]
        public virtual JsonResult RefreshArticle(RefreshArticleVM model)
        {
            AjaxReturnMsgVM returnValue = null;

            AsyncRefreshArticleManager.Instance.AddBy(AsyncRefreshArticleManager.LoadEnum.All);

            if(returnValue == null)
            {
                returnValue = new AjaxReturnMsgVM
                {
                    StatusCode = AjaxReturnMsgVM.Status.success,
                    StatusMessage = string.Empty,
                    Content = string.Empty
                };
            }

            return Json(returnValue);
        }

        // GET: /Admin/_RefreshArticleStatus
        [RestrictAccess(AccessGroup = ProviderCurrentMember.AccessGroup.ArticleManagement)]
        [AjaxOnly]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult RefreshArticleStatus()
        {
            AjaxReturnMsgVM returnMessage = new AjaxReturnMsgVM
            {
                StatusCode = AjaxReturnMsgVM.Status.failure,
                StatusMessage = string.Empty,
                Content = string.Empty
            };

            if (AsyncRefreshArticleManager.Instance.IsBusy)
            {
                returnMessage.StatusCode = AjaxReturnMsgVM.Status.success;
                returnMessage.StatusMessage = AsyncRefreshArticleManager.Instance.Progress.ToString() + " remaining";
                string errorHtml = string.Empty;
                if (AsyncRefreshArticleManager.Instance.ErrorList.Count > 0)
                {
                    errorHtml = "<ol><li>" + string.Join("</li>\n<li>", AsyncRefreshArticleManager.Instance.ErrorList) + "</li><ol>";
                }
                returnMessage.Content = errorHtml;
            }
            else if (AsyncRefreshArticleManager.Instance.Completed)
            {
                returnMessage.StatusCode = AjaxReturnMsgVM.Status.successAndStop;
                returnMessage.StatusMessage = AsyncRefreshArticleManager.Instance.Progress.ToString() + " remaining";
                string errorHtml = "no errors encountered.";
                if (AsyncRefreshArticleManager.Instance.ErrorList.Count > 0)
                {
                    errorHtml = "<ol><li>" + string.Join("</li>\n<li>", AsyncRefreshArticleManager.Instance.ErrorList) + "</li><ol>";
                }
                returnMessage.Content = errorHtml;
                AsyncRefreshArticleManager.Instance.Clear();
            }

            return Json(returnMessage, JsonRequestBehavior.AllowGet);
        }
    }
}
