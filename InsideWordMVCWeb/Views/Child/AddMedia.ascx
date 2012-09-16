<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Shared.AddMediaVM>" %>

<% using (Html.BeginForm( MVC.Shared.AddMedia(), FormMethod.Post,  new { enctype="multipart/form-data" })) { %>
    <%: Html.ValidationSummary(true) %>

    <div class="editor-label">
        <%: Html.Label("Upload Image")%>
    </div>
    <div class="editor-field">
        <%= Html.FileInput(Model.FilePhotoKey)%>
        <%: Html.ValidationMessage(Model.FilePhotoKey)%>
    </div>

    <div class="editor-submit">
        <input type="submit" value="Upload" />
    </div>

<% } %>

<div style="text-align: center; width: 100%">
    <i>Coming Soon: Select your image from your image gallery</i>
</div>