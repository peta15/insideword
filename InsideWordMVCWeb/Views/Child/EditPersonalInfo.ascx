<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.PersonalInfoVM>" %>
<script src="<%= Links.Content.Scripts.thirdparty.charCount_js %>" type="text/javascript"></script>
<div class="left InsideWord_partial">
    <% using (Html.BeginForm(MVC.Child.EditPersonalInfo(Model.MemberId))) {%>
        <%= Html.ValidationSummary(true, "Personal info mofication unsuccessful. Please correct the errors and try again.")%>

        <div>
            <div class="left">
                <div class="editor-label">
                    <%: Html.LabelFor(model => model.UserName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.UserName) %>
                    <%: Html.ValidationMessageFor(model => model.UserName) %>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(model => model.Bio) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextAreaFor(model => model.Bio, 5, 16, new { @class="charCount_textBio" })%>
                    <%: Html.ValidationMessageFor(model => model.Bio) %>
                </div>
            </div>
        </div>
   
        <div class="editor-submit">
            <input type="submit" value="Save" />
            <a href="<%: Url.Action(MVC.Member.Account(Model.MemberId, null)) %>" class="button">Cancel</a>
        </div>
    <% } %>
</div>