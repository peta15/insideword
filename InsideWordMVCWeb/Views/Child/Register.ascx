<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.RegisterVM>" %>

<!-- InsideWord_partial class is used by partial.js to allow ajax validation of the partial -->
<div class="InsideWord_partial">
    <% using (Html.BeginForm(MVC.Child.Register())) { %>
        <%= Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.")%>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.UserName) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.UserName)%>
            <%: Html.ValidationMessageFor(model => model.UserName)%>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Email)%>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Email)%>
            <%: Html.ValidationMessageFor(model => model.Email)%>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Password)%>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.Password)%>
            <%: Html.ValidationMessageFor(model => model.Password)%>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ConfirmPassword)%>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.ConfirmPassword)%>
            <%: Html.ValidationMessageFor(model => model.ConfirmPassword)%>
        </div>

        <div class="editor-label">
        </div>
        <div class="editor-field">
            <%: Html.CheckBoxFor(model => model.AgreeLegal)%>
            I have read and accepted the
            <a href="<%: Url.Action( MVC.Info.Terms() ) %>">terms of use</a> and
            <a href="<%: Url.Action( MVC.Info.Privacy() ) %>">privacy policy</a> *
            <%: Html.ValidationMessageFor(model => model.AgreeLegal)%>
        </div>

        <div class="editor-submit">
            <input type="submit" value="Register" />
        </div>

    <% } %>
</div>