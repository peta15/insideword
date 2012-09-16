<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Member.ResetPasswordVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Reset Password") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h1>Reset Password</h1>
    <div class="h1hr"></div>

    <% using (Html.BeginForm()) {%>
        <%= Html.ValidationSummary(true) %>

        <%: Html.HiddenFor(model => model.MemberId) %>
        <%: Html.HiddenFor(model => model.Key) %>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Password) %>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.Password) %>
            <%: Html.ValidationMessageFor(model => model.Password) %>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ConfirmPassword) %>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.ConfirmPassword) %>
            <%: Html.ValidationMessageFor(model => model.ConfirmPassword) %>
        </div>
            
        <div class="editor-submit">
            <input type="submit" value="Submit" />
        </div>

    <% } %>

</asp:Content>

