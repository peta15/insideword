<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.AdminIndexVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Administration") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <h1>Administration</h1>
    <div class="h1hr"></div>

    <ul>
        <% if (Model.CanAccessArticleManagement) { %>
            <li><%: Html.ActionLink("Article Management", MVC.Admin.Article()) %> </li>
        <% } if(Model.CanAccessCategoryManagement) { %>
            <li><%: Html.ActionLink("Category Management", MVC.Admin.Category()) %>
        </li>
        <% } if(Model.CanAccessConversationManagement) { %>
            <li><%: Html.ActionLink("Conversation Management", MVC.Admin.Conversation()) %>
        </li>
        <% } if(Model.CanAccessMemberManagement) { %>
            <li><%: Html.ActionLink("Member Management", MVC.Admin.Member()) %>
        </li>
        <% } if(Model.CanAccessSettingsManagement) { %>
            <li><%: Html.ActionLink("Settings Management", MVC.Admin.SettingsManagement()) %>
        </li>
        <% } %>
    </ul>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
