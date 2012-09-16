using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using InsideWordAdvancedResource;
using System.Configuration;
using InsideWordAdvancedResource.Config;
using InsideWordProvider;
using System.Collections.Generic;
using System;
using System.Reflection;
using InsideWordMVCWeb.Controllers;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.Models.Utility;
using InsideWordResource;

namespace InsideWordMVCWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Provider.DbCtx = new ObjectContextManager();
            InsideWordWebLog.Initialize(InsideWordSettingsDictionary.Instance);
            InsideWordWebLog.Instance.Log.Info("Starting up InsideWordWeb");

            // Load some data from the configs
            ProviderPermissionConfigSection permissionsSettings = ConfigurationManager.GetSection("providerPermissions") as ProviderPermissionConfigSection;
            Dictionary<string, ProviderPermission> rightsDict = ConfigReaderProviderPermission.Parse(permissionsSettings);

            // set the e-mail manager to the current host
            EmailManager.Instance.HttpHost = InsideWordWebSettings.HostName;

            // Register our routes
            AreaRegistration.RegisterAllAreas();
            _RegisterRoutes();
            // uncomment and run to test route debugging
            // RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            HttpContext a = HttpContext.Current;
            // Leave this function here. It's useful for when we need to insert temporary code to debug requests.
        }

        protected void Application_Error(Object sender, EventArgs args)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            string currentUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            string previousUrl = string.Empty;
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                previousUrl = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
            }
            string browser = HttpContext.Current.Request.UserAgent;
            string requestType = HttpContext.Current.Request.RequestType;
            InsideWordWebLog.Instance.Log.Error("Encountered error at url: " + currentUrl +"\n"+
                                               "previous url: "+ previousUrl+"\n"+
                                               "browser: " + browser +"\n" +
                                               "request type: "+requestType, ex);
        }

        private static bool _RegisterRoutes()
        {
            RouteCollection routes = RouteTable.Routes;

            // NOTE routes are matched from top to bottom

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*===========================================*
             * CHILD (not publicly acessible)
             * ==========================================*/
            routes.MapRoute(
                "child",
                "child/{action}",
                new { Controller = "Child" }
            );

            routes.MapRoute(
                "child edit_personal_info",
                "child/edit_personal_info/{memberId}",
                MVC.Child.EditPersonalInfo()
            );

            /*===========================================*
             * INFO
             * ==========================================*/
            routes.MapRoute(
                "info",
                "info/{action}",
                new { controller = "Info", action = "About" }
            );

            /*===========================================*
             * ARTICLE
             * ==========================================*/

            routes.MapRoute(
                "article article_editor",
                "article/editor/{articleId}",
                MVC.Article.ArticleEditor().AddRouteValues(new { articleId = BugFixUrlParameter.Optional })
            );

            routes.MapRoute(
                "article article_edit",
                "article/edit/{articleId}/{issuedKey}",
                MVC.Article.ArticleEdit().AddRouteValues(new { articleId = BugFixUrlParameter.Optional, issuedKey = BugFixUrlParameter.Optional })
            );

            routes.MapRoute(
                "article article_details",
                "article/{articleId}",
                MVC.Article.ArticleDetails()
            );

            /*===========================================*
             * MEMBER
             * ==========================================*/

            routes.MapRoute(
                "member request_validate_email",
                "member/request_validate_email/{memberId}/{email}",
                MVC.Member.RequestValidateEmail()
            );
            
            routes.MapRoute(
                "member validate_email",
                "member/validate_email/{key}",
                MVC.Member.ValidateEmail()
            );

            routes.MapRoute(
                "member delete",
                "member/delete/{key}",
                MVC.Member.Delete()
            );

            routes.MapRoute(
                "member profile",
                "member/profile/{memberId}/{page}",
                MVC.Member.Profile().AddRouteValues(new { page = BugFixUrlParameter.Optional })
            );

            routes.MapRoute(
                "member account",
                "member/account/{memberId}/{issuedKey}",
                MVC.Member.Account().AddRouteValues(new { issuedKey = BugFixUrlParameter.Optional })
            );

            routes.MapRoute(
                "member reset_password_request",
                "member/reset_password_request",
                MVC.Member.ResetPasswordRequest()
            );

            routes.MapRoute(
                "member change_password",
                "member/change_password/{issuedKey}",
                MVC.Member.ChangePassword().AddRouteValues( new { issuedKey = BugFixUrlParameter.Optional } )
            );

            routes.MapRoute(
                "member",
                "member/{action}/{memberId}",
                new { controller = "Member", memberId = BugFixUrlParameter.Optional }
            );

            /*===========================================*
             * GROUP
             * ==========================================*/
            routes.MapRoute(
                "groups detail",
                "groups/detail/{groupId}",
                MVC.Group.Details()
            );

            routes.MapRoute(
                "groups manage",
                "groups/manage/{groupId}",
                MVC.Group.Manage()
            );

            routes.MapRoute(
                "groups register",
                "groups/register",
                MVC.Group.Register()
            );

            routes.MapRoute(
                "groups index",
                "groups",
                MVC.Group.Index()
            );

            /*===========================================*
             * CONVERSATION
             * ==========================================*/
            routes.MapRoute(
                "conversation add",
                "conversation/add/article/{articleId}",
                MVC.Conversation.AddComment()
            );

            /*===========================================*
             * ADMIN
             * ==========================================*/

            routes.MapRoute(
                "admin category_edit",
                "admin/category/edit/{categoryId}",
                MVC.Admin.CategoryEdit().AddRouteValues(new { categoryId = BugFixUrlParameter.Optional })
            );

            routes.MapRoute(
                "admin",
                "admin/{action}",
                new { controller = "Admin", action = "Index" }
            );

            /*===========================================*
             * API
             * ==========================================*/
            routes.MapRoute(
                "api log_info",
                "api/log_info",
                MVC.API.LogInfo()
            );

            routes.MapRoute(
                "api ping",
                "api/ping",
                MVC.API.Ping()
            );

            routes.MapRoute(
                "api incr",
                "api/incr",
                MVC.API.Incr()
            );

            routes.MapRoute(
                "api domain_identification_request",
                "api/domain_identification_request",
                MVC.API.DomainIdentificationRequest()
            );

            routes.MapRoute(
                "api delayed_visit_request",
                "api/delayed_visit_request",
                MVC.API.DelayedVisitRequest()
            );

            routes.MapRoute(
                "api domain_identification",
                "api/domain_identification",
                MVC.API.DomainIdentification()
            );

            routes.MapRoute(
                "api issued_key_request",
                "api/issued_key_request",
                MVC.API.IssuedKeyRequest()
            );

            routes.MapRoute(
                "api publish_article",
                "api/publish_article",
                MVC.API.PublishArticle()
            );

            routes.MapRoute(
                "api change_article_state",
                "api/change_article_state",
                MVC.API.ChangeArticleState()
            );

            routes.MapRoute(
                "api login",
                "api/login",
                MVC.API.Login()
            );

            routes.MapRoute(
                "api default_category_id",
                "api/default_category_id",
                MVC.API.DefaultCategoryId()
            );

            routes.MapRoute(
                "api category",
                "api/category/{categoryId}",
                MVC.API.Category().AddRouteValues(new { categoryId = BugFixUrlParameter.Optional })
            );

            routes.MapRoute(
                "api account_sync",
                "api/account_sync",
                MVC.API.AccountSync()
            );

            routes.MapRoute(
                "api article_rank",
                "api/article_rank/{articleId}",
                MVC.API.ArticleRank()
            );

            routes.MapRoute(
                "api profile_link",
                "api/profile_link",
                MVC.API.ProfileLink()
            );

            /*===========================================*
             * ERROR
             * ==========================================*/
            routes.MapRoute(
                "error",
                "error/{errorCode}",
                MVC.Error.Index().AddRouteValues(new { errorCode = BugFixUrlParameter.Optional })
            );

            /*===========================================*
             * Shared
             * ==========================================*/

            routes.MapRoute(
                "shared vote",
                "shared/vote/article",
                MVC.Shared.Vote()
            );

            routes.MapRoute(
                "shared add_media",
                "shared/add_media/{purpose}/{type}/{memberId}",
                MVC.Shared.AddMedia().AddRouteValues(new { memberId = BugFixUrlParameter.Optional })
            );

            /*===========================================*
             * HOME
             * ==========================================*/
            routes.MapRoute(
                "index",
                "{categoryId}/{page}",
                MVC.Home.Index().AddRouteValues(new { categoryId = BugFixUrlParameter.Optional, page = BugFixUrlParameter.Optional } )
            );

            return true;
        }
    }
}