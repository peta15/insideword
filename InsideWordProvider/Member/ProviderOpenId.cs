using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Net.Mail;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;

namespace InsideWordProvider
{
    public class ProviderOpenId : ProviderAlternateMemberId
    {
        public ProviderOpenId()
        {
            Clear();
        }

        public ProviderOpenId(long id)
        {
            Load(id);
        }

        public string OpenId
        {
            set { _entityAlternateMemberId.AlternateId = Hasher.HashPassword(value); }
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================    
        protected ProviderOpenId(AlternateMemberId anAltType) : base(anAltType) { }

        protected override void EntityClear()
        {
            base.EntityClear();
            _entityAlternateMemberId.AlternateType = (int)AlternateType.OpenId;
            _entityAlternateMemberId.IsValidated = true;
            _entityAlternateMemberId.IsHashed = true;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static internal Converter<AlternateMemberId, ProviderOpenId> _converterEntityToProvider = new Converter<AlternateMemberId, ProviderOpenId>(_EntityToProvider);
        static internal ProviderOpenId _EntityToProvider(AlternateMemberId anAltType)
        {
            return new ProviderOpenId(anAltType);
        }

        static public long? FindOwner(string openId)
        {
            return _FindOwner(Hasher.HashPassword(openId), true, AlternateType.OpenId);
        }

        static public List<ProviderOpenId> LoadBy(long memberId)
        {
            List<ProviderOpenId> returnList = new List<ProviderOpenId>();
            if (memberId > -1)
            {
                int intAltType = (int)AlternateType.OpenId;
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
            return _CountBy(memberId, isValidated, AlternateType.OpenId);
        }
        
    }
}