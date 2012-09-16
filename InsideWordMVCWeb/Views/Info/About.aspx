<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("About") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="info about">
        <h1>About Us</h1>
        <div class="h1hr"></div>
        <br />
        <h3><i>InsideWord – Making it easy to publish, share and discover your articles.</i></h3>
        <div class="h3hr"></div>
        <p>With the influx of blogs, social networking sites, forums, message boards, and the many other mediums of communication out there, finding a single place to easily publish, share and discover your articles simply does not exist.</p>
        <p>Blogs allow you to create content and share it on the web. But getting the word out and attracting readers to your website is nearly impossible. It doesn’t mean your content isn’t any good, but how are you meant to easily share your blog when it’s the 7 millionth search result?</p>
        <p>Social networking sites aren’t any better. Built for sharing pictures and posting comments, they were never designed to share lengthy content. And if you’re able to figure out a way, it’s usually cumbersome and restricted to your immediate friends. How are other people meant to easily discover interesting content you’ve written?</p>
        <p><i>InsideWord</i> is going to change that.</p>
        <p><i>InsideWord</i> is your one-stop shop to effortlessly publish your own articles, share them with the greatest number of people possible, and discover interesting and highly ranked articles written by others. And it’s all 100% free – we simply want the act of sharing user-created articles to be as easy as possible, and build a vibrant community around them.</p>
        <br />
        <h3>InsideWord Team</h3>
        <div class="h3hr"></div>
        <ul>
            <li>Aaron</li>
            <li>Jesse</li>
            <li>Marc</li>
        </ul>
    </div>
</asp:Content>
