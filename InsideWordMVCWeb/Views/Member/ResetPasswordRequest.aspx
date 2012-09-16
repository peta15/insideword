<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Member.ResetPasswordRequestVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Reset Password") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h1>Reset Password</h1>
    <div class="h1hr"></div>

    <% using (Html.BeginForm(MVC.Member.ResetPasswordRequest())) {%>
        <%= Html.ValidationSummary(true) %>
        
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Email) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Email)%>
            <%: Html.ValidationMessageFor(model => model.Email)%>
        </div>

        <div class="editor-label">
            Type the words shown
        </div>

        <div class="editor-field">
        <%= Html.GenerateCaptcha() %>
        </div>
        
        <div class="editor-submit">
            <input type="submit" value="Request Password Reset" />
        </div>

    <% } %>

</asp:Content>

