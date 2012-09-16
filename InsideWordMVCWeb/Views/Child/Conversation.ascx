<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.ProviderViewModels.ConversationVM>" %>

<div class="conversation" id="conversation-<%: Model.Id %>">
    <% foreach (var comment in Model.CommentList) {
            Html.RenderPartial("~/Views/Child/Comment.ascx", comment);
       } %>
    <div class="cls"></div>
</div>