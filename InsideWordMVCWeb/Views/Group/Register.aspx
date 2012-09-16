<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Group.GroupRegisterVM>" %>

<asp:Content ID="Content4" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Group Register") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h1>Group Register</h1>
    <div class="h1hr"></div>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
            
        <div class="editor-submit">
            <input type="submit" value="Register" />
        </div>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>

