<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.MasterLoginModalVM>" %>

    <script src="<%= Links.Content.Scripts.modded.jquery_openid_js %>" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        SETTINGS.loggedOn = <%: Model.IsLoggedOn.ToString().ToLower() %>;
    </script>

    <div id="modalLogin" title="Login | Sign Up">
        <% if (Model.DisplayLoginModal) { %>
        <div id="modalLoginTabs">
	        <ul>
		        <li><a href="#modalLoginTabs-1">Open ID</a></li>
		        <li><a href="#modalLoginTabs-2">Sign Up</a></li>
		        <li><a href="#modalLoginTabs-3">Login</a></li>
                <li id="ui-tab-dialog-close"></li>
	        </ul>
	        <div id="modalLoginTabs-1">
                <% Html.RenderAction(MVC.Child.LoginOpenId()); %>
	        </div>
	        <div id="modalLoginTabs-2">
		        <% Html.RenderAction(MVC.Child.Register()); %>
	        </div>
	        <div id="modalLoginTabs-3">
		        <% Html.RenderAction(MVC.Child.Login()); %>
	        </div>
            <% } %>
        </div>
    </div>