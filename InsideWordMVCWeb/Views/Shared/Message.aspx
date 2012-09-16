<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Shared.MessageVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title(Model.Title) %>
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.message_css %>" media="all" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="message">
        <h1><%: Model.Title %></h1>
        <div class="h1hr"></div>

        <div class="messageBody <%: Model.CssClassContainer %>">
            <img src="<%: Model.Image.Src %>"
                 alt="<%: Model.Image.Alt %>"
                 width="<%: Model.Image.Width %>"
                 height="<%: Model.Image.Height %>" />
            <p><%= Model.Message %></p>
            <% if (Model.Details != null)
               { %>
                <ul>
                <% foreach (var detail in Model.Details)
                   { %>
                    <li><%= detail%></li>
                <% } %>
                </ul>
            <% }
               if (Model.LinkHref != null)
               { %>
            <a href="<%: Model.LinkHref %>" class="button messageButton"><%: Model.LinkText%></a>
            <% } %>
        </div>
    </div>
</asp:Content>