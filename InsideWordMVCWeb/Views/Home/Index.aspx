<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Home.IndexVM>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title(Model.PageTitle) %>
    <link rel="stylesheet" type="text/css" href="<%: Links.Content.css.index_css %>" media="all" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.thirdparty.jquery_masonry_min_js %>"></script>
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.modded.jquery_infinitescroll_js %>"></script>
    <script language="javascript" type="text/javascript" src="<%= Links.Content.Scripts.home_js %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="readerWriterSliderHolder">
        <div class="linkHolder">
            <a class="sliderPrev"><b>I Want To Read</b></a>
        </div>
        <div class="sliderHolder">
            <div id="readerWriterSlider"></div>
            <div class="cls"></div>
        </div>
        <div class="linkHolder">
            <a class="sliderNext"><b>I Want To Write</b></a>
        </div>
        <div class="cls"></div>
    </div>
    
    <div class="sliderIndex">
        <div class="sliderItems">
            <div class="sliderDiv">
                <% if (Model.ShowTitle) { %>
                    <h1 class="indexTitle"><%: Model.PageTitle %></h1>
                <% } else { %>
                    <div style="padding: 4px; text-align: center; font-size: 14px;">
                        <b>Write and promote your content without having to create a blog</b>
                    </div>
                <% } %>
                <div class="h1hr"></div>
    
                <% if (Model.BlurbList.Count == 0) { %>
                    <br class="cls" />
                    <h3 class="center">No articles on this page. <%: Html.ActionLink("Publish one", MVC.Article.ArticleEdit(null, null))%> or go to the <%: Html.ActionLink( "home page", MVC.Home.Index(null, null) ) %>.</h3>
                <% } else { %>
                    <div class="blurbList">
                        <% foreach (var item in Model.BlurbList) {
                            Html.RenderAction( MVC.Child.Blurb(item) );
                        } %>
                    </div>

                    <% if (!Model.IsLastPage) { %>
                        <p id="infscr-pageNav" class="center"><%: Html.ActionLink("More", MVC.Home.Index(Model.Category.Id, Model.NextPage), new { @class = "button buttonIndexWide" })%></p>
                    <% } %>
                <% } %>
            </div>
            <div class="sliderDiv">
                <% Html.RenderAction(MVC.Article.ArticleEditor(null)); %>
            </div>
        </div>
    </div>
</asp:Content>
