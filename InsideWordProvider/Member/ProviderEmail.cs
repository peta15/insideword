using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Net.Mail;
using System.Collections.Generic;

namespace InsideWordProvider
{
    public class ProviderEmail : ProviderAlternateMemberId
    {
        public ProviderEmail() : base()
        {
        }

        public ProviderEmail(long id) : this()
        {
            Load(id);
        }

        public MailAddress Email
        {
            get { return new MailAddress(_entityAlternateMemberId.AlternateId); }
            set { _entityAlternateMemberId.AlternateId = value.Address; }
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================    
        protected ProviderEmail(AlternateMemberId anAltType) : base(anAltType) { }

        protected override void EntityClear()
        {
            base.EntityClear();
            _entityAlternateMemberId.AlternateType = (int)AlternateType.Email;
            _entityAlternateMemberId.UsePassword = true;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static internal Converter<AlternateMemberId, ProviderEmail> _converterEntityToProvider = new Converter<AlternateMemberId, ProviderEmail>(_EntityToProvider);
        static internal ProviderEmail _EntityToProvider(AlternateMemberId anAltType)
        {
            return new ProviderEmail(anAltType);
        }

        static public long? FindOwner(MailAddress email, bool? isValidated = null)
        {
            long? memberId = null;
            bool isValid = true;
            if (isValidated.HasValue)
            {
                isValid = isValidated.Value;
            }
            _FindOwner(email.Address, isValid, AlternateType.Email);
            if (!memberId.HasValue && !isValidated.HasValue)
            {
                memberId = _FindOwner(email.Address, !isValid, AlternateType.Email);
            }
            return memberId;
        }

        static public List<ProviderEmail> LoadBy(long memberId)
        {
            List<ProviderEmail> returnList = new List<ProviderEmail>();
            if (memberId > -1)
            {
                int intAltType = (int)AlternateType.Email;
                returnList = DbCtx.Instance.AlternateMemberIds
                                                 .Where(anAltId => anAltId.MemberId == memberId &&
                                                                   anAltId.AlternateType == intAltType
                                                       )
                                                 .ToList()
                                                 .ConvertAll(_converterEntityToProvider);
            }
            return returnList;
        }

        static public int CountBy(long memberId, bool? isValidated)
        {
            return _CountBy(memberId, isValidated, AlternateType.Email);
        }
    }
}