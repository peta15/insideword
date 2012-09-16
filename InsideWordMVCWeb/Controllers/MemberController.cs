using System.Web.Mvc;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.WebProvider;
using System.Collections.Generic;
using InsideWordProvider;
using System;
using InsideWordMVCWeb.ViewModels.Member;
using InsideWordMVCWeb.Models.Annotation;
using System.Net.Mail;
using InsideWordAdvancedResource;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordResource;

namespace InsideWordMVCWeb.Controllers
{
    public partial class MemberController : BugFixController
    {
        // GET: /member/email_validation_sent
        public virtual ActionResult EmailValidationSent()
        {
            MessageVM viewModel = new MessageVM
            {
                Image = ImageLibrary.CheckEmail,
                CssClassContainer = "info",
                Message = "You will receive an e-mail shortly.  Follow the link provided in the email to validate your email.",
                Title = "Email Validation",
                LinkText = "Continue",
                LinkHref = Url.Action(MVC.Home.Index())
            };
            return View("Message", viewModel);
        }

        // GET: /member/registercomplete
        public virtual ActionResult RegisterComplete()
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            MessageVM viewModel = new MessageVM
            {
                Image = ImageLibrary.Success,
                CssClassContainer = "info",
                Message = "Welcome! Your account has been activated.  Explore our site:",
                Title = "Account Activated",
                Details = new List<string>
                {
                    "<a href='" + Url.Action( MVC.Home.Index(null, null) ) + "' class='button'>Home Page</a> Go back to the main page.",
                    "<a href='" + Url.Action( MVC.Article.ArticleEdit(null, null) ) + "' class='button'>Publish</a> Become an author!  Publish an article.",
                    "<a href='" + Url.Action( MVC.Member.Profile(currentMember.Id.Value, null) ) + "' class='button'>Profile</a> Check out your new member profile " + 
                    "where you can review your published articles and add details to show others who you are."
                }
            };

            return View("Message", viewModel);
        }

        // GET: /member/validate
        public virtual ActionResult ValidateEmail(string key)
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            List<string> errorList = new List<string>();
            ProviderIssuedKey nonceKey = new ProviderIssuedKey();
            MessageVM returnMessageVM = new MessageVM
            {
                Image = ImageLibrary.Alert,
                CssClassContainer = "failure",
                Message = "Failed to validate e-mail.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                Title = "E-mail Validation Failure",
                LinkText = "Continue",
                LinkHref = Url.Action(MVC.Home.Index()),
                Details = errorList
            };

            if (nonceKey.Load(key))
            {
                ProviderEmail anEmail = new ProviderEmail();
                if (anEmail.Load(nonceKey.Data))
                {
                    anEmail.IsValidated = true;
                    anEmail.EditDate = DateTime.UtcNow;
                    anEmail.Save();

                    if (currentMember.IsLoggedOn && currentMember.IsActive)
                    {
                        returnMessageVM = new MessageVM
                        {
                            Image = ImageLibrary.Success,
                            CssClassContainer = "info",
                            Message = "Your e-mail has been validated",
                            Title = "E-mail validated"
                        };
                    }
                    else if (currentMember.Login(key, null, false, ref errorList) == ProviderCurrentMember.LoginEnum.success)
                    {
                        returnMessageVM = new MessageVM
                        {
                            Image = ImageLibrary.Success,
                            CssClassContainer = "info",
                            Message = "Welcome! Your account has been activated.  Explore our site:",
                            Title = "Account Activated",
                            Details = new List<string>
                            {
                                "<a href='" + Url.Action( MVC.Home.Index(null, null) ) + "' class='button'>Home Page</a> Go back to the main page.",
                                "<a href='" + Url.Action( MVC.Article.ArticleEdit(null, null) ) + "' class='button'>Publish</a> Become an author!  Publish an article.",
                                "<a href='" + Url.Action( MVC.Member.Profile(currentMember.Id.Value, null) ) + "' class='button'>Profile</a> Check out your new member profile " + 
                                "where you can review your published articles and add details to show others who you are."
                            }
                        };
                    }
                }
            }

