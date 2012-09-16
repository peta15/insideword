<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.ProviderViewModels.ArticleVM>" %>

<div class="article">
    <h1><%: Html.ActionLink(Model.Title, MVC.Article.ArticleDetails(Model.Id))%></h1>
    <div class="articleHead">
        <div>
            Published by <%: Html.ProfileLink(Model.Author, Model.CurrentMember) %> <%: Model.EditDate %> in <%: Model.ArticleCategories %>
            / Read <%: Model.ViewCount %> times
        </div>
        <a href="<%: Url.Action( MVC.Article.ArticleDetails( Model.Id ) ) %>#vote">
            <div class="voteimg left"></div>
            <%: Model.ArticleVotes %> Votes
        </a>
        
        <a href='<%: Url.Action( MVC.Article.ArticleDetails(Model.Id))+"#comments" %>'>
           <div class="commentimg left"></div><%: Model.CountComments %> Comments
        </a>
        
        <% if (Model.CurrentMember.CanEdit) { %>
            <a href="<%: Url.Action( MVC.Article.ArticleEdit( Model.Id, null ) ) %>" class="editArticle right">Edit Article</a>
        <% } %>
    </div>
    <div class="hr"></div>
    <% if(Model.ShowBlurb) { %>
        <div class="blurb">
            <p><strong><%: Model.Blurb %></strong></p>
        </div>
    <% } %>
    <div class="articleBody"><%= Model.Text %></div>
    <div class="hr"></div>
    <div class="articleToolbar">
        <div class="left">
            <a name="vote"></a>
            <!-- large vote widget -->
            <form action="<%: Url.Action(MVC.Shared.Vote()) %>" class="voteWidget <%: Model.VoteStatus %>" method="post">
                <input type="hidden" name="articleId" value="<%: Model.Id %>" />
                <input type="hidden" name="voteValue" value="+1" class="voteValue" />
		        <strong title="Votes" class="result error"><%: Model.ArticleVotes %></strong>
		        <input type="submit" title="Vote up" value="+1" name="value" class="up callModalLogin" />
		        <input type="submit" title="Vote down" value="-1" name="value" class="down callModalLogin" />
	        </form>
        </div>
        <div class="right">
            <a href="http://www.addtoany.com/share_save?linkname=<%: Model.Title %>&linkurl=<%: Request.Url.AbsoluteUri %>" class="a2a_dd">
                <div class="emailimg left"></div> Share
            </a>
            <!--The javascript must stay embedded in this location for it to work-->
            <script type="text/javascript">
                a2a_linkname = "<%: Model.Title %>";
                a2a_linkurl = "<%: Request.Url.AbsoluteUri %>";
                a2a_color_main = "D7E5ED";
                a2a_color_border = "AECADB";
                a2a_color_link_text = "333333";
                a2a_color_link_text_hover = "333333";
                a2a_description = "<%: Model.Blurb %>";
            </script>
            <script type="text/javascript" src="http://static.addtoany.com/menu/page.js"></script>
        </div>
        <!--<div class="right">
            <a class="callModalLogin"><div class="flagimg"></div> Flag</a>
        </div>-->
    </div>
</div>