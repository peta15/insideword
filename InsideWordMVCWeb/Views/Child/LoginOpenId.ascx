<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.LoginOpenIdVM>" %>

<!-- InsideWord_partial class is used by partial.js to allow ajax validation of the partial -->
<div class="InsideWord_partial">
    <script language="javascript" type="text/javascript" src="<%: Links.Content.Scripts.loginOpenId_js %>"></script>
    <% using( Html.BeginForm(MVC.Child.LoginOpenId(), FormMethod.Post, new { @class="openid" }) ) { %>
        <%= Html.ValidationSummary(true) %>
        <div><ul class="providers">
            <li class="direct" title="Google">
                <div class="googleimg"></div><span>https://www.google.com/accounts/o8/id</span></li>
            <li class="direct" title="Yahoo">
		        <div class="yahooimg"></div><span>http://yahoo.com/</span></li>
            <li class="username" title="AOL screen name">
		        <div class="aolimg"></div><span>http://openid.aol.com/<strong>username</strong></span></li>
            <li class="openid" title="OpenID">
                <div class="openidimg"></div><span><strong>http://{your-openid-url}</strong></span></li>
            <li class="username" title="MyOpenID user name">
		        <div class="myopenidusernameimg"></div><span>http://<strong>username</strong>.myopenid.com/</span></li>
            <li class="username" title="Flickr user name">
		        <div class="flickrimg"></div><span>http://flickr.com/<strong>username</strong>/</span></li>
            <li class="username" title="Technorati user name">
		        <div class="technoratiimg"></div><span>http://technorati.com/people/technorati/<strong>username</strong>/</span></li>
            <li class="username" title="Wordpress blog name">
		        <div class="wordpressimg"></div><span>http://<strong>username</strong>.wordpress.com</span></li>
            <li class="username" title="Blogger blog name">
		        <div class="bloggerimg"></div><span>http://<strong>username</strong>.blogspot.com/</span></li>
            <li class="username" title="LiveJournal blog name">
		        <div class="livejournalimg"></div><span>http://<strong>username</strong>.livejournal.com</span></li>
            <li class="username" title="ClaimID user name">
		        <div class="claimidimg"></div><span>http://claimid.com/<strong>username</strong></span></li>
            <li class="username" title="Vidoop user name">
		        <div class="vidoopimg"></div><span>http://<strong>username</strong>.myvidoop.com/</span></li>
            <li class="username" title="Verisign user name">
		        <div class="verisignimg"></div><span>http://<strong>username</strong>.pip.verisignlabs.com/</span></li> 
        </ul></div> 
        <fieldset> 
            <label for="openid_username">Enter your <span>Provider user name</span></label> 
            <div>
                <span></span>
                <input type="text" name="openid_username" />
                <span></span>
                <input type="submit" value="Login" />
            </div> 
        </fieldset> 
        <fieldset>
            <div class="editor-label">
                <label for="openid_identifier">Enter your <a class="openid_logo" href="http://openid.net">OpenID</a></label>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.openid_identifier)%>
                <%: Html.ValidationMessageFor(model => model.openid_identifier)%>  
            </div>

            <div class="editor-submit">
                <input type="submit" value="Login" />
            </div>
        </fieldset>
        <fieldset>
            <img name="openid_loader" src="<%: Links.Content.img.@interface.loader_bar2_gif %>" alt="please wait" />
        </fieldset>
        <fieldset>
            <div>
                <b>What is OpenId login?</b>
                Use it to login with your Google, Yahoo or AOL account. Just click on one of the pictures to begin.
            </div>
        </fieldset>
    <% } %>
</div>

