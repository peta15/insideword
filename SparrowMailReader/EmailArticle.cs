using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lesnikowski.Client.IMAP;
using System.Text.RegularExpressions;
using InsideWordProvider;
using InsideWordResource;

namespace SparrowMailReader
{
    class EmailArticle
    {
        // regex definition of what constitues as an endline. Note that html endlines are included
        protected const string _sRegexSpaces = @"(?: )*";
        protected const string _sRegexEndline = @"(<br" + _sRegexSpaces + "(?:/)?>)|(\r\n)|(\n)";
        protected const string _sRegexValidChars = @"[^\[\]\r\n\f\t\v<>]";
        protected const string _sRegexValidTagInternal = @"(?:\s[^\<\>/]*)?";
        
        // Similar to WordPress these are the regex values used to find commands in the e-mail body.
        // E-mail commands are surrounded by square brackets "[" and "]" and can only exist in the
        // first lines of an e-mail. Currently there are six commands:
        // [help]                     : indicates that the help e-mail should be sent
        // [category <category name>] : indicates the category the article will use
        // [draft]                    : indicates that the article should be submitted as a draft
        // [publish]                  : indicates that the article should be published
        // [title <a title>]          : indicates what the title of the article should be. This always overwrites what is in the subject.
        // [append]                   : indicates that the original text of an article should be appended to rather than overwritten
        // [delete]                   : indicates that the article should be deleted completely.
        // [read <article id>]        : indicates that we should send back an article in e-mail form.
        // [blurb <a blurb>]          : indicates the blurb that the article will use.

        protected const string _sRegexSubjectHelp = @"^"+ _sRegexSpaces + "(?:(?:help)|h)" + _sRegexSpaces + @"$";
        protected const string _sRegexHelp = @"\[" + _sRegexSpaces + "(?:(?:help)|h)"+_sRegexSpaces+@"\]";
        protected const string _sRegexCategory = @"\["+_sRegexSpaces+"(?:(?:category)|c) ("+_sRegexValidChars+@"+)\]";
        protected const string _sRegexDraft = @"\["+_sRegexSpaces+@"(?:(?:(?:status)|s) )?"+_sRegexSpaces+@"(?:(?:draft)|d)"+_sRegexSpaces+@"\]";
        protected const string _sRegexPublish = @"\["+_sRegexSpaces+@"(?:(?:(?:status)|s) )?"+_sRegexSpaces+@"(?:(?:publish)|p)"+_sRegexSpaces+@"\]";
        protected const string _sRegexTitle = @"\["+_sRegexSpaces+@"(?:(?:title)|t) ("+_sRegexValidChars+@"+)\]";
        protected const string _sRegexAppend = @"\["+_sRegexSpaces+@"(?:(?:append)|a)"+_sRegexSpaces+@"\]";
        protected const string _sRegexDelete = @"\["+_sRegexSpaces+@"delete"+_sRegexSpaces+@"\]";
        protected const string _sRegexRead = @"\[" + _sRegexSpaces + @"(?:(?:read)|r)(?: ([1-9][0-9]*))?" + _sRegexSpaces + @"\]";
        protected const string _sRegexBlurb = @"\[" + _sRegexSpaces + @"(?:(?:blurb)|b)(:? (" + _sRegexValidChars + @"+))?\]";
        protected const string _sRegexValidCommand = @"\["+_sRegexValidChars+@"+\]";
        protected const string _sRegexCommandList = @"^(?:(?:" + _sRegexEndline + @"|\s)*" + _sRegexValidCommand + @"(?:" + _sRegexEndline + @"|\s)*)*";
        protected const string _sRegexHtmlBody = @"<body"+_sRegexValidTagInternal+@">((?:.|\s)*)</body\s*>";
        protected const string _sRegexEmailRe = @"^Re:";

