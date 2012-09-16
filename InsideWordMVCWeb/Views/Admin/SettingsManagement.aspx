<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.SettingsManagementVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <% using (Html.BeginForm( MVC.Admin.SettingsManagement() )) { %>
        <%: Html.ValidationSummary(true) %>
        
        <h1>InsideWord Settings</h1>
		<div class="h1hr"></div>
        
        <div class="editor-label">
            <%: Html.LabelFor(model => model.DefaultCategoryId)%>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.DefaultCategoryId, Model.CategoryList)%>
            <%: Html.ValidationMessageFor(model => model.DefaultCategoryId)%>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.LogThresholdName)%>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.LogThresholdName, Model.LogLevelList)%>
            <%: Html.ValidationMessageFor(model => model.LogThresholdName)%>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.LogEmailThresholdName)%>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.LogEmailThresholdName, Model.LogEmailLevelList)%>
            <%: Html.ValidationMessageFor(model => model.LogEmailThresholdName)%>
        </div>

        <div class="editor-submit">
            <input type="submit" value="Submit" />
        </div>

    <% } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
