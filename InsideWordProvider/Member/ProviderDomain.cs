using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Net.Mail;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InsideWordProvider
{
    public class ProviderDomain : ProviderAlternateMemberId
    {
        public ProviderDomain() : base() { }
        public ProviderDomain(long id) : base(id) { }

        public Uri Domain
        {
            get { return new Uri(_entityAlternateMemberId.AlternateId); }
            set { _entityAlternateMemberId.AlternateId = value.AbsoluteUri; }
        }

        static protected Regex _regexDomainStrip = new Regex(@"(http(s?)://(www\.)?)|(/$)");
        override public string DisplayName
        {
            get
            {
                return _regexDomainStrip.Replace(_entityAlternateMemberId.AlternateId, "");
            }
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================    
        protected ProviderDomain(AlternateMemberId anAltType) : base(anAltType) { }

        protected override void EntityClear()
        {
            base.EntityClear();
            _entityAlternateMemberId.AlternateType = (int)AlternateType.Domain;
            _entityAlternateMemberId.IsValidated = false;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static internal Converter<AlternateMemberId, ProviderDomain> _converterEntityToProvider = new Converter<AlternateMemberId, ProviderDomain>(_EntityToProvider);
        static internal ProviderDomain _EntityToProvider(AlternateMemberId anAltType)
        {
            return new ProviderDomain(anAltType);
        }

        static public long? FindOwner(Uri domain, bool isValidated)
        {
            return _FindOwner(domain.AbsoluteUri, isValidated, AlternateType.Domain);
        }

        static public List<ProviderDomain> LoadBy(long memberId)
        {
            List<ProviderDomain> returnList = new List<ProviderDomain>();
            if (memberId > -1)
            {
                int intAltType = (int)AlternateType.Domain;
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
            return _CountBy(memberId, isValidated, AlternateType.Domain);
        }
    }
}