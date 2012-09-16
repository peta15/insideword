<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Shared.AddMediaVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.article_submit_css %>" media="all" />
    <%= Html.Title("Change Profile Picture") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

<h1>Change Profile Picture</h1>
<div class="h1hr"></div>

<% Html.RenderPartial(MVC.Child.Views.AddMedia, Model); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
