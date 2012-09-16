<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Shared.AddMediaDoneVM>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.global_css %>" media="all" />
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.article_submit_css %>" media="all" />
    <%= Html.Title("Upload Media") %>
</head>
<body>

    <h3>Uploaded</h3>
	<div class="h3hr"></div>

    File URL: <span id="fileURL"><%: Model.FileURL %></span>

    <script type="text/javascript" src="<%: Links.Content.tiny_mce.tiny_mce_popup_js %>"></script>
    <script type="text/javascript" src="<%: Links.Content.Scripts.addmedia_js %>"></script>
</body>
</html>
