<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.ProviderViewModels.CurrentMemberVM>" %>

<div class="header">
    <a href="<%: Url.Action( MVC.Home.Index(null, null) ) %>">
        <img class="logo" src="<%: Links.Content.img.@interface.Logo_Alpha_png %>" alt="InsideWord" />
    </a>
    <div class="latest">
        <span class="motto">Your Newspaper </span><%: Html.NewsTicker() %>
    </div>
    
    <% if (Model.IsLoggedOn) { %>
        <div class="headerItem">
            <a href="<%: Url.Action( MVC.Member.Logout() ) %>" class="button">Logout</a>
        </div>
        <div class="headerItem">
            <%: Html.CombinedProfileAccountLink(Model) %>
        </div>
    <% } else { %>
        <div class="headerItem">
            <a class="callModalLogin button">Login | Signup</a>
        </div>
    <% } %>
</div>