            return View("Message", returnMessageVM);
        }

        // POST: /member/delete
        public virtual ActionResult Delete(string key)
        {
            MessageVM returnMessageVM;
            ProviderIssuedKey issuedKey = new ProviderIssuedKey();

            if (issuedKey.Load(key) && !issuedKey.HasExpired && issuedKey.IsValidated)
            {
                ProviderMember aMember = new ProviderMember(issuedKey.MemberId);
                ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

                // if the person is currently logged on with the account that is being deleted then log them off
                if (currentMember.Id == aMember.Id)
                {
                    currentMember.LogOff();
                }

                aMember.Delete();

                returnMessageVM = new MessageVM
                {
                    Image = ImageLibrary.Success,
                    CssClassContainer = "success",
                    Message = "Success! Member Acount deleted.",
                    Title = "Acount Deleted",
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Home.Index())
                };
            }
            else
            {
                returnMessageVM = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "Failed to delete member account.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                    Title = "Account Deletion Failure",
                    LinkText = "Continue",
                    LinkHref = Url.Action( MVC.Home.Index() )
                };
            }

            return View("Message", returnMessageVM);
        }

        // GET: /member/ResetPasswordRequest
        public virtual ActionResult ResetPasswordRequest()
        {
            return View(new ResetPasswordRequestVM());
        }

        // POST: /member/ResetPasswordRequest
        [HttpPost]
        [CaptchaValidator]
        public virtual ActionResult ResetPasswordRequest(ResetPasswordRequestVM model, bool captchaValid)
        {
            ActionResult returnValue = null;

            if (!captchaValid)
            {
                ModelState.AddModelError( "", "Captcha incorrect.  Please re-enter captcha");
            }
            else if (ModelState.IsValid)
            {
                ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

                try
                {
                    MailAddress email = new MailAddress(model.Email);
                    if (!currentMember.LoadByEmail(email))
                    {
                        ModelState.AddModelError("", "E-mail not recognized.");
                        returnValue = View(model);
                    }
                    else
                    {
                        EmailManager.Instance.SendPasswordResetEmail(email, currentMember);
                        returnValue = RedirectToAction(MVC.Member.ResetPasswordRequestComplete());
                    }
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    ModelState.AddModelError("", "Failed to send e-mail to reset password. Please contact us to resolve the issue.");
                }
            }

            if (returnValue == null)
            {
                returnValue = View(model);
            }

            return returnValue;
        }

        public virtual ActionResult ResetPasswordRequestComplete()
        {
            var viewModel = new MessageVM
            {
                Image = ImageLibrary.CheckEmail,
                CssClassContainer = "info",
                Message = "You will receive an e-mail shortly.  Follow the link provided in the email to reset your password.",
                Title = "Reset Your Password",
                LinkText = "Continue",
                LinkHref = Url.Action(MVC.Home.Index())
            };
            return View("Message", viewModel);
        }

        // GET: /member/ChangePassword
        public virtual ActionResult ChangePassword(string issuedKey)
        {
            ActionResult returnValue = null;

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (!string.IsNullOrWhiteSpace(issuedKey))
            {   
                ProviderIssuedKey aKey = new ProviderIssuedKey();
                if (!aKey.Load(issuedKey))
                {
                    MessageVM returnMessageVM = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "failure",
                        Message = "Invalid key provided.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                        Title = "Login failure",
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index())
                    };
                    returnValue = View(MVC.Shared.Views.Message, returnMessageVM);
                }
                else
                {
                    // validate the e-mail if it wasn't already.
                    ProviderEmail anEmail = new ProviderEmail();
                    if (anEmail.Load(aKey.Data) && !anEmail.IsValidated)
                    {
                        anEmail.IsValidated = true;
                        anEmail.EditDate = DateTime.UtcNow;
                        anEmail.Save();
                    }

                    List<string> errorList = new List<string>();
                    if (currentMember.Login(issuedKey, null, false, ref errorList) == ProviderCurrentMember.LoginEnum.success)
                    {
                        MessageVM returnMessageVM = new MessageVM
                        {
                            Image = ImageLibrary.Alert,
                            CssClassContainer = "failure",
                            Message = "Failed to login.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                            Title = "Login failure",
                            LinkText = "Continue",
                            LinkHref = Url.Action(MVC.Home.Index()),
                            Details = errorList
                        };
                        returnValue = View(MVC.Shared.Views.Message, returnMessageVM);
                    }
                }
            }
            
            if(currentMember.IsLoggedOn)
            {
                ChangePasswordVM viewModel = new ChangePasswordVM
                {
                    CurrentMemberId = currentMember.Id.Value
                };
                returnValue = View(viewModel);
            }

            return returnValue;
        }

        // POST: /member/ChangePassword
        [HttpPost]
        [RestrictAccess]
        public virtual ActionResult ChangePassword(string issuedKey, ChangePasswordVM model)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            if (ModelState.IsValid)
            {
                try
                {
                    bool previousActiveState = currentMember.IsActive;
                    currentMember.Password = model.Password;
                    currentMember.EditDate = DateTime.UtcNow;
                    currentMember.Save();

                    // if this just activated someone then send them to the completion page
                    if (currentMember.IsActive && previousActiveState != currentMember.IsActive)
                    {
                        returnValue = RedirectToAction(MVC.Member.RegisterComplete());
                    }
                    else
                    {
                        returnValue = RedirectToAction(MVC.Member.ChangePasswordComplete());
                    }
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    ModelState.AddModelError("", "Failed to change password. An administrator will contact you through e-mail regarding this issue.");
                }
            }

            if (returnValue == null)
            {
                returnValue = View(model);
            }

            return returnValue;
        }


        [RestrictAccess]
        public virtual ActionResult ChangePasswordComplete()
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;

            var viewModel = new MessageVM
            {
                Image = ImageLibrary.Success,
                CssClassContainer = "success",
                Message = "Your password has been changed successfully.",
                Title = "Change Password",
                LinkText = "Continue",
                LinkHref = Url.Action(MVC.Member.Profile(currentMember.Id.Value, null))
            };
            return View("Message", viewModel);
        }

        // GET: member/profile/{memberId}/{page}
        public virtual ActionResult Profile(long memberId, int? page)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            ProviderMember aMember = new ProviderMember(memberId);
            if (aMember.IsAnonymous && !currentMember.CanEdit(aMember))
            {
                if (currentMember.Owns(aMember))
                {
                    MessageVM viewModel = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "success",
                        Message = "Your profile doesn't exist yet! Choose a password and finish <a href='" + Url.Action(MVC.Member.ChangePassword()) + "' class='button'>activating</a> your account to get one.",
                        Title = "Info",
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index(null, null))
                    };
                    return View(MVC.Shared.Views.Message, viewModel);
                }
                else
                {
                    returnValue = RedirectToAction(MVC.Error.Index(404));
                }
            }
            else
            {
                int nPage = page ?? 0;
                returnValue = View(new ProfileVM(aMember, currentMember, nPage));
            }


            return returnValue;
        }

        // GET: member/logout
        public virtual ActionResult Logout()
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            currentMember.LogOff();
            return RedirectToAction(MVC.Home.Index(null, null));
        }

    /*===========================================*
    * Account
    * ==========================================*/

        // GET: /member/Account/{memberId}
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult Account(long memberId, string issuedKey)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            
            // If we have all of the required data then try an authentication with the issued key
            if (!currentMember.IsLoggedOn && !string.IsNullOrEmpty(issuedKey))
            {
                List<string> errorList = new List<string>();
                if (currentMember.Login(issuedKey, null, false, ref errorList) != ProviderCurrentMember.LoginEnum.success)
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
                        LinkHref = Url.Action(MVC.Home.Index(null, null)),
                    };

                    returnValue = View("Message", viewModel);
                }
            }

            if (returnValue == null)
            {
                ProviderMember aMember = new ProviderMember(memberId);
                if (!currentMember.CanEdit(aMember))
                {
                    var viewModel = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "failure",
                        Message = "Authorization Failure.  You may need to login or obtain access privileges to view this page.",
                        Title = "Authorization Failure",
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index(null, null))
                    };

                    returnValue = View("Message", viewModel);
                }
                else
                {
                    returnValue = View(new AccountVM(aMember, currentMember));
                }
            }

            return returnValue;
        }

        // GET: /member/account/_GetJqGridArticleList
        [AjaxOnly]
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual JsonResult _GetJqGridArticleList(long memberId, AccountArticleFilterVM model)
        {
            ProviderMember aMember = new ProviderMember(memberId);
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (currentMember.CanEdit(aMember))
            {
                model.MemberId = memberId;
                JqGridList<ProviderArticle> jqGridList;
                if (ModelState.IsValid)
                {
                    List<ProviderArticle> articleList = ProviderArticle.LoadBy(model.GetFilter);
                    jqGridList = new JqGridList<ProviderArticle>(model.GetFilter,
                                                                articleList,
                                                                JqGridConverter.Article,
                                                                ProviderArticle.Count(model.GetFilter));
                }
                else
                {
                    jqGridList = new JqGridList<ProviderArticle>();
                }

                return Json(jqGridList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        // POST: /member/account/_EditJqGridArticle
        [AjaxOnly]
        [RestrictAccess]
        [HttpPost]
        public virtual ActionResult _EditJqGridArticle(AccountEditArticleVM model)
        {
            JqGridResponse aResponse;
            if (ModelState.IsValid)
            {
                try
                {
                    aResponse = ArticleBL.Process(model.GetJqGridEdit, ProviderCurrentMember.Instance);
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
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

        // POST: /member/validateEmail/{memberId}/{email}
        [RestrictAccess]
        [HttpPost]
        public virtual ActionResult RequestValidateEmail(string newEmail, long memberId)
        {
            ActionResult returnValue = null;
            MailAddress mailAddress = null;

            // check if email is unique and try to parse it
            // if unique (not taken) and valid format (and thus not validated) then send activation email
            if (!IWStringUtility.TryParse(newEmail, out mailAddress))
            {
                MessageVM messageModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "Invalid e-mail",
                    Title = ErrorStrings.TITLE_ERROR,
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Member.Account(memberId, null)),
                };
                returnValue = View("Message", messageModel);
            }
            else if (ProviderEmail.FindOwner(mailAddress, true) != null)
            {
                MessageVM messageModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "E-mail is already taken.",
                    Title = ErrorStrings.TITLE_ERROR,
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Member.Account(memberId, null)),
                };
                returnValue = View("Message", messageModel);
            }
            else if (!ProviderMember.Exists(memberId))
            {
                MessageVM messageModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "No member with id "+memberId,
                    Title = ErrorStrings.TITLE_ERROR,
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Member.Account(memberId, null)),
                };
                returnValue = View("Message", messageModel);
            }
            else
            {
                ProviderMember member = new ProviderMember(memberId);
                ProviderEmail anEmail = new ProviderEmail();
                anEmail.MemberId = memberId;
                anEmail.IsValidated = false;
                anEmail.CreateDate = DateTime.UtcNow;
                anEmail.EditDate = DateTime.UtcNow;
                anEmail.Email = mailAddress;
                anEmail.Save();

                EmailManager.Instance.SendActivationEmail(mailAddress, member);

                returnValue = RedirectToAction(MVC.Member.EmailValidationSent());
            }

            return returnValue;
        }
    }
}
