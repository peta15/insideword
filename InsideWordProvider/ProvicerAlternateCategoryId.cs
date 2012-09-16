using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace InsideWordProvider
{
    public class ProviderAlternateCategoryId : Provider
    {
        protected AlternateCategoryId _entityAlternateCategoryId;

        public ProviderAlternateCategoryId() : base() { }
        public ProviderAlternateCategoryId(long id) : base(id) { }

        public long MemberId
        {
            get { return _entityAlternateCategoryId.MemberId; }
            set { _entityAlternateCategoryId.MemberId = value; }
        }

        public long? CategoryId
        {
            get { return _entityAlternateCategoryId.CategoryId; }
            set { _entityAlternateCategoryId.CategoryId = value; }
        }

        public long AlternateId
        {
            get { return _entityAlternateCategoryId.AlternateId; }
            set { _entityAlternateCategoryId.AlternateId = value; }
        }

        public string DisplayName
        {
            get { return _entityAlternateCategoryId.DisplayName; }
            set { _entityAlternateCategoryId.DisplayName = value; }
        }

        public bool OverrideFlag
        {
            get { return _entityAlternateCategoryId.OverrideFlag; }
            set { _entityAlternateCategoryId.OverrideFlag = value; }
        }

        public override bool Load(long id)
        {
            AlternateCategoryId altId = DbCtx.Instance.AlternateCategoryIds
                                            .Where(anAltId => anAltId.Id == id)
                                            .SingleOrDefault();
            return Load(altId);
        }

        public bool Load(long alternateId, long memberId)
        {
            AlternateCategoryId altId = DbCtx.Instance.AlternateCategoryIds
                                               .Where(anAltId => anAltId.AlternateId == alternateId &&
                                                                 anAltId.MemberId == memberId)
                                               .SingleOrDefault();
            return Load(altId);
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("ProviderAlternateCategoryId\n");
            sb.Append("\tAlternateCategoryIdId -\t" + _entityAlternateCategoryId.Id+"\n");
            sb.Append("\tSystemCreateDate -\t" + _entityAlternateCategoryId.SystemCreateDate + "\n");
            sb.Append("\tSystemEditDate -\t" + _entityAlternateCategoryId.SystemEditDate + "\n");
            sb.Append("\tMemberId -\t" + _entityAlternateCategoryId.MemberId + "\n");
            sb.Append("\tCategoryId -\t" + _entityAlternateCategoryId.CategoryId + "\n");
            sb.Append("\tAlternateId -\t" + _entityAlternateCategoryId.AlternateId + "\n");
            sb.Append("\tDisplayName -\t" + _entityAlternateCategoryId.DisplayName + "\n");
            sb.Append("\tOverrideFlag -\t" + _entityAlternateCategoryId.OverrideFlag + "\n");
            return sb.ToString();
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderAlternateCategoryId(AlternateCategoryId anAltType) : base(anAltType)
        {
            
        }
        
        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityAlternateCategoryId; }
            set { _entityAlternateCategoryId = (AlternateCategoryId)value; }
        }

        protected override void EntityClear()
        {
            _entityAlternateCategoryId = new AlternateCategoryId();
            _entityAlternateCategoryId.Id = -1;
            _entityAlternateCategoryId.SystemCreateDate = new DateTime();
            _entityAlternateCategoryId.SystemEditDate = new DateTime();
            _entityAlternateCategoryId.MemberId = -1;
            _entityAlternateCategoryId.CategoryId = null;
            _entityAlternateCategoryId.AlternateId = -1;
            _entityAlternateCategoryId.DisplayName = string.Empty;
            _entityAlternateCategoryId.OverrideFlag = false;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static protected internal Converter<AlternateCategoryId, ProviderAlternateCategoryId> _converterEntityToProvider = new Converter<AlternateCategoryId, ProviderAlternateCategoryId>(_EntityToProvider);
        static protected internal ProviderAlternateCategoryId _EntityToProvider(AlternateCategoryId alternateIdEntiy)
        {
            return new ProviderAlternateCategoryId(alternateIdEntiy);
        }

        static public List<ProviderAlternateCategoryId> LoadBy(long memberId)
        {
            List<ProviderAlternateCategoryId> returnList = null;
            if (memberId > -1)
            {
                returnList = DbCtx.Instance.AlternateCategoryIds
                                                 .Where(anAltId => anAltId.MemberId == memberId)
                                                 .ToList()
                                                 .ConvertAll(_converterEntityToProvider);
            }
            else
            {
                returnList = new List<ProviderAlternateCategoryId>();
            }

            return returnList;
        }

        static public long? BestMatch(string displayName)
        {
            long? categoryId = null;
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                categoryId = DbCtx.Instance.AlternateCategoryIds
                                            .Where(anAltId => anAltId.DisplayName.ToLower().Contains(displayName.ToLower()))
                                            .GroupBy( anAltId => anAltId.CategoryId )
                                            .OrderByDescending( groups => groups.Count() )
                                            .Select(group => group.Key)
                                            .FirstOrDefault();
            }

            return categoryId;
        }
    }
}
