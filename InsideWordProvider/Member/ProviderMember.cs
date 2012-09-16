using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using InsideWordResource;
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace InsideWordProvider
{
    public class ProviderMember : Provider
    {
        protected Member _entityMember;
        protected ChangeList<long> _pendingPostedPhotoIds;

        public ProviderMember() : base() { }
        public ProviderMember(long id) : base(id) { }

        public ProviderMember(MailAddress email)
        {
            LoadByEmail(email);
        }

        public DateTime CreateDate
        {
            get { return _entityMember.CreateDate; }
            set { _entityMember.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityMember.EditDate; }
            set { _entityMember.EditDate = value; }
        }

        public bool IsActive
        {
            get { return HasValidAltId(ProviderAlternateMemberId.AlternateType.OpenId) ||
                         HasValidAltId(ProviderAlternateMemberId.AlternateType.Domain) ||
                         (HasPassword && (HasValidAltId(ProviderAlternateMemberId.AlternateType.Email) || 
                                          HasValidAltId(ProviderAlternateMemberId.AlternateType.UserName)));
                }
        }

        /// <summary>
        /// A member that is banned loses all rights of every kind
        /// </summary>
        public bool IsBanned
        {
            get { return _entityMember.IsBanned; }
            set { _entityMember.IsBanned = value; }
        }

        public bool HasAdminRights
        {
            get
            {
                return _entityMember.IsSuperAdmin ||
                       _entityMember.IsMasterAdmin ||
                       _entityMember.IsUserAdmin ||
                       _entityMember.IsCategoryAdmin ||
                       _entityMember.IsArticleAdmin;
            }
        }

        /// <summary>
        /// A super admin is the highest possible admin level. The difference
        /// between this admin and the Master Admin is the following:
        /// - Can edit/delete Super admins
        /// - Can edit/delete Master admins
        /// </summary>
        public bool IsSuperAdmin
        {
            get { return _entityMember.IsSuperAdmin; }
            set { _entityMember.IsSuperAdmin = value; }
        }

        /// <summary>
        /// A master admin is the second highest possible admin level. This admin
        /// has access to everything except it cannot edit/delete super admins and
        /// master admins
        /// </summary>
        public bool IsMasterAdmin
        {
            get { return _entityMember.IsMasterAdmin; }
            set { _entityMember.IsMasterAdmin = value; }
        }

        /// <summary>
        /// A member admin has control over editing users as long as they are not an admin.
        /// A member adming also cannot delete users.
        /// </summary>
        public bool IsMemberAdmin
        {
            get { return _entityMember.IsUserAdmin; }
            set { _entityMember.IsUserAdmin = value; }
        }

        /// <summary>
        /// A member admin has the right to edit and move categories but does not have
        /// the right to delete a category.
        /// </summary>
        public bool IsCategoryAdmin
        {
            get { return _entityMember.IsCategoryAdmin; }
            set { _entityMember.IsCategoryAdmin = value; }
        }

        /// <summary>
        /// An article admin has the right to edit articles but does not have the right
        /// to delete an article.
        /// </summary>
        public bool IsArticleAdmin
        {
            get { return _entityMember.IsArticleAdmin; }
            set { _entityMember.IsArticleAdmin = value; }
        }

        public string Password
        {
            set { _entityMember.Password = Hasher.HashPassword(value); }
        }

        public bool HasPassword
        {
            get { return !string.IsNullOrEmpty(_entityMember.Password); }
        }

        /// <summary>
        /// Temporary keys usually sent to members in e-mails to allow them to accomplish certain tasks such as:
        /// - Activating their account
        /// - Resetting their password
        /// - Anonymously editing their articles
        /// </summary>
        public ProviderIssuedKey NextNonceIssuedKey()
        {
            ProviderIssuedKey nonceKey = new ProviderIssuedKey();
            nonceKey.CreateDate = DateTime.UtcNow;
            nonceKey.EditDate = DateTime.UtcNow;
            nonceKey.IsNonce = true;
            nonceKey.MemberId = Id.Value;
            nonceKey.Save();
            return nonceKey;
        }

        public ProviderIssuedKey CurrentMonthIssuedKey
        {
            get
            {
                ProviderIssuedKey currentKey = new ProviderIssuedKey();
                if (Id.HasValue)
                {
                    currentKey.LoadOrCreate(Id.Value, null, false, 1, true);
                }
                return currentKey;
            }
        }

        public ProviderIssuedKey NextMonthIssuedKey
        {
            get
            {
                ProviderIssuedKey nextKey = new ProviderIssuedKey();
                if (Id.HasValue)
                {
                    nextKey.LoadOrCreate(Id.Value, null, false, 2, true);
                }
                return nextKey;
            }
        }

        public bool IsAnonymous
        {
            get
            {
                return !HasValidAltId(ProviderAlternateMemberId.AlternateType.UserName) &&
                       !HasValidAltId(ProviderAlternateMemberId.AlternateType.Domain);
            }
        }

        /// <summary>
        /// Displays the member identification as they would like to display for public purposes
        /// </summary>
        public string DisplayName
        {
            get
            {
                string returnValue = null;
                if (IsAnonymous)
                {
                    returnValue = "Anonymous";
                }
                else
                {
                    List<ProviderUserName> userNameList = UserNames;
                    if (userNameList.Count > 0)
                    {
                        returnValue = userNameList[0].UserName;
                    }
                    else
                    {
                        List<ProviderDomain> domainList = Domains;
                        if (domainList.Count > 0)
                        {
                            returnValue = domainList[0].DisplayName;
                        }
                        else
                        {
                            List<ProviderEmail> emailList = Emails;
                            if (emailList.Count > 0)
                            {
                                returnValue = emailList[0].Email.Address;
                            }
                        }
                    }
                }
                returnValue = returnValue ?? "Anonymous";
                return returnValue;
            }
        }

        /// <summary>
        /// Displays the member identification that is considered most relevant for Administrative purposes.
        /// </summary>
        public string DisplayAdministrativeName
        {
            get
            {
                string returnValue = null;
                List<ProviderEmail> emailList = Emails;
                if (emailList.Count > 0)
                {
                    returnValue = emailList[0].Email.Address;
                }
                else
                {
                    List<ProviderDomain> domainList = Domains;
                    if (domainList.Count > 0)
                    {
                        returnValue = domainList[0].DisplayName;
                    }
                    else
                    {
                        List<ProviderUserName> userNameList = UserNames;
                        if (userNameList.Count > 0)
                        {
                            returnValue = userNameList[0].UserName;
                        }
                    }
                }

                returnValue = returnValue ?? "Anonymous";
                return returnValue;
            }
        }

        public bool HasValidAltId(ProviderAlternateMemberId.AlternateType anAltType)
        {
            return Id.HasValue && _entityMember.AlternateMemberIds
                                                    .Any(altId => altId.IsValidated &&
                                                                    altId.AlternateType == (int)anAltType);
        }

        public List<ProviderUserName> UserNames
        {
            get
            {
                return (Id.HasValue)
                       ? _entityMember.AlternateMemberIds
                                      .Where(altId => altId.AlternateType == (int)InsideWordProvider.ProviderAlternateMemberId.AlternateType.UserName)
                                      .ToList()
                                      .ConvertAll(ProviderUserName._converterEntityToProvider)
                       : new List<ProviderUserName>();
            }
        }

        public List<ProviderOpenId> OpenIds
        {
            get
            {
                return (Id.HasValue)
                        ? _entityMember.AlternateMemberIds
                                        .Where(altId => altId.AlternateType == (int)InsideWordProvider.ProviderAlternateMemberId.AlternateType.OpenId)
                                        .ToList()
                                        .ConvertAll(ProviderOpenId._converterEntityToProvider)
                        : new List<ProviderOpenId>();
            }
        }

        public List<ProviderEmail> Emails
        {
            get
            {
                return (Id.HasValue)
                        ? _entityMember.AlternateMemberIds
                                        .Where(altId => altId.AlternateType == (int)InsideWordProvider.ProviderAlternateMemberId.AlternateType.Email)
                                        .ToList()
                                        .ConvertAll(ProviderEmail._converterEntityToProvider)
                        : new List<ProviderEmail>();
            }
        }

        public List<ProviderDomain> Domains
        {
            get
            {
                return (Id.HasValue)
                        ? _entityMember.AlternateMemberIds
                                        .Where(altId => altId.AlternateType == (int)InsideWordProvider.ProviderAlternateMemberId.AlternateType.Domain)
                                        .ToList()
                                        .ConvertAll(ProviderDomain._converterEntityToProvider)
                        : new List<ProviderDomain>();
            }
        }

        public bool HasAlternateCategories
        {
            get { return Id.HasValue && _entityMember.AlternateCategoryIds.Count() > 0; }
        }

        public List<ProviderAlternateCategoryId> AlternateCategoryList
        {
            get { return _entityMember.AlternateCategoryIds
                                        .ToList()
                                        .ConvertAll(ProviderAlternateCategoryId._converterEntityToProvider); }
        }

        public string Bio
        {
            get { return _entityMember.Bio; }
            set { _entityMember.Bio = value; }
        }

        public long? ProfilePhotoId
        {
            get { return _entityMember.PhotoId; }
            set { _entityMember.PhotoId = value; }
        }

        public List<ProviderGroup> Groups
        {
            get { return ProviderGroup.LoadByMemberId(_entityMember.Id); }
        }

        public List<ProviderRole> RolesByGroup(long groupId)
        {
            return ProviderRole.LoadMemberRolesWithinGroup(groupId, _entityMember.Id);
        }

        /* TODO
        public Dictionary<ProviderGroup, List<ProviderRole>> RolesByGroup()
        {
            Dictionary<ProviderGroup, List<ProviderRole>> groupsAndRoles = new Dictionary<ProviderGroup, List<ProviderRole>>();
            foreach (ProviderGroup group in Groups)
            {
                groupsAndRoles.Add(group, group.Roles);
            }
            return groupsAndRoles;
        }
        */

        /// <summary>
        /// Photo Ids for Posted Photos
        /// </summary>
        public List<long> PostedPhotoIds
        {
            get
            {
                return _entityMember.PostedPhotos
                                     .Select(photo => photo.Id)
                                     .ToList();
            }
        }

        public void AddPostedPhoto(long photoId)
        {
            _pendingPostedPhotoIds.Add(photoId);
        }

        public void RemovePostedPhoto(long photoId)
        {
            _pendingPostedPhotoIds.Remove(photoId);
        }

        /// <summary>
        /// Indicates if a given member can modify the given object.
        /// </summary>
        /// <param name="editable">object we wish to check the rights for with regards to this member.</param>
        /// <returns>true if the member can edit it and false otherwise.</returns>
        public virtual bool CanEdit(Object editable)
        {
            bool returnValue = IsSuperAdmin;

            if(!returnValue)
            {
                if (editable is ProviderMember)
                {
                    //can edit this member as long as he isn't equal to or lower in admin rank
                    ProviderMember editMember = (ProviderMember)editable;
                    returnValue = IsActive && !IsBanned &&
                                ((IsMasterAdmin && !(editMember.IsSuperAdmin || editMember.IsMasterAdmin)) ||
                                    (IsMemberAdmin && !editMember.HasAdminRights));
                }
                else if (editable is ProviderArticle)
                {
                    // Can edit this article as long as they are the owner or have the admin rights
                    // An account does not need to be active to edit an article.
                    ProviderArticle editArticle = (ProviderArticle)editable;
                    return editArticle.IsNew ||
                            ( !IsBanned && Owns(editArticle) ) ||
                            ( IsMasterAdmin || IsArticleAdmin );
                }
                else if(editable is ProviderComment)
                {
                    //can edit this comment as long as they are the owner or have the admin rights
                    ProviderComment editComment = (ProviderComment)editable;
                    returnValue = IsActive && !IsBanned &&
                                    ( editComment.IsNew || Owns(editComment) || IsMasterAdmin || IsMemberAdmin );
                }
                else if(editable is ProviderCategory)
                {
                    //can edit this category as long as they have the admin rights
                    returnValue = IsActive && !IsBanned &&
                                    (IsMasterAdmin || IsCategoryAdmin);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Function indicates if a person owns a given object.
        /// Note: ownership doesn't always mean they can edit.
        /// </summary>
        /// <param name="property">object whose ownership is in question</param>
        /// <returns>true if the member owns this object</returns>
        virtual public bool Owns(Object property)
        {
            bool returnValue = false;

            if (property is ProviderArticle)
            {
                ProviderArticle anArticle = (ProviderArticle)property;
                returnValue = anArticle.IsNew || (Id.HasValue && anArticle.MemberId == Id);
            }
            else if (property is ProviderComment)
            {
                returnValue = ((ProviderComment)property).MemberId == Id;
            }
            else if (property is ProviderMember)
            {
                returnValue = (Id.HasValue && ((ProviderMember)property).Id == Id);
            }

            return returnValue;
        }

        static protected Func<InsideWordEntities, long, Member> _loadByMemberId = null;
        override public bool Load(long memberId)
        {
            if (_loadByMemberId == null)
            {
                // compile this query
                _loadByMemberId = CompiledQuery.Compile<InsideWordEntities, long, Member>(
                    (ctx, aMemberId) => ctx.Members
                                            .Include("AlternateMemberIds")
                                            .Include("AlternateCategoryIds")
                                            .Where(member => member.Id == aMemberId)
                                            .FirstOrDefault()
                );
            }

            Member entityMember = _loadByMemberId.Invoke(DbCtx.Instance, memberId);
            return Load(entityMember);
        }

        public bool LoadByEmail(MailAddress email)
        {
            long? id = ProviderEmail.FindOwner(email, true);
            return id.HasValue && Load(id.Value);
        }

        public bool LoadByUserName(string userName)
        {
            long? id = ProviderUserName.FindOwner(userName);
            return id.HasValue && Load(id.Value);
        }

        override public void Save()
        {
            //If no members exist yet then set this one as a super admin
            if (IsNew)
            {
                if (_memberCountIsZero)
                {
                    _entityMember.IsSuperAdmin = true;
                }
                // If it's new then we have to save the object first before creating the relationships
                base.Save();
            }

            //Handle the pending photo ids
            //Note we have to violate encapsulation of the entities
            foreach (long photoId in _pendingPostedPhotoIds.AddList)
            {
                // don't add if it already exists
                if (!_entityMember.PostedPhotos.Any(photo => photo.Id == photoId))
                {
                    Photo aPhoto = DbCtx.Instance.Photos.Where(photo => photo.Id == photoId).SingleOrDefault();
                    _entityMember.PostedPhotos.Add(aPhoto);
                    aPhoto.Members.Add(_entityMember);//don't forget the relationship to the photo or else it's one way
                }
            }

            foreach (long photoId in _pendingPostedPhotoIds.RemoveList)
            {
                Photo aPhoto = _entityMember.PostedPhotos.Where(photo => photo.Id == photoId).SingleOrDefault();
                // don't remove if it doesn't exist
                if (aPhoto != null)
                {
                    _entityMember.PostedPhotos.Remove(aPhoto);
                    aPhoto.Members.Remove(_entityMember);//don't forget the relationship to the photo or else it's one way
                }
            }

            _pendingPostedPhotoIds.Clear();

            base.Save();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityMember.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityMember.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityMember.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityMember.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityMember.EditDate);
            sb.Append("\n\tIsSuperAdmin =\t" + _entityMember.IsSuperAdmin);
            sb.Append("\n\tIsMasterAdmin =\t" + _entityMember.IsMasterAdmin);
            sb.Append("\n\tIsUserAdmin =\t" + _entityMember.IsUserAdmin);
            sb.Append("\n\tIsCategoryAdmin =\t" + _entityMember.IsCategoryAdmin);
            sb.Append("\n\tIsArticleAdmin =\t" + _entityMember.IsArticleAdmin);
            sb.Append("\n\tIsBanned =\t" + _entityMember.IsBanned);
            sb.Append("\n");

            return sb.ToString();
        }

        /* TODO: revisit all copy functions
        override public bool Copy(Provider untyped)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            //Do not copy over the system times and only the business logic
            //times since the system times are specific to a given instance.
            ProviderMember aMember = (ProviderMember)untyped;
            _entityMember.CreateDate = aMember._entityMember.CreateDate;
            _entityMember.EditDate = aMember._entityMember.EditDate;
            _entityMember.Email = aMember._entityMember.Email;
            _entityMember.IsSuperAdmin = aMember._entityMember.IsSuperAdmin;
            _entityMember.IsMasterAdmin = aMember._entityMember.IsMasterAdmin;
            _entityMember.IsCategoryAdmin = aMember._entityMember.IsCategoryAdmin;
            _entityMember.IsUserAdmin = aMember._entityMember.IsUserAdmin;
            _entityMember.IsArticleAdmin = aMember._entityMember.IsArticleAdmin;
            _entityMember.IsBanned = aMember._entityMember.IsBanned;
            _entityMember.UserName = aMember._entityMember.UserName;

            // for security reasons all these attributes are handled differently
            //_entityMember.Password = aMember._entityMember.Password; Do not copy password for security reasons
            //_entityMember.IssuedKey = aMember._entityMember.IssuedKey; Do not copy IssuedKey for security reasons
            //_entityMember.OpenId = aMember._entityMember.OpenId; Do not copy OpenId for security reasons
            _entityMember.IsValidEmail = false;

            _pendingPostedPhotoIds.Copy(aMember._pendingPostedPhotoIds);

            _entityObject = _entityMember;
            return true;
        }
        */

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected internal ProviderMember(Member aMember) : base(aMember) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityMember; }
            set { _entityMember = (Member)value; }
        }

        protected override void ClassClear()
        {
            base.ClassClear();
            _pendingPostedPhotoIds = new ChangeList<long>();
        }

        protected override void EntityClear()
        {
            _entityMember = new Member();
            _entityMember.SystemCreateDate = new DateTime();
            _entityMember.SystemEditDate = new DateTime();
            _entityMember.CreateDate = new DateTime();
            _entityMember.EditDate = new DateTime();
            _entityMember.IsSuperAdmin = false;
            _entityMember.IsMasterAdmin = false;
            _entityMember.IsUserAdmin = false;
            _entityMember.IsCategoryAdmin = false;
            _entityMember.IsArticleAdmin = false;
            _entityMember.IsBanned = false;
            _entityMember.Id = -1;
            _entityMember.Password = null;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        public const int UserNameSize = 20;
        public const int EmailSize = 64;
        public const int PasswordSize = 16;
        public const int IdDigitSizeSize = 18;
        public const int BioSize = 1024;

        static protected bool _memberCountIsZero;

        static ProviderMember()
        {
            using(InsideWordEntities dbContext = new InsideWordEntities())
            {
                 _memberCountIsZero = dbContext.Members.Count() == 0;
            }
        }

        static protected Converter<Member, ProviderMember> _converterEntityToProvider = new Converter<Member, ProviderMember>(_EntityToProvider);
        static protected ProviderMember _EntityToProvider(Member memberEntity)
        {
            return new ProviderMember(memberEntity);
        }

        static public List<ProviderMember> LoadAll()
        {
            return DbCtx.Instance.Members.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderMember> Load(IProviderMemberFilter filter)
        {
            int m = (filter.Page - 1);
            if (m < 0) m = 0;
            int skip = filter.Rows * m;
            IQueryable<Member> query = DbCtx.Instance.Members;
            query = MemberFilter(filter, query);
            query = MemberSort(filter, query);
            return query.Skip(skip)
                        .Take(filter.Rows)
                        .ToList()
                        .ConvertAll(_converterEntityToProvider);
        }

        static public int Count(IProviderMemberFilter filter)
        {
            IQueryable<Member> query = DbCtx.Instance.Members;
            query = MemberFilter(filter, query);
            return query.Count();
        }

        static protected IQueryable<Member> MemberSort(IProviderMemberFilter filter, IQueryable<Member> query)
        {
            // Sort grid data.  Sord is sort order, Sidx is the index to sort on
            if (filter.Sord == "desc")
            {
                // Sort by date if we don't know what this was
                query = query.OrderByDescending(aMember => aMember.SystemCreateDate);
            }
            else if (filter.Sord == "asc")
            {
                // Sort by date if we don't know what this was
                query = query.OrderBy(aMember => aMember.SystemCreateDate);
            }

            return query;
        }

        static protected IQueryable<Member> MemberFilter(IProviderMemberFilter filter, IQueryable<Member> query)
        {
            int userNameType = (int)ProviderAlternateMemberId.AlternateType.UserName;
            int emailType = (int)ProviderAlternateMemberId.AlternateType.Email;
            int openIdType = (int)ProviderAlternateMemberId.AlternateType.OpenId;
            return query.Where(aMember => ((filter.Id == null || aMember.Id == filter.Id) &&
                                                         (filter.UserName == null || aMember.AlternateMemberIds
                                                                                                   .Where(altId => altId.AlternateType == userNameType &&
                                                                                                                    altId.AlternateId.Contains(filter.UserName))
                                                                                                   .Count() > 0) &&
                                                         (filter.Email == null || aMember.AlternateMemberIds
                                                                                                   .Where(altId => altId.AlternateType == emailType &&
                                                                                                                    altId.AlternateId.Contains(filter.Email) &&
                                                                                                                    (filter.IsValidEmail == null || altId.IsValidated == filter.IsValidEmail))
                                                                                                   .Count() > 0) &&
                                                         (filter.HasPassword == null || (aMember.Password != null) == filter.HasPassword) &&
                                                         (filter.HasOpenId == null || (aMember.AlternateMemberIds
                                                                                                    .Where(altId => altId.AlternateType == openIdType)
                                                                                                    .Count() > 0) == filter.HasOpenId) &&
                                                         (filter.IsSuperAdmin == null || aMember.IsSuperAdmin == filter.IsSuperAdmin) &&
                                                         (filter.IsMasterAdmin == null || aMember.IsMasterAdmin == filter.IsMasterAdmin) &&
                                                         (filter.IsMemberAdmin == null || aMember.IsUserAdmin == filter.IsMemberAdmin) &&
                                                         (filter.IsCategoryAdmin == null || aMember.IsCategoryAdmin == filter.IsCategoryAdmin) &&
                                                         (filter.IsArticleAdmin == null || aMember.IsArticleAdmin == filter.IsArticleAdmin) &&
                                                         (filter.IsActive == null || ((aMember.AlternateMemberIds
                                                                                                      .Where(altId => altId.AlternateType == openIdType)
                                                                                                      .Count() > 0) ||
                                                                                              (aMember.Password != null && (aMember.AlternateMemberIds
                                                                                                                                    .Where(altId => altId.IsValidated == true &&
                                                                                                                                                     (altId.AlternateType == emailType ||
                                                                                                                                                       altId.AlternateType == userNameType))
                                                                                                                                    .Count() > 0))
                                                                                            ) == filter.IsActive) &&
                                                         (filter.IsBanned == null || aMember.IsBanned == filter.IsBanned) &&
                                                         (filter.IsSuperAdmin == null || aMember.IsSuperAdmin == filter.IsSuperAdmin))
                                            );
        }

        static public void Delete(long id)
        {
            ProviderMember aMember = new ProviderMember(id);
            aMember.Delete();
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id > -1 && DbCtx.Instance.Members.Any(aMember => aMember.Id == id);
        }

        /// <summary>
        /// Indicates if an object with a given Id and password exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <param name="password">password of the object we wish to check</param>
        /// <returns></returns>
        static protected Func<InsideWordEntities, long, string, bool> _existsIdAndPassword = null;
        static public bool Exists(long id, string password)
        {
            bool returnValue = !string.IsNullOrEmpty(password);
            if (returnValue)
            {
                if (_existsIdAndPassword == null)
                {
                    _existsIdAndPassword = CompiledQuery.Compile<InsideWordEntities, long, string, bool>(
                        (ctx, aMemberId, hashedPassword) => ctx.Members
                                                            .Any(aMember => aMember.Id == aMemberId && aMember.Password == hashedPassword)
                    );
                }

                string hashed= Hasher.HashPassword(password);
                returnValue = id > -1 && _existsIdAndPassword.Invoke( DbCtx.Instance, id, hashed);
            }

            return returnValue;
        }

        static public List<ProviderMember> LoadByGroupId(long groupId)
        {
            return DbCtx.Instance.Memberships
                .Where(aMembership => aMembership.GroupId == groupId)
                .Select(aMembership => aMembership.Member)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }
    }
}
