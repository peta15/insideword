<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Member.ChangePasswordVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Change Password") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h1>Change Password</h1>
    <div class="h1hr"></div>

    <% using (Html.BeginForm(MVC.Member.ChangePassword())) {%>
        <%= Html.ValidationSummary(true)%>

        <img src="<%: Links.Content.img.@interface.lock_png %>" class="left" alt="Secure" width="64" height="64" />
        
        <div class="left">
            
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
                <input type="submit" value="Change Password" />
                <a href="<%: Url.Action( MVC.Member.Profile( Model.CurrentMemberId, null ) ) %>" class="button">Cancel</a>
            </div>

        </div>

    <% } %>

</asp:Content>