        static protected readonly Regex _regexSubjectHelp = new Regex(_sRegexSubjectHelp, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexHelp = new Regex(_sRegexHelp, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexEndline = new Regex(_sRegexEndline, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexCategory = new Regex(_sRegexCategory, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexDraft = new Regex(_sRegexDraft, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexPublish = new Regex(_sRegexPublish, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexTitle = new Regex(_sRegexTitle, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexAppend = new Regex(_sRegexAppend, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexDelete = new Regex(_sRegexDelete, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexRead = new Regex(_sRegexRead, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexBlurb = new Regex(_sRegexBlurb, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexCommandList = new Regex(_sRegexCommandList, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexBlankDoc = new Regex(@"^((?:" + _sRegexEndline + @"|\s)+)$", RegexOptions.IgnoreCase);
        static protected readonly Regex _regexHtmlBody = new Regex(_sRegexHtmlBody, RegexOptions.IgnoreCase);
        static protected readonly Regex _regexEmailRe = new Regex(_sRegexEmailRe, RegexOptions.IgnoreCase);

        public EmailArticle(MessageInfo info, Imap emailClient)
        {
            ParseEmail(info, emailClient);
        }

        public bool IsHelp { get; set; }
        public bool IsSubjectHelp { get; set; }
        public string Title { get; set; }
        public bool IsSubjectId { get; set; }
        public string Text { get; set; }
        public string CategoryTitle { get; set; }
        public string Blurb { get; set; }
        public long? CategoryId { get; set; }
        public bool? IsPublished { get; set; }
        public bool Append { get; set; }
        public bool Delete { get; set; }
        public bool IsReadArticle { get; set; }
        public long? ReadArticle { get; set; }

        // This bool indicates that the entire e-mail's body was blank.
        // Which means no commands or any text. Just endlines, tabs and blank spaces.
        // Note Text being blank doesn't imply the e-mail was blank since Text doesn't contain any commands.
        public bool IsBlankEmail { get; set; }

        public void ParseEmail(MessageInfo info, Imap emailClient)
        {
            long id;
            string subject = info.Envelope.Subject;

            // Strip any 'Re:' from the subjects
            if (_regexEmailRe.IsMatch(subject))
            {
                subject = subject.Substring(3);
            }

            subject = subject.Trim();

            if (long.TryParse(subject, out id))
            {
                // the subject was a number which means we're editing the article.
                // this also means that the title doesn't change unless someone
                // uses the title command to change it, so for now set title to null.
                Title = null;
                IsSubjectId = true;
            }
            else
            {
                IsSubjectHelp = _regexSubjectHelp.IsMatch(subject);
                Title = subject;
            }

            string emailBody = emailClient.GetMimePartText(info.MessageNumber, info.BodyStructure.Text);
            string htmlEmailBody = emailClient.GetMimePartText(info.MessageNumber, info.BodyStructure.Html);

            if (!string.IsNullOrWhiteSpace(htmlEmailBody))
            {
                Text = ParseBody(htmlEmailBody);
            }
            else if (!string.IsNullOrWhiteSpace(emailBody))
            {
                Text = ParseBody(emailBody);
            }
        }

        protected string ParseBody(string emailBody)
        {
            Match htmlBodyMatch = _regexHtmlBody.Match(emailBody);
            string body = "";
            if (htmlBodyMatch.Success)
            {
                body = htmlBodyMatch.Groups[1].Value;
            }
            else
            {
                body = emailBody;
            }

            if (_regexBlankDoc.IsMatch(body))
            {
                IsBlankEmail = true;
                // the body is empty so set the special read command in this situation
                IsReadArticle = IsSubjectId;
            }
            else
            {
                body = ParseCommands(body);
                if (_regexBlankDoc.IsMatch(body))
                {
                    body = "";
                }
            }
            return body;
        }

        protected string ParseCommands(string emailBody)
        {
            Match commandsMatch = _regexCommandList.Match(emailBody);
            if (commandsMatch.Success)
            {
                string commands = commandsMatch.Groups[0].Value;
                emailBody = emailBody.Substring(commands.Length);

                Match categoryMatch = _regexCategory.Match(commands);
                Match draftMatch = _regexDraft.Match(commands);
                Match titleMatch = _regexTitle.Match(commands);
                Match publishMatch = _regexPublish.Match(commands);
                Match readMatch = _regexRead.Match(commands);
                Match blurbMatch = _regexBlurb.Match(commands);

                if (categoryMatch.Success)
                {
                    CategoryTitle = categoryMatch.Groups[1].Value.Trim();
                    long id;
                    if (long.TryParse(CategoryTitle, out id))
                    {
                        CategoryId = id;
                    }
                    else
                    {
                        CategoryId = ProviderCategory.ExistsTitle(CategoryTitle);
                    }
                }

                if (titleMatch.Success)
                {
                    Title = titleMatch.Groups[1].Value.Trim();
                }

                // if either draft or publish are set then we should set IsPublished
                if (draftMatch.Success || publishMatch.Success)
                {
                    // truth table to show how we determine the IsPublished logic
                    // draft publish IsPublished
                    // F     F       = null
                    // T     F       = F
                    // T     T       = F
                    // F     T       = T
                    IsPublished = !draftMatch.Success;
                }

                if (readMatch.Success)
                {
                    IsReadArticle = true;
                    long articleId;
                    string strId = readMatch.Groups[2].Value;
                    if (long.TryParse(strId, out articleId))
                    {
                        ReadArticle = articleId;
                    }
                }

                if (blurbMatch.Success)
                {
                    Blurb = blurbMatch.Groups[1].Value.Trim();
                }

                IsHelp = _regexHelp.IsMatch(commands);
                Append = _regexAppend.IsMatch(commands);
                Delete = _regexDelete.IsMatch(commands);
            }
            return emailBody;
        }
    }
}
