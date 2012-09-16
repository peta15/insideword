<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Shared.AddMediaVM>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.global_css %>" media="all" />
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.article_submit_css %>" media="all" />
    <%= Html.Title("Upload Media") %>
</head>
<body>
    <br />
    <br />
    <% Html.RenderPartial("~/Views/Child/AddMedia.ascx", Model); %>

</body>
</html>
