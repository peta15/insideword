using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Collections.Generic;

namespace InsideWordProvider
{
    public class ProviderUserName : ProviderAlternateMemberId
    {
        public ProviderUserName()
        {
            Clear();
        }

        public ProviderUserName(long id)
        {
            Load(id);
        }

        public string UserName
        {
            get { return _entityAlternateMemberId.AlternateId; }
            set { _entityAlternateMemberId.AlternateId = value; }
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================    
        protected ProviderUserName(AlternateMemberId anAltType)
        {
            _entityAlternateMemberId = anAltType;
        }

        protected override void EntityClear()
        {
            base.EntityClear();
            _entityAlternateMemberId.AlternateType = (int)AlternateType.UserName;
            _entityAlternateMemberId.UsePassword = true;
            _entityAlternateMemberId.IsValidated = true;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static internal Converter<AlternateMemberId, ProviderUserName> _converterEntityToProvider = new Converter<AlternateMemberId, ProviderUserName>(_EntityToProvider);
        static internal ProviderUserName _EntityToProvider(AlternateMemberId anAltType)
        {
            return new ProviderUserName(anAltType);
        }

        static public long? FindOwner(string userName)
        {
            return _FindOwner(userName, true, AlternateType.UserName);
        }

        static public List<ProviderUserName> LoadBy(long memberId)
        {
            List<ProviderUserName> returnList = new List<ProviderUserName>();
            if (memberId > -1)
            {
                int intAltType = (int)AlternateType.UserName;
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
            return _CountBy(memberId, isValidated, AlternateType.UserName);
        }
    }
}