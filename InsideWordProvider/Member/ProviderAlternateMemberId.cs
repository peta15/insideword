using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Net.Mail;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderAlternateMemberId : Provider
    {
        protected AlternateMemberId _entityAlternateMemberId;

        public ProviderAlternateMemberId() : base() { }
        public ProviderAlternateMemberId(long id) : base(id) { }

        public DateTime CreateDate
        {
            get { return _entityAlternateMemberId.CreateDate; }
            set { _entityAlternateMemberId.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityAlternateMemberId.EditDate; }
            set { _entityAlternateMemberId.EditDate = value; }
        }

        public DateTime? ExpiryDate
        {
            get { return _entityAlternateMemberId.ExpiryDate; }
            set { _entityAlternateMemberId.ExpiryDate = value; }
        }

        /// <summary>
        /// Attribute returns if this alternate member id has expired
        /// </summary>
        /// <returns></returns>
        public bool HasExpired
        {
            get { return ExpiryDate.HasValue && ExpiryDate.Value.CompareTo(DateTime.UtcNow) < 0; }
        }

        public long MemberId
        {
            get { return _entityAlternateMemberId.MemberId; }
            set { _entityAlternateMemberId.MemberId = value; }
        }

        public bool IsValidated
        {
            get { return _entityAlternateMemberId.IsValidated; }
            set { _entityAlternateMemberId.IsValidated = value; }
        }

        public bool IsNonce
        {
            get { return _entityAlternateMemberId.IsNonce; }
        }

        public bool UsePassword
        {
            get { return _entityAlternateMemberId.UsePassword; }
        }

        public bool IsHidden
        {
            get { return _entityAlternateMemberId.IsHidden; }
        }

        public long LoginCount
        {
            get { return _entityAlternateMemberId.LoginCount; }
        }

        public string Data
        {
            get { return _entityAlternateMemberId.Data; }
            set { _entityAlternateMemberId.Data = value; }
        }

        public virtual string DisplayName
        {
            get { return _entityAlternateMemberId.DisplayName; }
            set { _entityAlternateMemberId.DisplayName = value; }
        }

        public override bool Load(long id)
        {
            AlternateMemberId altId = DbCtx.Instance.AlternateMemberIds
                                            .Where(anAltId => anAltId.Id == id)
                                            .First();
            
            return Load(altId);
        }

        static protected Func<InsideWordEntities, string, string, bool, AlternateMemberId> _loadByAlternateId = null;
        public bool Load(string alternateId)
        {
            if (_loadByAlternateId == null)
            {
                _loadByAlternateId = CompiledQuery.Compile<InsideWordEntities, string, string, bool, AlternateMemberId>(
                    (ctx, anAlternateId, hashedId, isVal) => ctx.AlternateMemberIds
                                                       .Where(anAltId => (anAltId.AlternateId == anAlternateId ||
                                                                         (anAltId.IsHashed && anAltId.AlternateId == hashedId)) &&
                                                                        anAltId.IsValidated == isVal
                                                             )
                                                       .SingleOrDefault());
            }

            string hashed = Hasher.HashPassword(alternateId);
            // try the validated value first
            AlternateMemberId altId = _loadByAlternateId.Invoke(DbCtx.Instance, alternateId, hashed, true);
            if (altId == null)
            {
                // try the invalid one second
                altId = _loadByAlternateId.Invoke(DbCtx.Instance, alternateId, hashed, false);
            }
            return Load(altId);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityAlternateMemberId.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityAlternateMemberId.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityAlternateMemberId.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityAlternateMemberId.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityAlternateMemberId.EditDate);
            sb.Append("\n\tExpiryDate =\t" + _entityAlternateMemberId.ExpiryDate);
            sb.Append("\n\tMemberId =\t" + _entityAlternateMemberId.MemberId);
            sb.Append("\n\tAlternateType =\t" + _entityAlternateMemberId.AlternateType);
            sb.Append("\n\tAlternateId =\t" + _entityAlternateMemberId.AlternateId);
            sb.Append("\n\tIsValidated =\t" + _entityAlternateMemberId.IsValidated);
            sb.Append("\n\tIsNonce =\t" + _entityAlternateMemberId.IsNonce);
            sb.Append("\n\tUsePassword =\t" + _entityAlternateMemberId.UsePassword);
            sb.Append("\n\tIsHidden =\t" + _entityAlternateMemberId.IsHidden);
            sb.Append("\n\tLoginCount =\t" + _entityAlternateMemberId.LoginCount);
            sb.Append("\n\tData =\t" + _entityAlternateMemberId.Data);
            sb.Append("\n\tIsHashed =\t" + _entityAlternateMemberId.IsHashed);
            sb.Append("\n");

            return sb.ToString();
        }

        /*
        public override bool Copy(Provider untyped)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            //Do not copy over the system times and only the business logic
            //times since the system times are specific to a given instance.
            ProviderAlternateMemberId anAlternateMemberId = (ProviderAlternateMemberId)untyped;
            _entityAlternateMemberId.MemberId = anAlternateMemberId._entityAlternateMemberId.MemberId;
            _entityAlternateMemberId.AlternateType = anAlternateMemberId._entityAlternateMemberId.AlternateType;
            _entityAlternateMemberId.AlternateId = anAlternateMemberId._entityAlternateMemberId.AlternateId;
            _entityAlternateMemberId.IsValidated = anAlternateMemberId._entityAlternateMemberId.IsValidated;
            _entityAlternateMemberId.IsNonce = anAlternateMemberId._entityAlternateMemberId.IsNonce;
            _entityAlternateMemberId.ExpiryDate = anAlternateMemberId._entityAlternateMemberId.ExpiryDate;
            _entityAlternateMemberId.EditDate = anAlternateMemberId._entityAlternateMemberId.EditDate;
            _entityAlternateMemberId.CreateDate = anAlternateMemberId._entityAlternateMemberId.CreateDate;
            _entityAlternateMemberId.UsePassword = anAlternateMemberId._entityAlternateMemberId.UsePassword;
            _entityAlternateMemberId.IsHidden = anAlternateMemberId._entityAlternateMemberId.IsHidden;
            _entityObject = _entityAlternateMemberId;
            return true;
        }
        */

        public bool ValidateData()
        {
            bool returnValue = true;
            if (!string.IsNullOrWhiteSpace(Data))
            {
                MailAddress email = null;

                // if this is an alt id used to validate an e-mail then do so.
                if (IWStringUtility.TryParse(Data, out email))
                {
                    ProviderEmail altIdEmail = new ProviderEmail();
                    if (altIdEmail.Load(email.Address))
                    {
                        altIdEmail.IsValidated = true;
                        altIdEmail.Save();
                    }
                }
                else
                {
                    Uri domain = null;

                    // if this is an alt id used to validate an a domain then do so.
                    if (Uri.TryCreate(Data, UriKind.Absolute, out domain))
                    {
                        ProviderDomain altIdDomain = new ProviderDomain();
                        if (altIdDomain.Load(domain.AbsoluteUri))
                        {
                            altIdDomain.IsValidated = true;
                            altIdDomain.Save();
                        }
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Method attempts to delete the alternate member id if it has expired or is a nonce.
        /// </summary>
        /// <returns>true if the alternate id expired and false if the alternate id is still valid.</returns>
        public bool TryDecommission()
        {
            bool returnValue = IsNonce || HasExpired;
            if (returnValue)
            {
                Delete();
            }

            return returnValue;
        }

        /// <summary>
        /// Method that marks an alternate member id has having been used. This is for purposes of record keeping such has how many times a given alternate id has been used.
        /// </summary>
        public void Used()
        {
            _entityAlternateMemberId.LoginCount++;
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderAlternateMemberId(AlternateMemberId anAltType) : base(anAltType) { }
        
        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityAlternateMemberId; }
            set { _entityAlternateMemberId = (AlternateMemberId)value; }
        }

        protected override void EntityClear()
        {
            _entityAlternateMemberId = new AlternateMemberId();
            _entityAlternateMemberId.Id = -1;
            _entityAlternateMemberId.SystemCreateDate = new DateTime();
            _entityAlternateMemberId.SystemEditDate = new DateTime();
            _entityAlternateMemberId.CreateDate = new DateTime();
            _entityAlternateMemberId.EditDate = new DateTime();
            _entityAlternateMemberId.ExpiryDate = null;
            _entityAlternateMemberId.MemberId = -1;
            _entityAlternateMemberId.AlternateType = (int)AlternateType.None;
            _entityAlternateMemberId.AlternateId = string.Empty;
            _entityAlternateMemberId.IsValidated = false;
            _entityAlternateMemberId.IsNonce = false;
            _entityAlternateMemberId.UsePassword = false;
            _entityAlternateMemberId.IsHidden = false;
            _entityAlternateMemberId.LoginCount = 0;
            _entityAlternateMemberId.Data = null;
            _entityAlternateMemberId.IsHashed = false;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        public enum AlternateType
        {
            None = 0,
            UserName,
            Email,
            IssuedKey,
            OpenId,
            Domain
        }

        static protected long? _Exists(string alternateId, bool isValidated, AlternateType altType)
        {
            long? returnValue = null;
            if (!string.IsNullOrEmpty(alternateId))
            {
                int intAltType = (int)altType;
                returnValue = DbCtx.Instance.AlternateMemberIds
                                                 .Where(anAltId => anAltId.AlternateId == alternateId &&
                                                                   anAltId.IsValidated == isValidated &&
                                                                   (intAltType == 0 || anAltId.AlternateType == intAltType)
                                                       )
                                                 .Select(anAltId => anAltId.Id)
                                                 .DefaultIfEmpty(-1)
                                                 .Single();

                if (returnValue == -1)
                {
                    returnValue = null;
                }
            }
            return returnValue;
        }

        static protected long? _FindOwner(string alternateId, bool isValidated, AlternateType altType)
        {
            long? returnValue = null;
            if (!string.IsNullOrEmpty(alternateId))
            {
                int intAltType = (int)altType;
                returnValue = DbCtx.Instance.AlternateMemberIds
                                                 .Where(anAltId => anAltId.AlternateId == alternateId &&
                                                                   anAltId.IsValidated == isValidated &&
                                                                   (intAltType == 0 || anAltId.AlternateType == intAltType)
                                                       )
                                                 .Select(anAltId => anAltId.MemberId)
                                                 .DefaultIfEmpty(-1)
                                                 .Single();

                if (returnValue == -1)
                {
                    returnValue = null;
                }
            }
            return returnValue;
        }

        static protected int _CountBy(long memberId, bool? isValidated, AlternateType type)
        {
            int returnValue = 0;
            if (memberId > -1)
            {
                int intAltType = (int)type;
                returnValue = DbCtx.Instance.AlternateMemberIds
                                                 .Where(anAltId => anAltId.MemberId == memberId &&
                                                                   anAltId.AlternateType == intAltType &&
                                                                   (isValidated == null || anAltId.IsValidated == isValidated)
                                                       )
                                                 .Count();
            }
            return returnValue;
        }
    }
}