using System;
using System.Web;
using System.Security;
using System.Reflection;
using System.Globalization;
using System.Configuration;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using InsideWordProvider;
using InsideWordMVCWeb.ViewModels.Shared;
using System.Net.Mail;
using System.Linq;
using InsideWordResource;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Models.Annotation
{
    /*=========================================*/
    /* Validation
    /*=========================================*/

    // TODO replace some of these with remote validation in MVC3  http://www.asp.net/mvc/mvc3#BM_Model_Validation_Improvements

    /// <summary>
    /// Validation: checks e-mail is unique among total pool of activated members. Will return true if the user name belongs to the current member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EmailUniqueAttribute : ValidationAttribute
    {
        public string Email { get; private set; }

        public override bool IsValid(object value)
        {
            string inputEmail = Convert.ToString(value, CultureInfo.CurrentCulture);
            return ValidationSupport.IsEmailUnique(inputEmail);
        }
    }

    /// <summary>
    /// Validation: checks user name is unique among total pool of members. Will return true if the user name belongs to the current member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UserNameUniqueAttribute : ValidationAttribute
    {
        public string UserName { get; private set; }

        public override bool IsValid(object value)
        {
            string userName = Convert.ToString(value, CultureInfo.CurrentCulture);
            long? memberId =  ProviderUserName.FindOwner(userName);
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            return !memberId.HasValue ||
                   ( currentMember.IsLoggedOn &&
                     currentMember.IsActive &&
                     memberId.Value == currentMember.Id.Value );
        }
    }

    /// <summary>
    /// Validation: checks group name is unique among total pool of groups.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class GroupNameUniqueAttribute : ValidationAttribute
    {
        public string UserName { get; private set; }

        public override bool IsValid(object value)
        {
            string name = Convert.ToString(value, CultureInfo.CurrentCulture);
            long? memberId = ProviderGroup.ExistsName(name, true);
            return !memberId.HasValue;
        }
    }

    /// <summary>
    /// ReCaptcha Validation for a controller action
    /// 
    /// Example:
    /// [CaptchaValidator]  
    /// [AcceptVerbs( HttpVerbs.Post )]  
    /// public ActionResult CreateComment( Int32 id, bool captchaValid )  
    /// {  
    ///   .. Do something here  
    /// }  
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string ChallengeFieldKey = "recaptcha_challenge_field";
        private const string ResponseFieldKey = "recaptcha_response_field";

        /// <summary>
        /// Called before the action method is invoked
        /// </summary>
        /// <param name="filterContext">Information about the current request and action</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[ChallengeFieldKey];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[ResponseFieldKey];
            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {
                PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"],
                RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };

            var recaptchaResponse = captchaValidtor.Validate();

            // this will push the result value into a parameter in our Action
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;

            base.OnActionExecuting(filterContext);

            // Add string to Trace for testing
            //filterContext.HttpContext.Trace.Write("Log: OnActionExecuting", String.Format("Calling {0}", filterContext.ActionDescriptor.ActionName));
        }
    }

    /// <summary>
    /// Validation: ensures email address is in a valid format
    /// </summary>
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute() : base(IWConstants.RegexEmail) { }
    }

    /// <summary>
    /// Validation: boolean value must be true to pass validation
    /// </summary>
    public class BooleanRequiredToBeTrueAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null && (bool)value;
        }
    }

    /*=========================================*/
    /* Action Method Access Restriction
    /*=========================================*/

    /// <summary>
    /// Requires user authentication and optionally authorizaion for a certain group
    /// Require user logged in (but not necessarily in group): [RestrictAccess]
    /// Restrict access to a group: [RestrictAccess(AccessGroup=ProviderCurrentMember.AccessGroup.ArticleAdmin)]
    /// </summary>
    public class RestrictAccess : AuthorizeAttribute
    {
        public ProviderCurrentMember.AccessGroup AccessGroup;
        private bool FailedAuth = false;

        // override OnAuthorization to redirect to any action we want (such as ErrorController) if not authorized.
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (FailedAuth)
            {
                var viewModel = new MessageVM
                {
                    Image = ImageLibrary.Alert,
                    CssClassContainer = "failure",
                    Message = "Authorization Failure.  You may need to login or obtain access privileges to view this page.",
                    Title = "Authorization Failure",
                    LinkText = "Continue",
                    LinkHref = "/"
                };
                filterContext.Result = new ViewResult { ViewName = "Message", ViewData = new ViewDataDictionary(viewModel) };
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            FailedAuth = !( currentMember.IsLoggedOn && (AccessGroup == 0 || (AccessGroup != 0 && currentMember.CanAccess(AccessGroup))));
            return !FailedAuth;
        }
    }

    /// <summary>
    /// Requests must be from an ajax source to access the action method.  
    /// note that you could have multiple action methods with the same name and the ones with this attribute
    /// would be bypassed if not queried via ajax in favor of the others.
    /// </summary>
    public class AjaxOnly : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }

    /// <summary>
    /// Requests must be from a non-ajax source to access the action method.  
    /// note that you could have multiple action methods with the same name and the ones with this attribute
    /// would be bypassed if queried via ajax in favor of the others.
    /// </summary>
    public class NonAjaxOnly : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return !controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }

    /// <summary>
    /// Restrict action method to only be called via SSL
    /// </summary>
    public class RequireHTTPS : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            if (!controllerContext.HttpContext.Request.IsSecureConnection)
            {
                throw new SecurityException("This action " + methodInfo.Name + " can only be called via SSL");
            }
            return true;
        }
    }

    /*=========================================*/
    /* Global Controller Anti Forgery Token
    /*=========================================*/

    /// <summary>
    /// Global Controller Anti Forgery Token.  (implement globally with MVC3)
    /// usage: [ValidateAntiForgeryTokenWrapper(HttpVerbs.Post, Constants.AntiForgeryTokenSalt)]
    /// note the salt could be specified from web.config
    /// tokens must also be handled with jquery. see the jquery plugin jquery.antiForgery.js
    /// Put this in all forms: <%: this.Html.AntiForgeryToken(Constants.AntiForgeryTokenSalt) %>
    /// http://weblogs.asp.net/dixin/archive/2010/05/22/anti-forgery-request-recipes-for-asp-net-mvc-and-ajax.aspx
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false, Inherited = true)]
    public class ValidateAntiForgeryTokenWrapperAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly ValidateAntiForgeryTokenAttribute _validator;

        private readonly AcceptVerbsAttribute _verbs;

        public ValidateAntiForgeryTokenWrapperAttribute(HttpVerbs verbs)
            : this(verbs, null)
        {
        }

        public ValidateAntiForgeryTokenWrapperAttribute(HttpVerbs verbs, string salt)
        {
            this._verbs = new AcceptVerbsAttribute(verbs);
            this._validator = new ValidateAntiForgeryTokenAttribute()
            {
                Salt = salt
            };
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();
            if (this._verbs.Verbs.ToList().Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase))
            {
                this._validator.OnAuthorization(filterContext);
            }
        }
    }

    /// <summary>
    /// supporting validation methods for annotations, remote validation, and validation called in controllers
    /// </summary>
    public class ValidationSupport
    {

        static public bool IsEmailUnique(string inputEmail)
        {
            MailAddress email = null;
            bool returnValue = IWStringUtility.TryParse(inputEmail, out email);
            if (returnValue)
            {
                long? memberId = ProviderEmail.FindOwner(email, true);
                ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
                returnValue = !memberId.HasValue ||
                               (currentMember.IsLoggedOn &&
                                memberId.Value == currentMember.Id.Value);
            }
            return returnValue;
        }
    }
}
