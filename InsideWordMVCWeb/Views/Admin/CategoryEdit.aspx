<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.CategoryEditVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Category Edit") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h1>Category Edit</h1>
    <div class="h1hr"></div>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>

        <%: Html.HiddenFor(model => model.Id) %>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.ParentId) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.ParentId, Model.PotentialParentList) %>
            <%: Html.ValidationMessageFor(model => model.ParentId) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Title) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Title) %>
            <%: Html.ValidationMessageFor(model => model.Title) %>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.IsHidden) %>
        </div>
        <div class="editor-field">
            <%: Html.CheckBoxFor(model => model.IsHidden) %>
            <%: Html.ValidationMessageFor(model => model.IsHidden) %>
        </div>
            
        <div class="editor-submit">
            <input type="submit" value="Submit" />
         </div>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", MVC.Admin.Category()) %>
    </div>

</asp:Content>

