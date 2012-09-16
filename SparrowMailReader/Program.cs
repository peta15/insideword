using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Web;
using Lesnikowski.Client.IMAP;
using System.Configuration;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using InsideWordProvider;
using Lesnikowski.Mail.Headers;
using InsideWordAdvancedResource;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SparrowMailReader
{
    class Program
    {
        static protected bool _continue;
        static protected ILog _log;
        static protected SmtpNetworkElement _mailValues;
        static protected Imap _emailClient;
        static protected ObjectContextManager _ctxManager;
        static protected TimeSpan _idleTime;

        static int Main(string[] args)
        {
            int returnValue = 0;
            DateTime? startTime = null;
            DateTime? crashTime = null;

            do
            {
                try
                {
                    startTime = DateTime.UtcNow;

                    Initialize();
                    _log.Warn("Entering application.");
                    MainLoop();
                    _log.Warn("Exiting application.");
                }
                catch (Exception caughtException)
                {
                    if (_log == null)
                    {
                        _continue = false;
                        returnValue = 1;
                    }
                    else
                    {
                        _log.Error("Program threw exception", caughtException);
                        if (_continue)
                        {
                            if (!startTime.HasValue)
                            {
                                startTime = DateTime.UtcNow;
                            }
                            crashTime = DateTime.UtcNow;
                            TimeSpan runtimeBeforeCrash = crashTime.Value.Subtract(startTime.Value);
                            double runtimeSecs = runtimeBeforeCrash.TotalSeconds;
                            if (runtimeSecs > 5.0)
                            {
                                _continue = true;
                                returnValue = 0;
                                _log.Error("Program crashed after " + runtimeBeforeCrash.TotalHours + " hours. Restarting Program.");
                            }
                            else
                            {
                                _continue = false;
                                returnValue = 1;
                                _log.Error("Program crashed after " + runtimeSecs + " seconds. Will not restart Program to avoid infinite crash loop.");
                            }
                        }
                    }
                }
            } while (_continue);

            return returnValue;
        }

        static public void Initialize()
        {
            _continue = true;

            _log = LogManager.GetLogger(typeof(Program));
            XmlConfigurator.Configure();
            _log.Info("Logging initialized");

            //Set our thread priority to low
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            _log.Info("Thread priority adjusted");

            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            _log.Info("Exit handler initialized");

            Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SmtpSection imap = configFile.GetSection("imap") as SmtpSection;
            _mailValues = imap.Network;
            _log.Info("e-mail settings inialized");
            
            EmailManager.Instance.HttpHost = ConfigurationManager.AppSettings["TargetHttpHost"].ToString();
            _log.Info("e-mail manager inialized");

            _emailClient = new Imap();

            _ctxManager = new ObjectContextManager();
            Provider.DbCtx = _ctxManager;
            _log.Info("InsideWord Database object context manager initialized");


            _idleTime = new TimeSpan(0, 20, 0);
            _log.Info("Initializing idle time to "+_idleTime.TotalMinutes+" minutes.");
        }

        static public int MainLoop()
        {
            while (_continue)
            {
                if (!_emailClient.Connected)
                {
                    Login();
                }

                if (_emailClient.Connected)
                {
                    ParseNewMail();
                }
                _log.Info("Idling...");
                _emailClient.Idle(_idleTime);
            }
            _emailClient.Close();

            return 0;
        }

        /// <summary>
        /// Connect the pop client.
        /// </summary>
        /// <param name="accountInfo">The information account</param>
        static public void Login()
        {
            if (_mailValues.EnableSsl)
            {
                _log.Info("Attempting to ssl connect to " + _mailValues.Host + ":" + _mailValues.Port);
                _emailClient.ConnectSSL(_mailValues.Host, _mailValues.Port);
                _log.Info("Successful ssl connection to " + _mailValues.Host + ":" + _mailValues.Port);
            }
            else
            {
                _log.Info("Attempting to connect to " + _mailValues.Host + ":" + _mailValues.Port);
                _emailClient.Connect(_mailValues.Host, _mailValues.Port);
                _log.Info("Successful connection to " + _mailValues.Host + ":" + _mailValues.Port);
            }

            _log.Info("Attempting to login to " + _mailValues.Host + ":" + _mailValues.Port +" with "+_mailValues.UserName);
            _emailClient.Login(_mailValues.UserName, _mailValues.Password);
            _log.Info("Successful login to " + _mailValues.Host + ":" + _mailValues.Port + " with " + _mailValues.UserName);

            _log.Info("Selecting inbox");
            _emailClient.SelectInbox();
            _log.Info("Inbox selection successful");
        }

        static public void ParseNewMail()
        {
            // Retrieve newest e-mails
            List<long> uids = _emailClient.SearchFlag(Flag.Unseen);
            if (uids.Count > 0)
            {
                _log.Info("Processing " + uids.Count + " unseen message(s).");

                ProviderMember aMember = new ProviderMember();

                foreach (MessageInfo info in _emailClient.GetMessageInfoByUID(uids))
                {
                    _log.Info("Processing: " + info.Envelope.Sender + " / " + info.Envelope.From + " / " + info.Envelope.Subject);

                    if (ValidEmail(info))
                    {
                        // E-mail header seems good so far.
                        // Fetch the owning member.
                        aMember = GetEmailOwner(info);

                        _log.Info("Checking account settings.");
                        if (aMember.IsBanned)
                        {
                            _log.Info("Member is banned. Ignoring e-mail.");
                            MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                                        info.Envelope.From.ToString(),
                                                                        "insideword - article " + info.Envelope.Subject + " rejected",
                                                                        "DO NOT REPLY TO THIS E-MAIL<br />"
                                                                      + "The article you sent to us through e-mail was rejected because your account was banned.");
                            aMailMessage.IsBodyHtml = true;
                            EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
                        }
                        else
                        {
                            _log.Info("Member is good.");

                            long articleId = -1;
                            ProviderArticle anArticle = null;

                            // Try parsing the subject. If we successfully parse it as a long then this is an update to that article
                            if (long.TryParse(info.Envelope.Subject, out articleId))
                            {
                                _log.Info("Subject was number " + articleId + ", so attempt to load the article for editing purposes.");
                                if (ProviderArticle.Exists(articleId))
                                {
                                    // This article exists so load it
                                    anArticle = new ProviderArticle(articleId);
                                    _log.Info("Successfully loaded article " + articleId + " - " + anArticle.Title);
                                }
                                else
                                {
                                    _log.Info("Article with id " + articleId + " doesn't exist. Ignoring e-mail.");
                                    MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                                                info.Envelope.From.ToString(),
                                                                                "insideword - article rejected",
                                                                                "DO NOT REPLY TO THIS E-MAIL<br />"
                                                                                + "The article you sent to us through e-mail was rejected because you were trying to edit article "
                                                                                + articleId + ", which doesn't exist.");
                                    aMailMessage.IsBodyHtml = true;
                                    EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
                                }
                            }
                            else
                            {
                                _log.Info("This is a new article.");
                                anArticle = new ProviderArticle();
                            }

                            if (anArticle != null)
                            {
                                _log.Info("Check member's edit rights for the given article.");
                                if (!aMember.CanEdit(anArticle))
                                {
                                    _log.Info("Member does not have the rights to edit this article. Ignoring e-mail.");
                                    MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                                                info.Envelope.From.ToString(),
                                                                                "insideword - article rejected",
                                                                                "DO NOT REPLY TO THIS E-MAIL<br />"
                                                                              + "The article you sent to us through e-mail was rejected because you do not have the rights to edit it!");
                                    aMailMessage.IsBodyHtml = true;
                                    EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
                                }
                                else if (EditArticle(ref anArticle, aMember, info))
                                {
                                    MailAddress address = new MailAddress(info.Envelope.From.ToString());
                                    EmailManager.Instance.SendEditArticleEmail(address, anArticle, aMember);
                                }
                            }
                        }
                    }

                    _emailClient.MarkMessageSeen(info.MessageNumber);
                    _log.Info("Marking message as seen");
                    _log.Info("Done processing: " + info.Envelope.Sender + " / " + info.Envelope.From + " / " + info.Envelope.Subject);
                }

                _ctxManager.DisposeCtx();
            }
        }

        static public bool ValidEmail(MessageInfo info)
        {
            bool returnValue = false;
            IMailBoxList senderList = info.Envelope.Sender;
            if (senderList.Count == 0)
            {
                _log.Info("No sender information found.");
            }
            else if(senderList.Count > 1)
            {
                _log.Info("Too many senders provided ");
            }
            else if (string.IsNullOrWhiteSpace(info.Envelope.Subject))
            {
                _log.Info("Subject was empty");
                MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                            senderList[0].Address,
                                                            "insideword - article rejected",
                                                            "DO NOT REPLY TO THIS E-MAIL<br />"
                                                            + "The article you sent to us through e-mail was rejected because the subject of the e-mail you sent was blank."
                                                            + "The subject is used for the article title and as such cannot be blank.");
                aMailMessage.IsBodyHtml = true;
                EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
            }
            else if (info.BodyStructure.Text.Size == 0 && info.BodyStructure.Html.Size == 0)
            {
                _log.Info("E-mail body was blank.");
                MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                            senderList[0].Address,
                                                            "insideword - article "+info.Envelope.Subject+" rejected",
                                                            "DO NOT REPLY TO THIS E-MAIL<br />"
                                                            + "The article you sent to us through e-mail was rejected because your e-mail sheet was blank!");
                aMailMessage.IsBodyHtml = true;
                EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
            }
            else
            {
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Function that fetches the owner of the article
        /// </summary>
        /// <param name="info">Info regarding a particular e-mail article</param>
        /// <returns>The provider member who is the owner of this e-mail article</returns>
        static public ProviderMember GetEmailOwner(MessageInfo info)
        {
            ProviderMember aMember;
            string email = info.Envelope.Sender[0].Address;
            MailAddress address = new MailAddress(email);
            long? memberId = ProviderEmail.FindOwner(address, null);
            if (memberId.HasValue)
            {
                _log.Info("E-mail sender has an account.");
                aMember = new ProviderMember(memberId.Value);
            }
            else
            {
                _log.Info("Person does not have an account. Creating one.");
                aMember = new ProviderMember();
                aMember.CreateDate = DateTime.UtcNow;
                aMember.EditDate = DateTime.UtcNow;
                aMember.Save();

                ProviderEmail memberEmail = new ProviderEmail();
                memberEmail.CreateDate = DateTime.UtcNow;
                memberEmail.EditDate = DateTime.UtcNow;
                memberEmail.Email = address;
                memberEmail.IsValidated = true;
                memberEmail.MemberId = aMember.Id.Value;
                memberEmail.Save();
            }
            return aMember;
        }

        static public bool EditArticle(ref ProviderArticle anArticle, ProviderMember aMember, MessageInfo info)
        {
            _log.Info("Populating article from e-mail");
            bool returnValue = false;
            MailAddress senderAddress = new MailAddress(info.Envelope.Sender[0].Address);

            // Now parse the data from the e-mail
            EmailArticle anEmailArticle = new EmailArticle(info, _emailClient);

            if(anEmailArticle.IsHelp && !anEmailArticle.IsSubjectHelp)
            {
                // If we got the help command in the body but not in the subject then send the e-mail.
                // it's important that we check the subject help because otherwise we will send the help e-mail twice.
                _log.Info("Found the [help] command in the e-mail so respond with the help e-mail.");
                EmailManager.Instance.SendHelpEmail(senderAddress, aMember);
            }

            if (anEmailArticle.IsSubjectHelp)
            {
                // If we got help in the subject then ONLY send the help e-mail. Since the subject can't be an article title or id.
                _log.Info("This was a subject help request so send the help e-mail but do nothing else.");
                EmailManager.Instance.SendHelpEmail(senderAddress, aMember);
            }
            else
            {
                if(anEmailArticle.IsBlankEmail)
                {
                    _log.Info("E-mail was completely blank");
                }
                else if (anEmailArticle.Delete)
                {
                    if (anArticle.IsNew)
                    {
                        _log.Info("Attempted to delete a new article, which makes no sense so ignore this e-mail.");
                        MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                                    senderAddress.Address,
                                                                    "insideword - article " + info.Envelope.Subject + " rejected",
                                                                    "DO NOT REPLY TO THIS E-MAIL<br />"
                                                                    + "The delete command you sent us in an e-mail was ignored since you tried to delete an article that doesn't exist. Be sure that you used the correct article id number in the subject.");
                        aMailMessage.IsBodyHtml = true;
                        EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
                    }
                    else
                    {
                        _log.Info("Deleting article.");
                        string title = anArticle.Title;
                        long id = anArticle.Id.Value;
                        string result;
                        if (anArticle.Delete())
                        {
                            result = "Successfully deleted article " + title + " with id " + id;
                        }
                        else
                        {
                            result = "Failed to delete article " + title + " with id " + id;
                        }
                        _log.Info(result);
                        MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                                    senderAddress.Address,
                                                                    "insideword - article " + info.Envelope.Subject + " rejected",
                                                                    "DO NOT REPLY TO THIS E-MAIL<br />"
                                                                    + result + "<br />");
                        aMailMessage.IsBodyHtml = true;
                        EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
                        returnValue = false;
                    }
                }
                else
                {
                    if (anArticle.IsNew)
                    {
                        anArticle.CreateDate = DateTime.UtcNow;

                        // only update the member id when we first create the article otherwise an admin could end up taking control when editing it.
                        anArticle.MemberId = aMember.Id.Value;
                    }
                    anArticle.EditDate = DateTime.UtcNow;

                    if (anArticle.IsNew || !string.IsNullOrWhiteSpace(anEmailArticle.Title))
                    {
                        anArticle.Title = anEmailArticle.Title;
                    }

                    if (anEmailArticle.CategoryId.HasValue)
                    {
                        anArticle.AddCategory(anEmailArticle.CategoryId.Value);
                    }

                    if (anEmailArticle.Blurb != null)
                    {
                        if (string.IsNullOrWhiteSpace(anEmailArticle.Blurb))
                        {
                            anArticle.Blurb = null;
                        }
                        else
                        {
                            anArticle.Blurb = anEmailArticle.Blurb;
                        }
                    }

                    // don't overwrite unless we have some content.
                    if (!string.IsNullOrWhiteSpace(anEmailArticle.Text))
                    {
                        if (anArticle.IsNew || !anEmailArticle.Append)
                        {
                            anArticle.RawText = anEmailArticle.Text;
                        }
                        else
                        {
                            anArticle.RawText += anEmailArticle.Text;
                        }
                    }

                    if (anEmailArticle.IsPublished.HasValue)
                    {
                        // if the e-mail specifically set the publish status then set it in the article
                        anArticle.IsPublished = anEmailArticle.IsPublished.Value && anEmailArticle.CategoryId.HasValue;
                    }
                    else if (anArticle.IsNew)
                    {
                        // if the article is new but no status was set in the e-mail then set the status based on the following
                        anArticle.IsPublished = anEmailArticle.CategoryId.HasValue;
                    }
                    else
                    {
                        // don't change the status of the article
                    }
                    anArticle.Save();
                    returnValue = true;
                }

                if (anEmailArticle.IsReadArticle)
                {
                    if (anEmailArticle.ReadArticle.HasValue)
                    {
                        _log.Info("Request to send back article " + anEmailArticle.ReadArticle.Value);
                        try
                        {
                            ProviderArticle aDifferentArticle = new ProviderArticle(anEmailArticle.ReadArticle.Value);
                            if (aDifferentArticle.IsPublished || aMember.CanEdit(aDifferentArticle))
                            {
                                EmailManager.Instance.SendArticleEmail(senderAddress, anArticle, aMember);
                            }
                            else
                            {
                                throw new Exception("Member doesn't have the rights to request article " + anEmailArticle.ReadArticle.Value);
                            }
                        }
                        catch (Exception caughtException)
                        {
                            _log.Info("Failed to send back article " + anEmailArticle.ReadArticle.Value, caughtException);
                            MailMessage aMailMessage = new MailMessage("donotreply@insideword",
                                                                        senderAddress.Address,
                                                                        "insideword - article request " + anEmailArticle.ReadArticle.Value + " failed",
                                                                        "DO NOT REPLY TO THIS E-MAIL<br />"
                                                                        + "Failed to return the article with id " + anEmailArticle.ReadArticle.Value + "<br />"
                                                                        + "Check to make sure the article id is a valid id.<br />");
                            aMailMessage.IsBodyHtml = true;
                            EmailManager.Instance.DefaultSmtp.Send(aMailMessage);
                        }
                    }
                    else
                    {
                        _log.Info("Sending back current article");
                        EmailManager.Instance.SendArticleEmail(senderAddress, anArticle, aMember);
                    }
                }
            }

            return returnValue;
        }

        static public bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    _log.Info("CTRL+C");
                    _continue = false;
                    break;
                case CtrlTypes.CTRL_BREAK_EVENT:
                    _log.Info("BREAK");
                    _continue = false;
                    break;
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    _log.Info("CLOSE");
                    _continue = false;
                    break;
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                    _log.Info("LOGOFF");
                    _continue = false;
                    break;
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    _log.Info("SHUTDOWN");
                    _continue = false;
                    break;
            }
            return true;
        }

        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endregion
    }
}
