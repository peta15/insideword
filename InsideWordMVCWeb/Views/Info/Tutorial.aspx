<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Tutorial") %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Body" runat="server">
    <div class="info about">
        <h1>Tutorials</h1>
        <div class="h1hr"></div>
        <br />
        <iframe title="YouTube video player" width="480" height="390" src="http://www.youtube.com/embed/24lD0TtCW7U" frameborder="0" allowfullscreen></iframe>
    </div>
</asp:Content>
