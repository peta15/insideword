<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.LoginVM>" %>

<!-- InsideWord_partial class is used by partial.js to allow ajax validation of the partial -->
<div class="InsideWord_partial">
    <% using (Html.BeginForm(MVC.Child.Login())) {%>
        <%= Html.ValidationSummary(true) %>
        
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Email) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Email)%>
            <%: Html.ValidationMessageFor(model => model.Email)%>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Password) %>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.Password) %>
            <%: Html.ValidationMessageFor(model => model.Password) %>
        </div>
            
        <div class="editor-label">
        </div>
        <div class="editor-field">
            <%: Html.CheckBoxFor(model => model.RememberMe) %>
            Stay Signed In
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <%: Html.ActionLink("Forgot Password?", MVC.Member.ResetPasswordRequest() ) %>
        </div>
 
        <div class="editor-submit">
            <input type="submit" value="Login" />
        </div>

    <% } %>
</div>
