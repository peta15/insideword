<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Contact Us")%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="info feedback">
        <h1>Contact Us</h1>
        <div class="h1hr"></div>
        <br />
        <p>We are fanatical about feedback.  Contact us and let us know what we can do better! </p>
        <p><i>Feedback, tips, suggestions, press, business, bugs…</i></p>
        <p>Contact us at <b>support@insideword.com</b></p>
    </div>
</asp:Content>