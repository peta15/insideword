<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Publish By Email") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="info publishByEmail">
        <h1>Publish By E-mail</h1>
        <div class="h1hr"></div>
        <br />
        <ol>
            <li><a href="#faq1">What is publish by e-mail?</a></li>
            <li><a href="#faq2">How do I publish by e-mail?</a></li>
            <li><a href="#faq3">How do I set the title of an article?</a></li>
            <li><a href="#faq4">How do I set the category of an article?</a></li>
            <li><a href="#faq5">Help! All my articles are being saved as drafts and never published.</a></li>
            <!--<li><a href="#faq6">How do I add pictures?</a></li>-->
            <li><a href="#faq7">How do I save my e-mail articles as drafts instead of publishing them?</a></li>
            <li><a href="#faq8">How do I edit an article by e-mail?</a></li>
            <li><a href="#faq9">Where can I find an article's id number?</a></li>
            <li><a href="#faq10">How do I change my article's title?</a></li>
            <li><a href="#faq11">How do I append to my article rather than overwrite it?</a></li>
            <li><a href="#faq12">How do I change my article from draft to publish?</a></li>
            <li><a href="#faq13">How do I delete an article by e-mail?</a></li>
            <li><a href="#faq14">What are e-mail commands?</a></li>
            <li><a href="#faq15">What are all the e-mail commands?</a></li>
        </ol>
        <dl id="faq1">
            <dt>What is publish by e-mail?</dt><dd>
                <p>
                    Publish by e-mail allows you to submit articles to InsideWord by e-mail, so that you can publish or draft things quickly from your mobile phone.
                </p>
            </dd>
        </dl>
        <dl id="faq2">
            <dt>How do I publish by e-mail?</dt><dd>
                <p>
                    Simply send an e-mail to publish@insideword.com. Your e-mail subject is the title of the article and the e-mail body is where your text goes.
                </p>
            </dd>
        </dl>
        <dl id="faq3">
            <dt>How do I set the title of an article?</dt><dd>
                <p>
                    The "subject" line of your e-mail is the article's title.
                </p>
            </dd>
        </dl>
        <dl id="faq4">
            <dt>How do I set the category of an article?</dt><dd>
                <p>
                    You can set the category of an article by writting [category <i>some category</i>] or [c <i>some category</i>] on the first line of the e-mail.
                    If you don't include a category in your article then it will not be saved as a draft instead of published.
                </p>
            </dd>
        </dl>
        <dl id="faq5">
            <dt>Help! All my articles are being saved as drafts and never published.</dt><dd>
                <p>See the following:</p>
                <ul>
                     <li><a href="#faq4">How do I set the category of an article?</a></li>
                     <li><a href="#faq12">How do I change my article from draft to publish?</a></li>
                     <li><a href="#faq7">How do I save my e-mail articles as drafts instead of publishing them?</a></li>
                </ul>
            </dd>
        </dl>
        <!--
        <dl id="faq6">
            <dt>How do I add pictures?</dt><dd>
                <p>Attach a picture to your e-mail and it will be added.</p>
            </dd>
        </dl>
        -->
        <dl id="faq7">
            <dt>How do I save my e-mail articles as drafts instead of publishing them?</dt><dd>
                <p>
                    You can set your articles as draft by writing [status draft] or [s d] or [draft] or [d] on the first line of the e-mail.
                </p>
            </dd>
        </dl>
        <dl id="faq8">
            <dt>How do I edit an article by e-mail?</dt><dd>
                <p>
                    Write the article's id in the "subject" of your article. Becareful however when doing this. By default your article will be deleted and replaced with your new text. If you simply want to add to it then see the following:
                </p>
                <ul>
                     <li><a href="#faq11">How do I append to my article rather than overwrite it?</a></li>
                </ul>
            </dd>
        </dl>
        <dl id="faq9">
            <dt>Where can I find an article's id number?</dt><dd>
                <p>
                    Every time you successfully publish by e-mail you will receive a response e-mail that will provide the id of the article.
                </p>
            </dd>
        </dl>
        <dl id="faq10">
            <dt>How do I change my article's title</dt><dd>
                <p>
                    First make sure that you set the e-mail "subject" to your article's id number so that we know which article you want to edit.
                    Then you can change the title to an article by writing [title <i>your new title</i>] or [t <i>your new title</i>] in the first lines of the e-mail.
                </p>
            </dd>
        </dl>
        <dl id="faq11">
            <dt>How do I append to my article rather than overwrite it?</dt><dd>
                <p>
                    First make sure that you set the e-mail "subject" to your article's id number so that we know which article you want to edit.
                    You can append to an article by writing [append] or [a] in the first lines of the e-mail.
                </p>
            </dd>
        </dl>
        <dl id="faq12">
            <dt>How do I change my article from draft to publish?</dt><dd>
                <p>
                    First make sure that you set the e-mail "subject" to your article's id number so that we know which article you want to edit.
                    You can set your articles as publish by writing [status publish] or [s p] or [publish] or [p] on the first lines of the e-mail.
                </p>
            </dd>
        </dl>
        <dl id="faq13">
            <dt>How do I delete an article by e-mail?</dt><dd>
                <p>
                    First make sure that you set the e-mail "subject" to your article's id number so that we know which article you want to edit.
                    You can delete an article by writing [delete] on the first lines of the e-mail.
                </p>
            </dd>
        </dl>
        <dl id="faq14">
            <dt>What are e-mail commands?</dt><dd>
                <p>
                    E-mail commands are special lines of text you write in the beginning of your e-mail.
                    They always look like text inside square brackets like this, [<i>blah blah blah</i>].
                </p>
            </dd>
        </dl>
        <dl id="faq15">
            <dt>What are all the e-mail commands?</dt><dd>
                <p>
                    There are many e-mail commands. You can get a list of all of them and what they do by sending an e-mail to publish@insideword.com and by writing help in the subject of your e-mail. You don't have to enter any text in the e-mail's body.
                </p>
            </dd>
        </dl>
    </div>
</asp:Content>