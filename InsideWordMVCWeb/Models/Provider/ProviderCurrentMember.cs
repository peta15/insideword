using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Collections.Generic;
using InsideWordProvider;
using InsideWordResource;
using System.Net.Mail;
using System.Web.SessionState;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Models.WebProvider
{
    /// <summary>
    /// Summary description for ProviderCurrentMember
    /// </summary>
    public class ProviderCurrentMember : ProviderMember
    {
        protected bool _isLoggedOn;
        protected const string _currentUserKey = "_currentUserKey";
        protected const string _currentWorkSpaceKey = "_currentWorkSpaceKey";

        public bool IsLoggedOn
        {
            get { return _isLoggedOn; }
        }

        public List<string> HostAddressList { get; set; }

        public override bool Delete()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session[_currentUserKey] = null;
            return base.Delete();
        }

        public void LogOff()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session[_currentUserKey] = null;
            HttpContext.Current.Items[_currentUserKey] = null;
            Clear();
        }

        /// <summary>
        /// Indicates if a given member can modify the given object.
        /// This is an overrided version of the ProviderMember parent's version of the function.
        /// This function takes into account whether the current member is logged in or not.
        /// </summary>
        /// <param name="editable">object we wish to check the rights for with regards to this member.</param>
        /// <returns>true if the member can edit it and false otherwise.</returns>
        public override bool CanEdit(Object editable)
        {
            bool returnValue = IsSuperAdmin;

            if (!returnValue)
            {
                if (editable is ProviderMember)
                {
                    //can edit this member as long as he isn't equal to or lower in admin rank
                    ProviderMember editMember = (ProviderMember)editable;
                    returnValue = IsActive && IsLoggedOn && !IsBanned &&
                                ((IsMasterAdmin && !(editMember.IsSuperAdmin || editMember.IsMasterAdmin)) ||
                                 (IsMemberAdmin && !editMember.HasAdminRights) ||
                                 (Owns(editMember) && editMember.IsArticleAdmin == IsArticleAdmin &&
                                                      editMember.IsCategoryAdmin == IsCategoryAdmin &&
                                                      editMember.IsMasterAdmin == IsMasterAdmin &&
                                                      editMember.IsMemberAdmin == IsMemberAdmin &&
                                                      editMember.IsSuperAdmin == IsSuperAdmin));
                }
                else if (editable is ProviderArticle)
                {
                    // Can edit this article as long as they are the owner or have the admin rights
                    // An account does not need to be active to edit an article.
                    ProviderArticle editArticle = (ProviderArticle)editable;
                    returnValue = editArticle.IsNew ||
                           (!IsBanned && Owns(editArticle)) ||
                           (IsMasterAdmin || IsArticleAdmin);
                }
                else if (editable is ProviderComment)
                {
                    //can edit this comment as long as they are the owner or have the admin rights
                    ProviderComment editComment = (ProviderComment)editable;
                    returnValue = IsLoggedOn && !IsBanned &&
                                 (editComment.IsNew || Owns(editComment) || IsMasterAdmin || IsMemberAdmin);
                }
                else if (editable is ProviderCategory)
                {
                    //can edit this category as long as they have the admin rights
                    returnValue = IsActive && IsLoggedOn && !IsBanned &&
                                  (IsMasterAdmin || IsCategoryAdmin);
                }
            }

            return returnValue;
        }

        override public bool Owns(Object property)
        {
            bool returnValue = base.Owns(property);
            if (!returnValue)
            {
                if (property is ProviderArticle)
                {
                    ProviderArticle anArticle = (ProviderArticle)property;
                    if (!anArticle.IsNew)
                    {
                        returnValue = CurrentWorkSpace.ArticleIdList.Exists(anId => anId == anArticle.Id.Value);
                    }
                }
            }
            return returnValue;
        }

        public enum AccessGroup
        {
            // enum must start at 1 for the AuthorizationAttribute in Annotations.cs
            Administration = 1,
            MemberManagement,
            CategoryManagement,
            ArticleManagement,
            ConversationManagement
        }

        public bool CanAccess(AccessGroup group)
        {
            bool returnValue = false;

            // A banned or inactive user can't access anything
            if (IsActive && IsLoggedOn && !IsBanned)
            {
                // Super Admins and Master Admins can access everything
                // however the master admin is restricted to what he can edit.
                if (IsSuperAdmin || IsMasterAdmin)
                {
                    returnValue = true;
                }
                else
                {
                    switch (group)
                    {
                        case AccessGroup.Administration:
                            returnValue = HasAdminRights;
                            break;
                        case AccessGroup.MemberManagement:
                            returnValue = IsMemberAdmin;
                            break;
                        case AccessGroup.CategoryManagement:
                            returnValue = IsCategoryAdmin;
                            break;
                        case AccessGroup.ArticleManagement:
                            returnValue = IsArticleAdmin;
                            break;
                        case AccessGroup.ConversationManagement:
                            returnValue = IsMemberAdmin;
                            break;
                    }
                }
            }

            return returnValue;
        }

        public Workspace CurrentWorkSpace
        {
            get
            {
                HttpSessionState session = HttpContext.Current.Session;
                if (session[_currentWorkSpaceKey] == null)
                {
                    session[_currentWorkSpaceKey] = new Workspace();
                }
                return (Workspace)session[_currentWorkSpaceKey];
            }
        }

        /// <summary>
        /// Function to validate and login an active member
        /// </summary>
        /// <param name="alternateId">one of all possible alternateIds a member could have</param>
        /// <param name="password">password of the member we wish to login. Password can be null/empty if no password is expected with this alternateId.</param>
        /// <param name="rememberMe">If true then the system will give the member a cookie that will last a month. The system will use this cookie to auto-login the member.</param>
        /// <param name="errorList">if the login fails for whatever reason then all the errors will be returned in the errorList</param>
        /// <returns>true if the function succeeded in login the member in and false otherwise.</returns>
        public LoginEnum Login(string alternateId, string password, bool rememberMe, ref List<string> errorList)
        {
            LoginEnum returnValue = LoginEnum.unknown;
            _isLoggedOn = false;

            ProviderAlternateMemberId anAltId = new ProviderAlternateMemberId();
            if (!anAltId.Load(alternateId) ||
                 (anAltId.UsePassword && !Exists(anAltId.MemberId, password)))
            {
                returnValue = LoginEnum.unknown;
                errorList.Add("Invalid Id or Password.");
            }
            else if (anAltId.HasExpired)
            {
                returnValue = LoginEnum.expired;
                // The id has expired and was decommissioned so return an error
                errorList.Add("Your Id has expired.");
            }
            else
            {
                try
                {
                    Load(anAltId.MemberId);

                    if (!anAltId.IsValidated)
                    {
                        returnValue = LoginEnum.invalid;
                        errorList.Add("Id has not been validated.");
                    }
                    else if (IsBanned)
                    {
                        returnValue = LoginEnum.banned;
                        errorList.Add("Your account has been banned.");
                    }
                    else
                    {
                        returnValue = LoginEnum.success;
                        anAltId.ValidateData();
                        CurrentWorkSpace.Save(this);
                        CreateLoginCookie(rememberMe, ref errorList);
                        _isLoggedOn = true;
                    }

                    if (_isLoggedOn)
                    {
                        // Try and decommission the alternate id
                        if (!anAltId.TryDecommission())
                        {
                            // The alternate id was not decommissioned and is still valid so mark it as used
                            anAltId.Used();
                            anAltId.Save();
                        }
                    }
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error(caughtException);
                    errorList.Add("Invalid Id or Password.");
                }
            }

            return returnValue;
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderCurrentMember() : base() { }
        protected ProviderCurrentMember(long id) : base(id) { }

        override protected void ClassClear()
        {
            base.ClassClear();
            _isLoggedOn = false;
            HostAddressList = new List<string>();
        }

        protected ProviderCurrentMember(Member aMember) : base(aMember) { }

        /// <summary>
        /// Function used to create a member's login information in an authentication cookie
        /// </summary>
        /// <param name="rememberMe">boolean used to indicate if the cookie should be stored for longer</param>
        /// <returns>true if the cookie was created successfully and false otherwise.</returns>
        protected bool CreateLoginCookie(bool rememberMe, ref List<string> errorList)
        {
            LoginTicket aTicket = new LoginTicket();
            aTicket.Key = CurrentMonthIssuedKey.IssuedKey;
            aTicket.RemberMe = rememberMe;
            HttpContext.Current.Response.AppendCookie(aTicket.CreateCookieTicket());
            return true;
        }

        protected class LoginTicket
        {
            protected const char DELIMITER = '-';
            public LoginTicket()
            {

            }

            public bool RemberMe { get; set; }
            public string Key { get; set; }

            public FormsAuthenticationTicket CreateTicket()
            {
                DateTime expiration;
                if (RemberMe)
                {
                    expiration = DateTime.UtcNow.AddDays(7);
                }
                else
                {
                    expiration = DateTime.UtcNow.AddMinutes(30);
                }
                return new FormsAuthenticationTicket(1,
                                                     "InsideWord",
                                                     DateTime.UtcNow,
                                                     expiration, // value of time out property
                                                     true, // Value of IsPersistent property
                                                     Key + DELIMITER + RemberMe.ToString(),
                                                     FormsAuthentication.FormsCookiePath);
            }

            public HttpCookie CreateCookieTicket()
            {
                DateTime expiration;
                if (RemberMe)
                {
                    expiration = DateTime.UtcNow.AddDays(7);
                }
                else
                {
                    expiration = DateTime.UtcNow.AddMinutes(30);
                }
                string encryptedTicket = FormsAuthentication.Encrypt(CreateTicket());
                HttpCookie aCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                                    encryptedTicket);
                aCookie.Expires = expiration;

                return aCookie;
            }

            public bool ParseTicket(FormsAuthenticationTicket ticket)
            {
                string[] data = ticket.UserData.Split(DELIMITER);

                // first check the size of the data. We expect at least 2 pieces of data.
                bool returnValue = data.Length == 2;

                if (returnValue)
                {
                    // we expect the first piece of data to be the issued key in string format
                    // the second to be the remember me boolean
                    bool remember = false;
                    returnValue = bool.TryParse(data[1], out remember);
                    if (returnValue)
                    {
                        Key = data[0];
                        RemberMe = remember;
                    }
                }

                return returnValue;
            }
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public ProviderCurrentMember Instance
        {
            get
            {
                ProviderCurrentMember currentMember;

                // First see if the member can be retrieved from the current HttpRequest
                if (HttpContext.Current.Items.Contains(_currentUserKey))
                {
                    currentMember = (ProviderCurrentMember)HttpContext.Current.Items[_currentUserKey];
                }
                else if (HttpContext.Current.Session[_currentUserKey] != null)
                {
                    // pull the member's id from the session and store it in the HttpRequest
                    long memberId = (long)HttpContext.Current.Session[_currentUserKey];
                    currentMember = new ProviderCurrentMember();
                    if (!currentMember.Load(memberId))
                    {
                        // the member no longer exists and was possibly deleted by an admin so perform a logoff
                        currentMember.LogOff();
                    }
                    else
                    {
                        currentMember._isLoggedOn = true;
                        HttpContext.Current.Items[_currentUserKey] = currentMember;
                    }
                }
                else if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    // If it wasn't in the cache then check if the member had an encrypted cookie
                    // which would mean the user is coming back and should be auto-logged in.
                    LoginTicket aTicket = new LoginTicket();
                    if (!aTicket.ParseTicket(((FormsIdentity)HttpContext.Current.User.Identity).Ticket))
                    {
                        // bad data?! Clear the ticket
                        FormsAuthentication.SignOut();

                        // just create an anonymous member
                        currentMember = new ProviderCurrentMember();
                    }
                    else
                    {
                        currentMember = new ProviderCurrentMember();
                        List<string> errorList = new List<string>();// we will ignore the error list for now.

                        if (currentMember.Login(aTicket.Key, null, aTicket.RemberMe, ref errorList) == LoginEnum.success)
                        {
                            // store the member
                            HttpContext.Current.Session[_currentUserKey] = currentMember.Id;
                            HttpContext.Current.Items[_currentUserKey] = currentMember;
                        }
                    }
                }
                else
                {
                    // just create an anonymous member
                    currentMember = new ProviderCurrentMember();
                }

                return currentMember;
            }

            set
            {
                HttpContext.Current.Session[_currentUserKey] = value;
            }
        }

        public enum LoginEnum
        {
            success = 0,
            unknown,
            expired,
            invalid,
            banned
        }
    }
}