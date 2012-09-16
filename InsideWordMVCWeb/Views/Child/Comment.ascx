<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.ProviderViewModels.CommentVM>" %>

<div id="comment-<%: Model.Id %>" class="<%: Model.IsReply ? "commentsAreaReply" : "commentsArea" %>">
    <div class="<%: Model.IsReply ? "userDetailReply" : "userDetail" %>">
        <strong><%: Html.ProfileLink(Model.Author, Model.CurrentMember, 9) %></strong><br />
        <%: Model.EditDate %><br />
        <strong><%: Model.PageOwns? "Author" : ""%></strong>
    </div>
    <div class="<%: Model.IsReply ? "commentBodyReply" : "commentBody" %>">
        <div class="<%: Model.IsReply ? "commentBodyTextReply" : "commentBodyText" %>">
            <p><%: Model.Text %></p>
        </div>
        <div class="commentReplyText">
            <a id="openCommentReply" class="callModalLogin openCommentReply pointer">Reply</a>
            <!--<br />
            <% if (!Model.IsHidden) { %>
            <a id="flagComment" class="callModalLogin pointer">Flag</a>
            <% } %>-->
        </div>
    </div>
</div>