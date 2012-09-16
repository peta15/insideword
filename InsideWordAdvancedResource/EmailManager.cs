using System.Net.Mail;
using InsideWordProvider;
using System;

namespace InsideWordAdvancedResource
{
    public class EmailManager
    {
        protected const string _tab = "&nbsp;&nbsp;&nbsp;&nbsp;";
        protected Random _roll;
        public SmtpClient DefaultSmtp { get; set; }
        public string HttpHost { get; set; }

        public void SendActivationEmail(MailAddress email, ProviderMember aMember)
        {
            ProviderIssuedKey nonceIssuedKey = new ProviderIssuedKey();
            nonceIssuedKey.LoadOrCreate(aMember.Id.Value, email.Address, true, null, true);

            string activateUrl = HttpHost + "member/validate_email/" + nonceIssuedKey.IssuedKey;
            string deleteUrl = HttpHost + "member/delete/" + nonceIssuedKey.IssuedKey;
            string emailBody = "DO NOT SHARE THIS E-MAIL OR REPLY TO THIS E-MAIL<br />"
                            + "<br />"
                            + "<br />"
                            + "Click the link below to validate your e-mail with InsideWord:<br />"
                            + "<a href='" + activateUrl + "'>VALIDATE E-MAIL</a><br />"
                            + "<br />"
                            + "Don't know what this e-mail is about? Chances are someone used your e-mail by accident. Select the link below to delete the account:<br />"
                            + "<a href='" + deleteUrl + "'>DELETE ACCOUNT</a><br />"
                            + "<br />"
                            + "<br />"
                            + "<br />";
                            //+ DidYouKnow();

            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        "InsideWord - e-mail validation",
                                                        emailBody);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);
        }

        public void SendPasswordResetEmail(MailAddress email, ProviderMember aMember)
        {
            string passwordResetUrl = HttpHost + "member/change_password/" + aMember.CurrentMonthIssuedKey.IssuedKey;

            string emailBody = "DO NOT SHARE THIS E-MAIL OR REPLY TO THIS E-MAIL<br />"
                            + "<br />"
                            + "<br />"
                            + "Click the link below to choose your new password:<br />"
                            + "<a href='" + passwordResetUrl + "'>RESET PASSWORD</a><br />"
                            + "<br />"
                            + "<br />"
                            + "<br />";
                            //+ DidYouKnow();

            
            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        "InsideWord - reset password e-mail",
                                                        emailBody);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);
        }

        /// <summary>
        /// Function to send an Edit/Delete e-mail to a member.
        /// </summary>
        /// <param name="anArticle">Article that member would like to edit/delete</param>
        /// <param name="aMember">Member that will receive the e-mail</param>
        /// <returns>true if the e-mail was sent successfully and false otherwise.</returns>
        public bool SendEditArticleEmail(MailAddress email, ProviderArticle anArticle, ProviderMember aMember)
        {
            ProviderIssuedKey nonceIssuedKey = new ProviderIssuedKey();
            nonceIssuedKey.LoadOrCreate(aMember.Id.Value, email.Address, true, null, true);

            // create a month issued key for editing the article
            ProviderIssuedKey monthExpiry = new ProviderIssuedKey();
            monthExpiry.LoadOrCreate(aMember.Id.Value, email.Address, false, 1, true);

            string editUrl = HttpHost + "article/edit/" + anArticle.Id.Value + "/" + monthExpiry.IssuedKey;
            string activateUrl = HttpHost + "member/change_password/" + nonceIssuedKey.IssuedKey;
            string deleteUrl = HttpHost + "member/delete/" + nonceIssuedKey.IssuedKey;
            string submitState = "";
            if (anArticle.IsPublished)
            {
                submitState = "Your article was submited to our system.";
            }
            else
            {
                submitState = "Your article was submited to our system as a draft.";
            }
            string category;
            if(anArticle.CategoryIds.Count > 0)
            {
                category = (new ProviderCategory(anArticle.CategoryIds[0])).Title;
            }
            else
            {
                category = "none";
            }

            string emailBody = "DO NOT SHARE THIS E-MAIL OR REPLY TO THIS E-MAIL<br />"
                            + "<br />"
                            + "<br />"
                            + submitState + "<br />"
                            + "Id: "+anArticle.Id.Value.ToString()+"<br />"
                            + "Title: "+anArticle.Title +"<br />"
                            + "Category: " + category + "<br />"
                            + "<br />"
                            + "Click the link below to edit/delete your article at InsideWord:<br />"
                            + "<a href='" + editUrl + "'>EDIT ARTICLE</a><br />"
                            + "<br />";
            
            if (!aMember.IsActive)
            {
                emailBody += "Click the link below to finish activating your account:<br />"
                            + "<a href='" + activateUrl + "'>FINISH ACTIVATING ACCOUNT</a><br />"
                            + "<br />"
                            + "Don't know what this e-mail is about? Chances are someone used your e-mail by accident. Select the link below to delete the account:<br />"
                            + "<a href='" + deleteUrl + "'>DELETE ACCOUNT</a><br />"
                            + "<br />";
            }

            emailBody += "<br />"
                        + "<br />"
                        + "<br />";
                        //+ DidYouKnow();
            
            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        "InsideWord - edit article " + anArticle.Title,
                                                        emailBody);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);
            return true;
        }

        /// <summary>
        /// Function to send an notification that the article was commented on
        /// </summary>
        /// <param name="anArticle">Article that was commented on</param>
        /// <param name="aMember">Member that will receive the e-mail</param>
        /// <returns>true if the e-mail was sent successfully and false otherwise.</returns>
        public bool SendCommentNotificationEmail(MailAddress email, ProviderArticle anArticle, ProviderMember commentAuthor)
        {
            string articleUrl = HttpHost + "article/" + anArticle.Id;
            string memberUrl = string.Empty;
            if (commentAuthor.IsAnonymous)
            {
                memberUrl = "Anonymous";
            }
            else
            {
                memberUrl = "<a href='"+HttpHost + "member/profile/" + commentAuthor.Id.Value+"'>" + commentAuthor.DisplayName + "</a>";
            }

            string emailBody = "DO NOT REPLY TO THIS E-MAIL<br />"
                + "<br />"
                + "<br />"
                + memberUrl + " has left a comment on your article, <a href='" + articleUrl + "'>" + anArticle.Title + "</a>"
                + "<br />"
                + "<br />"
                + "<br />";
                //+ DidYouKnow();

            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        "InsideWord - Someone commented on your article! " + anArticle.Title,
                                                        emailBody);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);
            return true;
        }

        /// <summary>
        /// Function to send an notification that the article was commented on
        /// </summary>
        /// <param name="anArticle">Article that was commented on</param>
        /// <param name="aMember">Member that will receive the e-mail</param>
        /// <returns>true if the e-mail was sent successfully and false otherwise.</returns>
        public bool SendVoteNotificationEmail(MailAddress email, ProviderArticle anArticle)
        {
            string articleUrl = HttpHost + "article/" + anArticle.Id;
            string voteResult = ".";
            if (anArticle.CountVotes > 0)
            {
                voteResult = " to a total of " + anArticle.CountVotes+".";
            }

            string emailBody = "DO NOT REPLY TO THIS E-MAIL<br />"
                            + "<br />"
                            + "<br />"
                            + "Your article, <a href='" + articleUrl + "'>" + anArticle.Title + "</a>, has been voted up"+voteResult
                            + "<br />"
                            + "<br />"
                            + "<br />";
                            //+ DidYouKnow();

            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        "InsideWord - Your article has been voted up! " + anArticle.Title,
                                                        emailBody);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);
            return true;
        }

        public bool SendHelpEmail(MailAddress email, ProviderMember aMember)
        {
            const string helpText = "DO NOT REPLY TO THIS E-MAIL<br />"
                                    + "<br />"
                                    + "<br />"
                                    + "<h1>Publish-by-E-mail Help, Tips & Tricks</h1>"
                                    + "<p>"
                                    + "When publishing by e-mail there are some special commands and tricks you should know about. "
                                    + "For starters, the subject line of the e-mail will be the Title of your article."
                                    + "The body of the e-mail is the text of your article and where you also write the special commands that will be discussed below."
                                    + "</p><p>"
                                    + "These special commands must be written first in the e-mail before the text of your article. "
                                    + "Some of these commands help only when editing an article by e-mail."
                                    + "</p><p>"
                                    + "To edit an article by e-mail you write the article's unique id number in the subject line instead of a title. "
                                    + "The unique article number id indicates to InsideWord which article you'd like to edit. "
                                    + "The unique number of an article is provided for you in the e-mail sent back to you when your article is accepted. "
                                    + "It can also be found in the web address when viewing the article."
                                    + "</p><p>"
                                    + "Below is the list of all valid commands when publishing by e-mail. "
                                    + "It is important that you write the square brackets [ and ]:"
                                    + "</p>"
                                    + "<ul>"
                                    + "    <li>"
                                    + "        [help] - This command will cause InsideWord to send you the help e-mail you're reading right now.<br />"
                                    + "        <br />Shorthand: [h]. Also for this command you can type [help], help, [h] or h in the subject of the e-mail.<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [category <i>category name</i>] - This command allows you to set the category for your article based on what you write for <i>category name</i>. "
                                    + "        The <i>category name</i> can be the name of the category or its unique id number. "
                                    + "        If you don't include a category when you initially publish an article by e-mail then the article will be saved as Draft (i.e. not visible to other people) until you set a category.<br />"
                                    + "        <br />Shorthand: [c <i>category name</i>]<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [blurb <i>your blurb</i>] - This command allows you to set the blurb of your article based on what you write for <i>your blurb</i>. "
                                    + "        By default if you don't include the blurb command then the blurb will be auto-generated after you publish the article from the first few sentences of your article.<br />"
                                    + "        <br />Shorthand: [b <i>your blurb</i>]<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [status draft] - This command allows you to save the article as a Draft. "
                                    + "        By default an article that is submitted by e-mail is automatically published on InsideWord (i.e. everyone can see it). "
                                    + "        So use this command if you want to edit the article a little more before publishing it at a late time. "
                                    + "        You can also use this command to change an article from Publish back to Draft.<br />"
                                    + "        <br />Shorthand: [draft] or [s d] or simply [d]<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [status publish] - This command allows you to save the article as Publish. "
                                    + "        By default an article that is submitted by e-mail is automatically published on InsideWord (i.e. everyone can see it). "
                                    + "        So typically this command is used to change an existing article you've written as Draft to Publish.<br />"
                                    + "        <br />Shorthand: [publish] or [s p] or simply [p]<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [append] - This command allows you to add additional text to the end of an existing article you've previously submitted. "
                                    + "        By default, when editing an article you will overwrite (i.e. completely erase and replace) your previous text. "
                                    + "        So typically this command is used when you want to incrementally add to the end of an existing article over time.<br / >"
                                    + "        <br />Shorthand: [a]<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [delete] - This command allows you to delete an existing article you've previously submitted. "
                                    + "        Use this command with caution as it will completely remove the article and everything related to it such as conversations and comments.<br />"
                                    + "        <br />Shorthand: To avoid accidental deletes, there is no shorthand for this command<br /><br />"
                                    + "    </li>"
                                    + "    <li>"
                                    + "        [read <i>unique article number</i>] - This command will cause InsideWord to e-mail you a specific article. "
                                    + "        If you omit an article id number then it will e-mail you the article you're currently submiting. "
                                    + "        Use this command and the [status draft] command to help you preview your article before Publishing it. <br />"
                                    + "        <br />Shorthand: [r <i>unique article number</i>]. You can also send a blank e-mail with just the article id number in the subject<br /><br />"
                                    + "    </li>"
                                    + "</ul>"
                                    + "<br />"
                                    + "<h2>Examples</h2>"
                                    + "<p>"
                                    + "    <u>Note:</u> The terms <b>Subject</b> and <b>Body</b> are being used to indicate where the commands should be written in the e-mail. "
                                    + "     You should not actually write <b>Subject</b> or <b>Body</b>."
                                    + "</p>"
                                    + "<p>"
                                    + "    Here is an example e-mail that publishes a new article with the Title \"Hello world!\" and with the text \"This is an article\". "
                                    + "    The category command was not used so it will have no category and will be saved by InsideWord as a Draft."
                                    + "</p>"
                                    + _tab + "<b>Subject:</b> Hello world!<br />"
                                    + _tab + "<b>Body:</b> This is an article!<br />"
                                    + "<br />"
                                    + "<br />"
                                    + "<p>"
                                    + "    Here is an example e-mail that adds to the previous article (let's pretend the previous article's id was 11). "
                                    + "    The article's new body after this will be \"This is an article with more text\". "
                                    + "    Note the article still has no category."
                                    + "</p>"
                                    + _tab + "<b>Subject:</b> 11<br />"
                                    + _tab + "<b>Body:</b> [append]<br />"
                                    + _tab + "with more text<br />"
                                    + "<br />"
                                    + "<br />"
                                    + "<p>This example e-mail will finally add a category to the previous article and will publish it.</p>"
                                    + _tab + "<b>Subject:</b> 11<br />"
                                    + _tab + "<b>Body:</b> [category politics]<br />"
                                    + _tab + "[status publish]<br />"
                                    + "<br />"
                                    + "<br />"
                                    + "<p>This example e-mail will return to us the article with id 23.</p>"
                                    + _tab + "<b>Subject:</b> 23<br />"
                                    + _tab + "<b>Body:</b>"
                                    + "<br />"
                                    + "<p>Below is the article returned by this e-mail.</p>"
                                    + _tab + "<b>Subject:</b> 23<br />"
                                    + _tab + "<b>Body:</b> [title example article]<br />"
                                    + _tab + "[category fun]<br />"
                                    + _tab + "[status draft]<br />"
                                    + _tab + "I have yet to publish this article... will do it at some later date.<br />"
                                    + "<p>"
                                    + "    The article as you can see is already formated for editing purposes and can be resent. "
                                    + "    It should be mentioned however that only articles that are yours or have been published can be retrieved this way. "
                                    + "    You also cannot edit this article unless you are the owner."
                                    + "</p>"
                                    + "<br />"
                                    + "<p>To help avoid some confusion, do note that an article that is published on our main site and an article that is published by e-mail are the same thing. You can edit an article that was published by e-mail using our website AND you can edit an article published on our site by using e-mail.</p>";

            
            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        "InsideWord - help",
                                                        helpText);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);
            return true;
        }

        /// <summary>
        /// Function to send an Edit/Delete e-mail to a member.
        /// </summary>
        /// <param name="anArticle">Article that member would like to edit/delete</param>
        /// <param name="aMember">Member that will receive the e-mail</param>
        /// <returns>true if the e-mail was sent successfully and false otherwise.</returns>
        public bool SendArticleEmail(MailAddress email, ProviderArticle anArticle, ProviderMember aMember)
        {
            string emailText = "[title " + anArticle.Title + "]<br />";

            if (anArticle.IsPublished)
            {
                emailText += "[status publish]<br />";
            }
            else
            {
                emailText += "[status draft]<br />";
            }

            if (anArticle.CategoryIds.Count > 0)
            {
                emailText += "[category "+(new ProviderCategory(anArticle.CategoryIds[0])).Title+"]<br />";
            }

            if (!anArticle.BlurbIsAutoGenerated)
            {
                emailText += "[blurb " + anArticle.Blurb+"]<br />";
            }

            emailText += "<br />" + anArticle.ParsedText;

            MailMessage aMailMessage = new MailMessage("donotreply@insideword.com",
                                                        email.Address,
                                                        anArticle.Id.ToString(),
                                                        emailText);
            aMailMessage.IsBodyHtml = true;
            DefaultSmtp.Send(aMailMessage);

            return true;
        }

        public string DidYouKnow()
        {
            string random = "<u><b>Did you know?</b></u>";
            switch (_roll.Next(17))
            {
                case 0:
                case 1:
                    random += "<p>You can publish by sending us just an e-mail at publish@insideword.com. For more details click <a href='" + HttpHost + "/info/publishbyemailfaq' >here</a></p>";
                    break;
                case 2:
                    random += "<p>When publishing by e-mail, you can edit an existing article by putting the article's id in the subject line.</p>";
                    break;
                case 3:
                    random += "<p>When publishing by e-mail, you can write special commands in your first line? For more details click <a href='" + HttpHost + "/info/publishbyemailfaq' >here</a></p>";
                    break;
                case 4:
                    random += "<p>When publishing by e-mail, you can edit an existing article by putting the article's id in the subject line.</p>";
                    break;
                case 5:
                    random += "<p>When publishing by e-mail, you can specify which category you want your article to belong to by writing [category <i>some category</i>] or [c <i>some category</i>] on the first line of the e-mail.</p>";
                    break;
                case 6:
                    random += "<p>When publishing by e-mail, you can send it in as a draft if you write [draft] or [d] or [status draft] or [s d] on the first line of the e-mail.</p>";
                    break;
                case 7:
                    random += "<p>When publishing by e-mail, you can set the status to publish by writing [status publish] or [s p] on the first line of the e-mail.</p>";
                    break;
                case 8:
                    random += "<p>When editing an article by e-mail you can add to the end of it instead of replacing it by writing [append] or [a] on the first line of the e-mail.</p>";
                    break;
                case 9:
                    random += "<p>It went wherever I did go...</p>";
                    break;
                case 10:
                    random += "<p>When publishing by e-mail, you can have more than one special command? Just write one after the other.</p>";
                    break;
                case 11:
                    random += "<p>When publishing by e-mail, you can completely delete an article by writing [delete] ? Just make sure you mean it!</p>";
                    break;
                case 12:
                    random += "<p>When publishing by e-mail, if you attach a picture it will be added to the article.</p>";
                    break;
                case 13:
                    random += "<p>Your articles are ranked by a sophisticated network of sparrows. We tried using pigeons, but Google bought them all out.</p>";
                    break;
                case 14:
                    random += "<p>You can use your gmail, hotmail, yahoo and other accounts to automatically login?</p>";
                    break;
                case 15:
                    random += "<p>marshmallow?</p>";
                    break;
                case 16:
                    random += "<p>An article's rank will drop over time and will ultimately reach zero.</p>";
                    break;
            }

            return random;
        }

        //=========================================================
        // PROTECTED FUNCTIONS
        //=========================================================
        protected EmailManager()
        {
            DefaultSmtp = new SmtpClient();
            _roll = new Random((int)DateTime.UtcNow.Ticks);
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static protected EmailManager _instance;

        static public EmailManager Instance { get { return _instance; } }

        static EmailManager()
        {
            // configured based on the smtp values of the config file
            _instance = new EmailManager();
        }
    }
}