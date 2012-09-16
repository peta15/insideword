using System.Web.Mvc;
using System.Net.Mail;
using System.Collections.Generic;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;

using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using InsideWordMVCWeb.ViewModels.Member;
using InsideWordMVCWeb.ViewModels.Child;
using InsideWordAdvancedResource;
using System;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Controllers
{
    public partial class ChildController : BugFixController
    {
        protected const string _loginPreviousPageKey = "_loginPreviousPageKey";

        [ChildActionOnly]
        public virtual ActionResult MasterNavigationBar()
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            ActionResult returnValue = View(new MasterNavigationBarVM()
            {
                DisplayAdminTools =  currentMember.CanAccess( ProviderCurrentMember.AccessGroup.Administration )
            });
            return returnValue;
        }

        [ChildActionOnly]
        public virtual ActionResult MasterHeader()
        {
            return View(new CurrentMemberVM(ProviderCurrentMember.Instance));
        }

        [ChildActionOnly]
        public virtual ActionResult MasterLoginModal()
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            ActionResult returnValue = View(new MasterLoginModalVM()
            {
                IsLoggedOn = currentMember.IsLoggedOn,
                DisplayLoginModal = !currentMember.IsLoggedOn
            });

            return returnValue;
        }

        [ChildActionOnly]
        public virtual ActionResult Article(ArticleVM viewModel)
        {
            return View(viewModel);
        }

        [ChildActionOnly]
        public virtual ActionResult Blurb(BlurbVM articleBlurb)
        {
            return View(articleBlurb);
        }

        [ChildActionOnly]
        public virtual ActionResult Conversation(ConversationVM viewModel)
        {
            return View(viewModel);
        }

        // GET: /child/login
        [ChildActionOnly]
        public virtual ActionResult Login()
        {
            return View(new LoginVM());
        }

        // POST: /child/login
        [HttpPost]
        [AjaxOnly]
        public virtual JsonResult Login(LoginVM model)
        {
            PartialPostVM returnValue = null;

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (ModelState.IsValid)
            {
                List<string> errorList = new List<string>();
                if (currentMember.Login(model.Email,
                                    model.Password,
                                    model.RememberMe,
                                    ref errorList) == ProviderCurrentMember.LoginEnum.success)
                {
                    string previousUrl = null;
                    if (HttpContext.Request.UrlReferrer != null)
                    {
                        previousUrl = HttpContext.Request.UrlReferrer.AbsoluteUri;
                    }
                    returnValue = new PartialPostVM
                    {
                        Action = PartialPostVM.ActionType.redirect,
                        Content = previousUrl
                    };
                }
                else
                {
                    foreach (string error in errorList)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            if (returnValue == null)
            {
                returnValue = new PartialPostVM
                {
                    Action = PartialPostVM.ActionType.refresh,
                    Message = String.Empty,
                    Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.Login, (object)model)
                };
            }

            return Json(returnValue);
        }

        [ChildActionOnly]
        public virtual ActionResult LoginOpenId()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public virtual JsonResult LoginOpenId(LoginOpenIdVM model)
        {
            PartialPostVM returnValue = null;

            if (string.IsNullOrEmpty(model.openid_identifier) || !Identifier.IsValid(model.openid_identifier))
            {
                ModelState.AddModelError("", "The specified login identifier is invalid");
                returnValue = new PartialPostVM
                {
                    Action = PartialPostVM.ActionType.refresh,
                    Message = String.Empty,
                    Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.LoginOpenId)
                };
            }
            else
            {
                try
                {
                    var openid = new OpenIdRelyingParty();
                    Identifier oiProvider = Identifier.Parse(model.openid_identifier);
                    Realm currentHost = new Realm(InsideWordWebSettings.HostName);
                    Uri returnUrl = new Uri(Url.ActionAbsolute(MVC.Child.LoginOpenIdProcess()));
                    IAuthenticationRequest request = openid.CreateRequest(oiProvider, 
                                                                          currentHost,
                                                                          returnUrl);
                    // Request some additional data
                    request.AddExtension(new ClaimsRequest
                    {
                        Email = DemandLevel.Require
                    });

                    returnValue = new PartialPostVM
                    {
                        Action = PartialPostVM.ActionType.redirect,
                        Message = string.Empty,
                        Content = request.RedirectingResponse.Headers["Location"]
                    };

                    string previousUrl = null;
                    if (HttpContext.Request.UrlReferrer != null)
                    {
                        previousUrl = HttpContext.Request.UrlReferrer.AbsoluteUri;
                    }
                    Session[_loginPreviousPageKey] = previousUrl;
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    ModelState.AddModelError("", "The specified login identifier is invalid");
                    returnValue = new PartialPostVM
                    {
                        Action = PartialPostVM.ActionType.refresh,
                        Message = String.Empty,
                        Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.LoginOpenId)
                    };
                }
            }
            return Json(returnValue);
        }

        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult LoginOpenIdProcess()
        {
            OpenIdRelyingParty openid = new OpenIdRelyingParty();
            IAuthenticationResponse response = openid.GetResponse();
            ActionResult returnValue = null;

            if (response == null)
            {
                MessageVM viewModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "We could not log you in with your chosen OpenId.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                    Title = "Login failure",
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Home.Index())
                };
                returnValue = View(MVC.Shared.Views.Message, viewModel);
            }
            else
            {
                MessageVM viewModel = null;
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        ClaimsResponse sreg = response.GetExtension<ClaimsResponse>();
                        string openId = response.ClaimedIdentifier.ToString();
                        string host = response.Provider.Uri.Host;
                        MailAddress email = null;
                        if (sreg != null)
                        {
                            email = new MailAddress(sreg.Email);
                        }

                        // Create or update a member with this information.
                        MemberBL.UpdateMemberOpenId(openId, email, host);

                        ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
                        List<string> errorList = new List<string>();
                        if (currentMember.Login(openId, null, true, ref errorList) != ProviderCurrentMember.LoginEnum.success)
                        {
                            viewModel = new MessageVM
                            {
                                Image = ImageLibrary.Alert,
                                CssClassContainer = "failure",
                                Message = "We could not log you in with your chosen OpenId.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                                Title = "Login failure",
                                LinkText = "Continue",
                                LinkHref = Url.Action(MVC.Home.Index()),
                                Details = errorList
                            };
                            returnValue = View(MVC.Shared.Views.Message, viewModel);
                        }
                        else
                        {
                            string previousUrl = Session[_loginPreviousPageKey] as string;
                            if (string.IsNullOrWhiteSpace(previousUrl))
                            {
                                returnValue = RedirectToAction(MVC.Home.Index());
                            }
                            else
                            {
                                returnValue = Redirect(previousUrl);
                            }
                        }
                        break;

                    case AuthenticationStatus.Canceled:
                        viewModel = new MessageVM
                        {
                            Image = ImageLibrary.Alert,
                            CssClassContainer = "failure",
                            Message = "Login was cancelled by your OpenId provider.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                            Title = "Login failure",
                            LinkText = "Continue",
                            LinkHref = Url.Action(MVC.Home.Index())
                        };
                        returnValue = View(MVC.Shared.Views.Message, viewModel);
                        break;

                    case AuthenticationStatus.Failed:
                        InsideWordWebLog.Instance.Log.Error(response.Exception);
                        viewModel = new MessageVM
                        {
                            Image = ImageLibrary.Alert,
                            CssClassContainer = "failure",
                            Message = "We could not log you in with your chosen OpenId.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                            Title = "Login failure",
                            LinkText = "Continue",
                            LinkHref = Url.Action(MVC.Home.Index())
                        };
                        returnValue = View(MVC.Shared.Views.Message, viewModel);
                        break;
                }
            }
            return returnValue;
        }

        // GET: /child/register
        /// <summary>
        /// Register controller/view serves as both a means to register a new member and to update/activate an already existing one
        /// </summary>
        /// <param name="issuedKey">This optional value is used by InsideWord e-mails to allow someone to complete activation.</param>
        /// <returns></returns>
        [ChildActionOnly]
        public virtual ActionResult Register()
        {
            return PartialView();
        }

        // POST: /child/register
        [HttpPost]
        [AjaxOnly]
        public virtual JsonResult Register(RegisterVM model)
        {
            PartialPostVM returnValue = null;

            if (ModelState.IsValid)
            {
                try
                {
                    ProviderMember registerMember = new ProviderMember();
                    MemberBL.Save(model, ref registerMember);
                    EmailManager.Instance.SendActivationEmail(new MailAddress(model.Email), registerMember);
                    returnValue = new PartialPostVM
                    {
                        Action = PartialPostVM.ActionType.redirect,
                        Content = Url.Action(MVC.Member.EmailValidationSent())
                    };
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    ModelState.AddModelError("", "Failed to create account. An administrator will contact you through e-mail regarding this issue.");
                }
            }

            if (returnValue == null)
            {
                returnValue = new PartialPostVM
                {
                    Action = PartialPostVM.ActionType.refresh,
                    Message = string.Empty,
                    Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.Register, (object)model)
                };
            }

            return Json(returnValue);
        }

        // GET: /member/edit_personal_info/{memberId}
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        [ChildActionOnly]
        public virtual ActionResult EditPersonalInfo(long memberId)
        {
            ProviderMember aMember = new ProviderMember(memberId);
            ActionResult returnValue = PartialView(new PersonalInfoVM(aMember));
            return returnValue;
        }

        // POST: /member/edit_personal_info/{memberId}
        [HttpPost]
        [RestrictAccess]
        public virtual JsonResult EditPersonalInfo(long memberId, PersonalInfoVM model)
        {
            PartialPostVM returnValue = null;
            if (ModelState.IsValid)
            {
                try
                {
                    ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
                    ProviderMember aMember = new ProviderMember(model.MemberId);
                    if(!currentMember.CanEdit(aMember))
                    {
                        // TODO: Replace this by throwing a real HTML 401 status code
                        Redirect(Url.Action(MVC.Error.Index(401)));
                    }
                    else if (!MemberBL.SavePersonalInfo(model, aMember))
                    {
                        ModelState.AddModelError("", "Failed to update personal info. An administrator will contact you through e-mail regarding this issue.");
                    }
                    else
                    {
                        returnValue = new PartialPostVM
                        {
                            Action = PartialPostVM.ActionType.redirect,
                            Message = String.Empty,
                            Content = Request.UrlReferrer.AbsoluteUri
                        };
                    }
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    ModelState.AddModelError("", "Failed to update personal info. An administrator will contact you through e-mail regarding this issue.");
                }
            }

            if (returnValue == null)
            {
                returnValue = new PartialPostVM
                {
                    Action = PartialPostVM.ActionType.refresh,
                    Message = String.Empty,
                    Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.EditPersonalInfo, (object)model)
                };
            }

            return Json(returnValue);
        }

        // GET: /member/edit_alternate_category/{memberId}
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        [ChildActionOnly]
        public virtual ActionResult EditAlternateCategory(long memberId)
        {
            ActionResult returnValue = null;
            ProviderMember aMember = new ProviderMember(memberId);
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (currentMember.CanEdit(aMember))
            {
                returnValue = PartialView(new AlternateCategoryListVM(aMember,
                                                                      ProviderCategory.Root.Children()));
            }
            else
            {
                // TODO: Replace this by throwing a real HTML 401 status code
                returnValue = RedirectToAction(MVC.Error.Index(401));
            }

            return returnValue;
        }

        // POST: /member/edit_alternate_category/{memberId}
        [HttpPost]
        [AjaxOnly]
        [RestrictAccess]
        public virtual JsonResult EditAlternateCategory(long memberId, AlternateCategoryListVM model)
        {
            PartialPostVM returnValue = null;
            ProviderMember aMember = new ProviderMember(model.MemberId);
            if (ModelState.IsValid)
            {
                try
                {
                    ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
                    if (!currentMember.CanEdit(aMember))
                    {
                        // TODO: Replace this by throwing a real HTML 401 status code
                        Redirect(Url.Action(MVC.Error.Index(401)));
                    }
                    else if (!MemberBL.SaveAlternateCategories(model, aMember))
                    {
                        ModelState.AddModelError("", "Failed to save the alternate category changes. An administrator will contact you through e-mail regarding this issue.");
                    }
                    else
                    {
                        returnValue = new PartialPostVM
                        {
                            Action = PartialPostVM.ActionType.redirect,
                            Message = String.Empty,
                            Content = Request.UrlReferrer.AbsoluteUri
                        };
                    }
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    ModelState.AddModelError("", "Failed to save the alternate category changes. An administrator will contact you through e-mail regarding this issue.");
                }
            }

            if (returnValue == null)
            {
                model.Refresh(aMember, ProviderCategory.Root.Children());
                returnValue = new PartialPostVM
                {
                    Action = PartialPostVM.ActionType.refresh,
                    Message = String.Empty,
                    Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.EditAlternateCategory, (object)model)
                };
            }

            return Json(returnValue);
        }
    }
}
