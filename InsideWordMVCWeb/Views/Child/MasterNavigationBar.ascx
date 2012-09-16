<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.MasterNavigationBarVM>" %>

<div class="navigationBar">
    <div class="navigationList">
        <ul class="sf-menu sf-vertical sf-js-enabled sf-shadow">
            <li>
                <a id="publishArticleLink" class="callLoginModal important" href="<%: Url.Action( MVC.Article.ArticleEdit(null, null) ) %>"><b class="plus">+</b> <b class="large">Publish Article</b></a>
            </li>
            <% if (Model.DisplayAdminTools) { %>
                <li><%: Html.ActionLink("Administration", MVC.Admin.Index() )%></li>
            <% } %>
        </ul>
        <%= Html.NavigationList() %>
        <ul class="sf-menu sf-vertical sf-js-enabled sf-shadow">
            <li><%: Html.ActionLink("Explore InsideWord", MVC.Info.About() ) %>
                <ul>
                    <li><%: Html.ActionLink("Contact Us", MVC.Info.ContactUs() ) %></li>
                    <li><%: Html.ActionLink("About", MVC.Info.About() ) %></li>
                    <li><%: Html.ActionLink("FAQ", MVC.Info.Faq() ) %></li>
                    <li><%: Html.ActionLink("Privacy", MVC.Info.Privacy() ) %></li>
                    <li><%: Html.ActionLink("Guidelines", MVC.Info.Guidelines() ) %></li>
                    <li><%: Html.ActionLink("Terms", MVC.Info.Terms() ) %></li>
                    <li><%: Html.ActionLink("Tutorial", MVC.Info.Tutorial() ) %></li>
                </ul>
            </li>
        </ul>
        <div class="cls"></div>
        <div class="promo">
            <a href="http://wordpress.org/extend/plugins/insidewordsyncher/">
                <img src="<%: Links.Content.img.@interface.wordpress_jpg %>" width="75px" height="75px" alt="wordpress" />
            </a>
            <p><a href="http://wordpress.org/extend/plugins/insidewordsyncher/">Sync your WordPress blog</a></p>
        </div>
        <!--
        <div class="promo">
            <a href="<%: Url.Action(MVC.Info.PublishByEmail()) %>">
                <img src="<%: Links.Content.img.@interface.bigemail_jpg %>" width="75px" height="76px" alt="e-mail" />
            </a>
            <p><a href="<%: Url.Action(MVC.Info.PublishByEmail()) %>">Publish by<br />E-mail</a></p>
        </div>
        -->
    </div>
</div>