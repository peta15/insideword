<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.CategoryManagementVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Category Management") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <h1>Category Management</h1>
    <div class="h1hr"></div>

    <div class="editor-label">
        <label>New Category:</label>
    </div>
    <div class="editor-field">
        <%: Html.ActionLink("Add", MVC.Admin.CategoryEdit()) %>
    </div>
    
    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>

        <div class="editor-label">
            <label>Categories:</label>
        </div>
        <div id="navEditGrid" class="editor-field"></div>

        <div class="editor-submit">
            <input type="submit" value="Submit" />
         </div>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script language="javascript" type="text/javascript" src="<%: Links.Content.Scripts.category_js %>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $.getJSON('<%: Url.Action( MVC.API.Category() ) %>', null, function (data) {
                // modify the builder object's edit url
                categoryLiBuilder.editUrl = '<%: Url.Action( MVC.Admin.CategoryEdit() ) %>';
                $("#navEditGrid").createList(data.Children, categoryLiBuilder);
            });
        });
    </script>
</asp:Content>