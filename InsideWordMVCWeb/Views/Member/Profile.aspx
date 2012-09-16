<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Member.ProfileVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title(Model.PageTitle)%>
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.comment_css %>" media="all" />
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.profile_css %>" media="all" />
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.article_css %>" media="all" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.thirdparty.jquery_masonry_min_js %>"></script>
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.modded.jquery_infinitescroll_js %>"></script>
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.profile_js %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="articleLeft articleList">
        <h1 class="articleListTitle">
            <% if(Model.HasWeb) {%>
                   <a href="<%: Model.WebUri%>" target="_blank">
            <% } %>
            <%: Model.HeaderTitle %>
            <% if(Model.HasWeb) {%>
                   </a>
            <% } %>
        </h1>
        <div class="h1hr"></div>
           <% if (Model.ArticleList.Count == 0) {
                if(Model.CurrentMember.Owns) {%>
                    <h3 class="articleMessage">You have no articles yet. <%: Html.ActionLink("Click here to compose one", MVC.Article.ArticleEdit(null, null))%></h3>
                <% } else { %>
                    <h3 class="articleMessage">No articles yet by this author. We hope they will publish soon!</h3>
                <% }
              } else { %>
                <div class="articleList">
                    <% foreach (var item in Model.ArticleList) {
                        Html.RenderAction( MVC.Child.Article(item) );
                    } %>
                </div>

                <% if (!Model.IsLastPage) { %>
                    <p id="infscr-pageNav" class="center"><%: Html.ActionLink("More", MVC.Member.Profile(Model.MemberId.Value, Model.NextPage), new { @class = "button buttonIndexWide" })%></p>
                <% } %>
            <% } %>
    </div>
    <div class="articleRight">
        <p class="birthday"><b>Joined</b><br /><%: Model.Birthday %></p>
        <img src="<%: Model.ProfileImage.Src %>"
             alt="<%: Model.ProfileImage.Alt %>"
             width="<%: Model.ProfileImage.Width %>"
             height="<%: Model.ProfileImage.Height %>"
             class="profileImg adjustedProfileImg" />
        <% if (Model.CurrentMember.CanEdit) { %>
            <div class="cls"></div>
            <a href="<%: Url.Action(MVC.Member.Account(Model.MemberId.Value, null)) %>" class="button">My Settings</a>
            <div class="cls"></div>
        <% } %>
        <% if (Model.HasWeb) { %>
           <p class="bio"><b>Web </b><a href="<%: Model.WebUri%>" target="_blank"><%: Model.WebDisplay %></a></p>
        <% } %>
        <% if (Model.HasBio) { %>
           <p class="bio"><b>Bio </b><%: Model.Bio%></p>
        <% } %>

        <% if (!Model.NoArticles) { %>
            <div class="articleList">
                <h3>Latest Articles Authored</h3>
                <ul>
                <% foreach (var aBlurb in Model.ArticleList) { %>
                    <li>
                        <%: Html.ActionLink(aBlurb.Title, MVC.Article.ArticleDetails(aBlurb.Id))%>
                    </li>
                <% } %>
                </ul>
            </div>
        <% } %>
    </div>
</asp:Content>
