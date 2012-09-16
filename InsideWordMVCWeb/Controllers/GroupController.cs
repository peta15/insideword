using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InsideWordMVCWeb.ViewModels.Group;
using InsideWordProvider;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Controllers
{
    public partial class GroupController : BugFixController
    {
        // GET: /Group/
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Details(long groupId)
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            return View(new GroupDetailVM(new ProviderGroup(groupId), currentMember));
        }

        public virtual ActionResult Register()
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (!currentMember.IsActive)
            {
                MessageVM messageModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "You must first active an account with us before you can register a Group",
                    Title = ErrorStrings.TITLE_WARNING,
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Home.Index()),
                };

                returnValue = View("Message", messageModel);
            }
            else
            {
                returnValue = View();
            }

            return returnValue;
        }

        [HttpPost]
        public virtual ActionResult Register(GroupRegisterVM model)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            if (!currentMember.IsActive)
            {
                MessageVM messageModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "You must first activate an account with us before you can register a Group",
                    Title = ErrorStrings.TITLE_WARNING,
                    LinkText = "Continue",
                    LinkHref = Url.Action(MVC.Home.Index()),
                };

                returnValue = View("Message", messageModel);
            }
            else if (ModelState.IsValid)
            {
                ProviderGroup aGroup = new ProviderGroup();
                if (!GroupBL.Save(model, aGroup))
                {
                    var viewModel = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "failure",
                        Message = "Failed to activate group.  Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                        Title = "Group registration failure",
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index())
                    };
                    returnValue = View("Message", viewModel);
                }
                else
                {
                    var viewModel = new MessageVM
                    {
                        Image = ImageLibrary.Success,
                        CssClassContainer = "info",
                        Message = "Your Group has been created:",
                        Title = "Group created",
                        Details = new List<string>
                        {
                            "<a href='" + Url.Action( MVC.Group.Details(aGroup.Id.Value) ) + "' class='button'>View</a>  your group",
                            "<a href='" + Url.Action( MVC.Group.Manage(aGroup.Id.Value) ) + "' class='button'>Manage</a> your group"
                        }
                    };
                    returnValue = View("Message", viewModel);
                }
            }

            if (returnValue == null)
            {
                returnValue = View(model);
            }

            return returnValue;
        }

        public virtual ActionResult Manage(long groupId)
        {
            return View(new GroupManageVM(new ProviderGroup(groupId)));
        }

        [HttpPost]
        public virtual ActionResult Manage(GroupManageVM model)
        {
            return View(new GroupManageVM());
        }
    }
}
