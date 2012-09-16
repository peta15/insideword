using System.Web.Mvc;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Controllers
{
    public partial class ErrorController : BugFixController
    {
        // GET: /Error/errorCode
        public virtual ActionResult Index(int? errorCode)
        {
            string errorMessage = string.Empty;
            if (errorCode.HasValue)
            {
                errorMessage = ErrorStrings.HtmlErrorCode(errorCode.Value);
            }
            else
            {
                errorMessage = "Unknown error";
            }

            var viewModel = new MessageVM
            {
                Image = ImageLibrary.Alert,
                CssClassContainer = "failure",
                Message = errorMessage + "<br />Please <a href=\"" + Url.Action(MVC.Info.ContactUs()) + "\">contact us</a> to resolve the issue.",
                Title = ErrorStrings.TITLE_ERROR,
                LinkText = "Continue",
                LinkHref = Url.Action( MVC.Home.Index() ),
            };
            
            return View( MVC.Shared.Views.Message, viewModel);
        }
    }
}
