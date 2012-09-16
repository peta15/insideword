<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("FAQ") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="info faq">
        <h1>FAQ</h1>
        <div class="h1hr"></div>
        <br />
        <ol>
            <li><a href="#faq1">What is <i>InsideWord</i>?</a></li>
            <li><a href="#faq2">Why should I use <i>InsideWord</i>?</a></li>
            <li><a href="#faq3">Why do I need to sign up?</a></li>
            <li><a href="#faq4">How do I write/publish an article?</a></li>
            <li><a href="#faq5">Can I include photos in my articles?</a></li>
            <li><a href="#faq6">Is there a word limit to how much I can write?</a></li>
            <li><a href="#faq7">What happens when I publish an article?</a></li>
            <li><a href="#faq8">Can I edit or remove an article I’ve already published?</a></li>
            <li><a href="#faq9">Do I own the content I publish on <i>InsideWord</i>?</a></li>
            <li><a href="#faq10">How are articles ranked on <i>InsideWord</i>?</a></li>
            <li><a href="#faq11">How do articles get better placement on <i>InsideWord</i>?</a></li>
            <li><a href="#faq12">What does voting do?</a></li>
            <li><a href="#faq13">How do I vote?</a></li>
            <li><a href="#faq14">What is a blurb?</a></li>
            <li><a href="#faq15">How can I find the most popular articles?</a></li>
            <li><a href="#faq16">How can I find the newest articles?</a></li>
            <li><a href="#faq17">How can I find older articles?</a></li>
            <li><a href="#faq18">What are categories and how do they work?</a></li>
            <li><a href="#faq19">Can I comment on an article?</a></li>
            <li><a href="#faq20">How do I comment on an article?</a></li>
            <li><a href="#faq21">What is a Member Profile?</a></li>
            <li><a href="#faq22">What does the Flag button do?</a></li>
            <li><a href="#faq23">What is Just Published at the top of my screen?</a></li>
            <li><a href="#faq24">Can I share articles outside of <i>InsideWord</i>?</a></li>
            <li><a href="#faq25">Are there any more features coming?</a></li>
        </ol>
        <dl id="faq1">
            <dt>What is <i>InsideWord</i>?</dt><dd>
                <p>
                    <i>InsideWord</i> makes it easy to publish, share and discover your articles. You
                    can publish articles about anything — your interests, opinions, news, insights or
                    analysis — share them with your friends and the <i>InsideWord</i> community, and
                    discover articles written by others.
                </p>
                <p>
                    Whether you write or read a blog, have something to say or are interested in reading
                    about, <i>InsideWord</i> is the place for you.</p>
            </dd>
        </dl>
        <dl id="faq2">
            <dt>Why should I use <i>InsideWord</i>?</dt><dd>
                <p>
                    <i>InsideWord</i> is your one-stop shop for effortlessly publishing articles, sharing
                    them with the greatest number of people possible, and discovering interesting and
                    highly ranked articles written by others. And it’s all 100% free – <i>InsideWord</i>
                    simply wants the act of sharing user-created articles to be as easy as possible,
                    and build a vibrant community around them.</p>
            </dd>
        </dl>
        <dl id="faq3">
            <dt>Why do I need to sign up?</dt><dd>
                <p>
                    Signing up is free. You will need to sign up to publish new articles so you can
                    be credited for what you’ve written. You will also need to be signed in to post
                    comments and vote on articles.</p>
            </dd>
        </dl>
        <dl id="faq4">
            <dt>How do I write/publish an article?</dt><dd>
                <p>
                    Just click the Publish Article button and you’re ready to go. You can use the Article
                    Body section to write your article or simply copy and paste your article from somewhere
                    else.</p>
                <p>
                    Only 3 fields are required to publish an article:</p>
                <ol>
                    <li>A Title </li>
                    <li>A Category </li>
                    <li>Your article </li>
                </ol>
            </dd>
        </dl>
        <dl id="faq5">
            <dt>Can I include photos with my article?</dt><dd>
                <p>
                    Yes. You have the option to upload a photo, as well as include the photographer’s
                    name and a write photo caption. This picture will also be used in the article’s
                    blub.</p>
            </dd>
        </dl>
        <dl id="faq6">
            <dt>Is there a word limit to how much I can write?</dt><dd>
                <p>
                    No. There are some character limits for the titles and blurbs, but there is no
                    word limit to how much you can write in an article.</p>
            </dd>
        </dl>
        <dl id="faq7">
            <dt>What happens when I publish an article?</dt><dd>
                <p>
                    When you publish a new article, it instantly gets published to <i>InsideWord</i>. Check
                    out the Just Published banner at the top of your screen to see it.</p>
                <p>
                    Everyone will immediately be able to read your article, vote on it, and write comments.
                    It can be found by either navigating to it through the categories on the left of
                    your screen, from your user profile page, or searching for it.</p>
                <p>
                    Over time your article may get an increasingly prominent place on the website which
                    will allow more and more users to discover your article.</p>
            </dd>
        </dl>
        <dl id="faq8">
            <dt>Can I edit or remove an article I’ve already published?</dt><dd>
                <p>
                    Yes. Simply login and navigate to your member profile page. There will be a list
                    of links so you can edit or remove each article you’ve published.</p>
            </dd>
        </dl>
        <dl id="faq9">
            <dt>Do I own the content I publish on <i>InsideWord</i>?</dt><dd>
                <p>
                    Yes. You own all the content you submit to the site.
                </p>
                <p>
                    For clarity, by contributing to <i>InsideWord</i>, you also grant <i>InsideWord</i>
                    the right to use your content—a perpetual, irrevocable, transferable, non-exclusive,
                    worldwide, royalty-free licence to use, store, redistribute, edit, and modify your
                    content.</p>
                <p>
                    Please read our <%: Html.ActionLink( "Terms of Use", MVC.Info.Terms() ) %> and
                    <%: Html.ActionLink("Privacy Policy", MVC.Info.Privacy())%> for further detail.</p>
            </dd>
        </dl>
        <dl id="faq10">
            <dt>How are articles ranked on <i>InsideWord</i>?</dt><dd>
                <p>
                    <i>InsideWord</i> uses a complex algorithm to automatically rank articles based
                    on popularity. The more people that enjoy your article, the higher it will get ranked
                    on the website.
                </p>
            </dd>
        </dl>
        <dl id="faq11">
            <dt>How do articles get better placement on <i>InsideWord</i>?</dt><dd>
                <p>
                    It is a completely automated process. Nobody at <i>InsideWord</i> is able to manually
                    promote or rank an article higher. Instead, <i>InsideWord</i> is utilizing a complex
                    algorithm which determines where articles appear.
                </p>
                <p>
                    There are a variety of factors that go into determining the placement of an article
                    on the website. If you publish an article, we encourage you to share it with your
                    friends, which may benefit its placement on the website.</p>
            </dd>
        </dl>
        <dl id="faq12">
            <dt>What does voting do?</dt><dd>
                <p>
                    Voting allows you to express whether you like or dislike an article. You are only
                    allowed to vote once per article.</p>
                <p>
                    <i>InsideWord</i> will automatically promote articles that people like. Voting is
                    one of the many variables that <i>InsideWord</i> employs to determine whether an
                    article should receive a prominent place on the website.
                </p>
            </dd>
        </dl>
        <dl id="faq13">
            <dt>How do I vote?</dt><dd>
                <p>
                    At the end of every article you can click either a green thumbs up or a red thumbs
                    down button. You are only allowed to vote once per article.
                </p>
                <p>
                    After you vote, the net number of total votes will appear (a combination of the
                    upvotes and downvotes) and the thumbs will grey out.</p>
            </dd>
        </dl>
        <dl id="faq14">
            <dt>What is a blurb?</dt><dd>
                <p>
                    A blurb is the brief description that contains a summary of what your article is
                    about. Blurbs can be found on the homepage, in the category pages, and in your user
                    profile. Blurbs contain your article’s title, a photo, your username, and a brief
                    description.
                </p>
            </dd>
        </dl>
        <dl id="faq15">
            <dt>How can I find the most popular articles?</dt><dd>
                <p>
                    The most popular articles will appear at the top of the list of articles.
                </p>
            </dd>
        </dl>
        <dl id="faq16">
            <dt>How can I find the newest articles?</dt><dd>
                <p>
                    You can find new articles by scrolling down the pages. The webpage will automatically
                    expand revealing more and more articles. The newest articles appear at the bottom
                    and the more popular articles appear at the top. As people read, comment and vote
                    on the articles, more popular content will appear further up the list of articles.</p>
                <p>
                    Articles will also be archived in the author’s member profile. Simply click the
                    author’s name to go to his or hers member profile page.</p>
            </dd>
        </dl>
        <dl id="faq17">
            <dt>How can I find older articles?</dt><dd>
                <p>
                    Older articles will appear below the newest articles on the pages. To find older
                    articles, you can either scroll down the page or search for them in the search bar.
                    Over time, popular articles will slowly lose their rank at the top of the article
                    list and can be found when you scroll down.</p>
                <p>
                    Articles will also be archived in the author’s member profile. Simply click the
                    author’s name to go to his or hers member profile page.</p>
            </dd>
        </dl>
        <dl id="faq18">
            <dt>What are Categories and how do they work?</dt><dd>
                <p>
                    Categories can be found on the left side of your screen. They allow you to navigate
                    to a page devoted to a specific topic of interest where all the articles relate
                    to one another.</p>
            </dd>
        </dl>
        <dl id="faq19">
            <dt>Can I comment on an article?</dt><dd>
                <p>
                    Yes. You will need to be signed into <i>InsideWord</i> to do so. Simply click the
                    article and scroll to the bottom. You can also click the Comments link in an article’s
                    blurb to take you directly to the article’s comment section. Please be mindful that
                    inappropriate comments will be flagged and deleted from the website.</p>
            </dd>
        </dl>
        <dl id="faq20">
            <dt>How do I comment on an article?</dt><dd>
                <p>
                    You can either Start a Conversation or reply to an existing conversation. Starting
                    a conversation allows you to start a new comment thread. Clicking Reply allows you
                    to comment within an existing conversation.</p>
            </dd>
        </dl>
        <dl id="faq21">
            <dt>What is your Member Profile?</dt><dd>
                <p>
                    A member profile is your personal space on <i>InsideWord</i>. Simply sign up to
                    <i>InsideWord</i> and click on your username to go to your member profile.</p>
                <p>
                    Your username will appear in every article you publish. Readers can click on it
                    to take them to your profile so they can discover other articles you’ve written.
                </p>
                <p>
                    When you are logged in, you’ll be able to edit or remove articles you’ve written,
                    change your password and username, and upload a profile picture.</p>
            </dd>
        </dl>
        <dl id="faq22">
            <dt>What does the Flag button do?</dt><dd>
                <p>
                    The Flag button allows readers to alert <i>InsideWord</i> to inappropriate content
                    (both articles and comments), such as advertisements, profane language, and indecent
                    content. Flagged articles and comments will be deleted very quickly if they infringe
                    on <i>InsideWord</i>’s <%: Html.ActionLink("Community Guidelines", MVC.Info.Guidelines()) %>.
                    For repeat infringers, the member’s account may be deleted too.</p>
            </dd>
        </dl>
        <dl id="faq23">
            <dt>What is Just Published at the top of my screen?</dt><dd>
                <p>
                    Just Published features the titles of the most recent articles published on <i>InsideWord</i>.
                    Clicking on a title will redirect you to that article.</p>
                <p>
                    Tip: You can also quickly scan the newest titles by hovering your mouse over
                    them and holding down the ctrl key + scrolling up or down on your mouse wheel. Or,
                    hover you mouse over the titles and click the left or right arrows on your keyboard.</p>
            </dd>
        </dl>
        <dl id="faq24">
            <dt>Can I share articles outside of <i>InsideWord</i>?</dt><dd>
                <p>
                    Yes. Each article has a Share link. Simply click this link and you will have the
                    option of sharing the article by means of your email or a social networking site.</p>
            </dd>
        </dl>
        <dl id="faq25">
            <dt>Are there any more features coming?</dt><dd>
                <p>
                    Yes, a lot more. Please leave us <%: Html.ActionLink("feedback", MVC.Info.ContactUs()) %>
                    as well. We read all of it and will try our best to implement your suggestions as
                    quickly as possible.</p>
            </dd>
        </dl>
    </div>
</asp:Content>
