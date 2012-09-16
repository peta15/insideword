<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Article.ArticleEditorVM>" %>

<div class="InsideWord_partial">
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.article_css %>" media="all" />
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.article_submit_css %>" media="all" />

    <% using (Html.BeginForm( MVC.Article.ArticleEditor(Model.ArticleId) )) { %>
        <div>
            <h1 class="inline left headerMarginBottom">Publish Article</h1>
            <span class="right noSignUp">
                <h3 class="red"><i>No</i> Sign Up Required. Publish <i>Anonymously</i>.</h3>
                <div class="cls"></div>
                <h3>... or Sign Up & Publish with a User Name.</h3>
            </span>
        </div>

        <% if (Model.CreateDate != null) { %>
        <div class="editor-label">Create Date:</div>
        <div class="editor-field-large"><%: Model.CreateDate %></div>
        <div class="editor-label">Edit Date:</div>
        <div class="editor-field-large"><%: Model.EditDate %></div>
        <div class="cls"></div>
	    <br />
        <% } %>

        <h3>Title & Blurb</h3>
		<div class="h3hr"></div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Title) %>
        </div>
        <div class="editor-field-large counterWrapper">
            <%: Html.TextBoxFor(model => model.Title, new { @class="charCount_textTitle" }) %>
            <%: Html.ValidationMessageFor(model => model.Title) %>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Blurb) %>
            <p class="note">Optional</p>
        </div>
        
        <div class="editor-field-large counterWrapper">
            <%: Html.TextBoxFor(model => model.Blurb, new { @class="charCount_textBlurb" }) %>
            <%: Html.ValidationMessageFor(model => model.Blurb) %>
            <p class="note">Auto-generated to first few sentences of article when left blank.</p>
        </div>
        
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ArticleCategoryId)%>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.ArticleCategoryId, Model.CategoryList)%>
            <%: Html.ValidationMessageFor(model => model.ArticleCategoryId)%>
        </div>

        <h3>Article</h3>
		<div class="h3hr"></div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ArticleBody) %>
        </div>
        <div class="editor-field-large counterWrapper">
            <%: Html.TextAreaFor(model => model.ArticleBody, 32, 85, new { @class = "tinymce" })%>
            <%: Html.ValidationMessageFor(model => model.ArticleBody) %>
        </div>
        
        <div class="h3hr"></div>

        <% if (Model.ShowEmailInput) { %>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ArticleEmail) %>
            <p class="note">Optional</p>
        </div>
        <div class="editor-field-large counterWrapper">
            <%: Html.TextBoxFor(model => model.ArticleEmail, new { @class="charCount_textArticleEmail" }) %>
            <%: Html.ValidationMessageFor(model => model.ArticleEmail) %>
            <p class="note">
                If you don’t want to sign up, provide an email address here so you can edit this article later and eventually create an account.
            </p>
        </div>
        <% } %>

        <div class="editor-label"></div>
        <div class="editor-field-large"><%: Html.ValidationSummary(true, "Errors found in your article. Please correct them and re-submit.") %></div>

        <div class="editor-submit">
            <input id="submitType" type="hidden" name="submitType" value="Save" />
            <input id="publishButton" type="submit" value="Publish" class="red" />  <!-- TODO replace value with Html.Encode(Resources.Messages.Publish) for globalization -->
            <input id="saveButton" type="submit" value="Save" />
            <input id="previewButton" type="submit" value="Preview" />
        </div>

    <% } %>

    <script src="<%= Links.Content.Scripts.thirdparty.charCount_js %>" type="text/javascript"></script>
    <script src="<%= Links.Content.Scripts.publish_article_js %>" type="text/javascript"></script>
    <script src="<%= Links.Content.tiny_mce.jquery_tinymce_js %>" type="text/javascript"></script>
</div>