<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Guidelines") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="info guidelines">
        <h1>Community Guidelines</h1>
        <div class="h1hr"></div>
        <br />
        <p>
            We have established these Community Guidelines for your safety, to keep the articles
            and comments respectful, and to foster a healthy environment for discussion.</p>
        <dl>
            <dt>We Review and Remove User’s Accounts Whose Username is Inappropriate</dt><dd>
                <ul>
                    <li>Contains website or email addresses</li>
                    <li>Contains contact information (i.e. phone numbers, address, etc.)</li>
                    <li>Appears to impersonate someone else</li>
                    <li>Contains swear words or are otherwise objectionable</li></ul>
            </dd>
        </dl>
        <dl>
            <dt>We Review and Remove Articles and Comments Flagged as Inappropriate</dt><dd>
                <ul>
                    <li>When an article or comment gets flagged, we review it to determine whether it violates
                        our Terms of Use</li></ul>
            </dd>
        </dl>
        <dl>
            <dt>Please Do Not Cross the Line</dt><dd>
                <p>
                    We encourage free speech and defend everyone’s right to express unpopular points
                    of view. However, we reserve the right to delete articles and comments which violate
                    these common-sense issues:</p>
                <ul>
                    <li>Hate speech (speech which attacks or demeans a group based on race or ethnic origin,
                        religion, disability, gender, age, veteran status, and sexual orientation)</li>
                    <li>Copyright violation. Only publish articles and comments that you made or are authorized
                        to use</li>
                    <li>Contains swear words for the purpose of malicious intent or other language likely
                        to offend</li>
                    <li>Breaks the law or condones or encourages unlawful activity. This includes breach
                        of copyright, defamation and contempt of court</li>
                    <li>Advertises products or services for profit or gain</li>
                    <li>Are seen to impersonate someone else</li>
                    <li>Include contact details such as phone numbers, postal or email addresses</li>
                    <li>Are written in anything other than English</li>
                    <li>Describes or encourages activities which could endanger the safety or well-being
                        of others</li>
                    <li>Are considered to be 'spam', such as posts containing the same or similar message
                        posted multiple times</li>
                    <li>Are considered to be off-topic for the particular message board</li>
                </ul>
                <p>
                    Please take these rules seriously. All we ask is you try your best to understand
                    them and respect the spirit in which they were created.
                </p>
                <p>
                    If your account is terminated you are prohibited from creating any new accounts.</p>
            </dd>
        </dl>
        <dl>
            <dt><i>InsideWord</i> is for the Community</dt><dd>
                <p>
                    This is your community! Each and every user of <i>InsideWord</i> makes the site what it
                    is, so don’t be afraid to get involved.</p>
                <ul>
                    <li>Have fun with the site</li>
                    <li>Give feedback</li>
                    <li>You may not like everything you see</li>
                </ul>
            </dd>
        </dl>
        <p>
            <i>Thanks for reading!</i></p>
    </div>
</asp:Content>