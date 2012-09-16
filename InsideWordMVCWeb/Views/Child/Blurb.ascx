<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.ProviderViewModels.BlurbVM>" %>

<div class="blurb">
	<div class="blurbContent">
    <% if (Model.BlurbPhoto != null) { %>
		<div class="photoWrapper">
			<a href="<%: Url.Action( MVC.Article.ArticleDetails( Model.Id ) ) %>"> <%: Html.Photo(Model.BlurbPhoto) %> </a>
		</div>
    <% } %>
		<h2><%: Html.ActionLink(Model.Title, MVC.Article.ArticleDetails(Model.Id))%></h2>
		<p class="author"> Published by <%: Html.ProfileLink(Model.Author, null) %> <%: Model.EditDate %></p>
		<p><%: Model.Text %></p>
	</div>
    <div class="miniToolBar">
        <a href="<%: Url.Action( MVC.Article.ArticleDetails( Model.Id ) ) %>#comments"><div class="commentimg left"></div><%: Model.CountComments %> Comments</a>
        <a href="<%: Url.Action( MVC.Article.ArticleDetails( Model.Id ) ) %>#vote"><div class="voteimg left"></div><%: Model.ArticleVotes %> Votes</a>
        <div class="right">
            <%: Html.ActionLink("Read Article", MVC.Article.ArticleDetails(Model.Id))%>
        </div>
    </div>
</div>