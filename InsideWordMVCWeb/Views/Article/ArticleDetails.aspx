<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Article.DetailsVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title(Model.PageTitle) %>
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.article_css %>" media="all" />
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.comment_css %>" media="all" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.article_js %>" ></script>
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.thirdparty.charCount_js %>" ></script>
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.thirdparty.jquery_scrollTo_min_js %>" ></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="articleLeft">
        <% Html.RenderAction( MVC.Child.Article( Model.Article ) ); %>
        <div class="comments">
            <div>
                <div class="left"><h3><a name="comments">Comments (<%: Model.Article.CountComments%>)</a></h3></div>
                <div class="right">
                    <br />
                    <a id="redirectToStartConvo" href="#textComment" class="comments redirectToStartConvo">Start a conversation</a> or click reply below
                </div>
                <div class="hr"></div>
            </div>

            <div class="commentsList">
            <% foreach (var conversation in Model.ConversationList) { %>
                <% Html.RenderAction(MVC.Child.Conversation(conversation)); %>
            <% } %>
            </div>
            <div class="commentsForm">
                <% using (Html.BeginForm( MVC.Conversation.AddComment(Model.Article.Id, null, null) ) ) { %>
                    <h3><a name="textComment">Start a Conversation</a></h3>
                    <textarea name="textComment" cols="20" rows="3" <%= Model.Article.CurrentMember.IsLoggedOn ? "" : "disabled=\"disabled\"" %> class="txtArea textComment charCount_comment"></textarea>
                    <input type="submit" value="Create Conversation" <%= Model.Article.CurrentMember.IsLoggedOn ? "" : "disabled=\"disabled\"" %> class="callModalLogin button createConversationSubmit left" />
                    <%= Model.Article.CurrentMember.IsLoggedOn ? "" : "&nbsp; <a class='callModalLogin'>Please Login to comment</a>"%>
                    <div class="loadingComment left hidden">
                        <img src="<%: Links.Content.img.@interface.loader_bar_gif %>" alt="Loading" />
                    </div>
                <% } %>
            </div>
        </div>
    </div>

    <div class="articleRight">
        <img src="<%: Model.Article.Author.ProfileImage.Src %>"
             alt="<%: Model.Article.Author.ProfileImage.Alt %>"
             width="<%: Model.Article.Author.ProfileImage.Width %>"
             height="<%: Model.Article.Author.ProfileImage.Height %>"
             class="profileImg" />
        <div class="articleList">
            <h3>More Articles By <%: Html.ProfileLink(Model.Article.Author, Model.Article.CurrentMember) %></h3>
            <ul id="otherArticlesByAuthorList">
                <% if (Model.RelatedHeadlinesByAuthor.Count == 0) { %>
                    <li>None yet. If you like the article vote it up and they will be encouraged to write more.</li>
                <% } else {
                      foreach (var item in Model.RelatedHeadlinesByAuthor) { %>
                      <li><%: Html.ActionLink(item.Value, MVC.Article.ArticleDetails(item.Key))%></li>
                      <% }
                } %>
            </ul>
        </div>
        <div class="articleList">
            <h3>Related Articles</h3>
            <ul id="relatedArticlesList">
                <% if (Model.RelatedHeadlinesByCategory.Count == 0) { %>
                    <li>None yet. <%: Html.ActionLink("Create one!", MVC.Article.ArticleEdit(null, null))%></li>
                <% } else {
                    foreach (var item in Model.RelatedHeadlinesByCategory) { %>
                    <li><%: Html.ActionLink(item.Value, MVC.Article.ArticleDetails(item.Key))%></li>
                    <% }
                } %>
            </ul>
        </div>
    </div>
    <div class="cls"></div>

    <!-- TODO: Cleanup! If statement checks if the person is logged in and then
         the if statements below that also check. Shouldn't it all be true at that point?
    -->
    <% if (Model.Article.CurrentMember.IsLoggedOn) { %>
        <!-- comment reply template (starts hidden) -->
        <div class="commentsAreaReply commentReplyTemplate">
            <div class="userDetail">
                <strong>
                <%: Html.ProfileLink(Model.Article.CurrentMember, Model.Article.CurrentMember) %>
                </strong>
                <br />
                now
                <br />
                <strong><%: Model.Article.CurrentMember.Owns ? "Author" : ""%></strong>
            </div>
            <div class="commentBodyReply">
                <% using (Html.BeginForm( MVC.Conversation.AddComment(Model.Article.Id, null, null) )) { %>
                    <textarea name="textComment" rows="5" cols="53" <%= Model.Article.CurrentMember.IsLoggedOn ? "" : "disabled=\"disabled\"" %> class="commentReplyTextArea charCount_commentReply"></textarea>
                    <div class="commentReplyControls">
                        <input type="submit" value="Post Reply" <%= Model.Article.CurrentMember.IsLoggedOn ? "" : "disabled=\"disabled\"" %> class="button replyCommentSubmit" />
                        &nbsp; <span class="voteText close">Cancel</span> <%= Model.Article.CurrentMember.IsLoggedOn ? "" : "&nbsp; <a class='callModalLogin'>Please Login to comment</a>"%>
                        <div class="loadingComment hidden">
                            <img src="<%: Links.Content.img.@interface.loader_bar_gif %>" alt="Loading" />
                        </div>
                    </div>
                <% } %>
            </div>
            <div class="cls"></div>
        </div>
    <% } %>
</asp:Content>