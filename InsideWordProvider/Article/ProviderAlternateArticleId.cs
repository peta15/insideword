using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Net.Mail;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderAlternateArticleId : Provider
    {
        protected AlternateArticleId _entityAlternateArticleId;

        public ProviderAlternateArticleId() : base() { }
        public ProviderAlternateArticleId(long id) : base(id) { }

        public long MemberId
        {
            get { return _entityAlternateArticleId.MemberId; }
            set { _entityAlternateArticleId.MemberId = value; }
        }

        public long ArticleId
        {
            get { return _entityAlternateArticleId.ArticleId; }
            set { _entityAlternateArticleId.ArticleId = value; }
        }

        public string AlternateId
        {
            get { return _entityAlternateArticleId.AlternateId; }
            set { _entityAlternateArticleId.AlternateId = value; }
        }

        public override bool Load(long id)
        {
            AlternateArticleId altId = DbCtx.Instance.AlternateArticleIds
                                            .Where(anAltId => anAltId.Id == id)
                                            .SingleOrDefault();
            return Load(altId);
        }

        public bool Load(string alternateId, long memberId)
        {
            AlternateArticleId altId = DbCtx.Instance.AlternateArticleIds
                                               .Where(anAltId => anAltId.AlternateId == alternateId &&
                                                                 anAltId.MemberId == memberId)
                                               .SingleOrDefault();
            return Load(altId);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tAlternateArticleIdId =\t" + _entityAlternateArticleId.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityAlternateArticleId.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityAlternateArticleId.SystemEditDate);
            sb.Append("\n\tMemberId =\t" + _entityAlternateArticleId.MemberId);
            sb.Append("\n\tArticleId =\t" + _entityAlternateArticleId.ArticleId);
            sb.Append("\n\tAlternateId =\t" + _entityAlternateArticleId.AlternateId);
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
            _entityAlternateArticleId.MemberId = anAlternateMemberId._entityAlternateArticleId.MemberId;
            _entityAlternateArticleId.AlternateType = anAlternateMemberId._entityAlternateArticleId.AlternateType;
            _entityAlternateArticleId.AlternateId = anAlternateMemberId._entityAlternateArticleId.AlternateId;
            _entityAlternateArticleId.IsValidated = anAlternateMemberId._entityAlternateArticleId.IsValidated;
            _entityAlternateArticleId.IsNonce = anAlternateMemberId._entityAlternateArticleId.IsNonce;
            _entityAlternateArticleId.ExpiryDate = anAlternateMemberId._entityAlternateArticleId.ExpiryDate;
            _entityAlternateArticleId.EditDate = anAlternateMemberId._entityAlternateArticleId.EditDate;
            _entityAlternateArticleId.CreateDate = anAlternateMemberId._entityAlternateArticleId.CreateDate;
            _entityAlternateArticleId.UsePassword = anAlternateMemberId._entityAlternateArticleId.UsePassword;
            _entityAlternateArticleId.IsHidden = anAlternateMemberId._entityAlternateArticleId.IsHidden;
            _entityObject = _entityAlternateArticleId;
            return true;
        }
        */

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderAlternateArticleId(AlternateArticleId anAltType) : base(anAltType)
        {
            
        }
        
        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityAlternateArticleId; }
            set { _entityAlternateArticleId = (AlternateArticleId)value; }
        }

        protected override void EntityClear()
        {
            _entityAlternateArticleId = new AlternateArticleId();
            _entityAlternateArticleId.Id = -1;
            _entityAlternateArticleId.SystemCreateDate = new DateTime();
            _entityAlternateArticleId.SystemEditDate = new DateTime();
            _entityAlternateArticleId.MemberId = -1;
            _entityAlternateArticleId.ArticleId = -1;
            _entityAlternateArticleId.AlternateId = string.Empty;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public long? Exists(string alternateId, long? memberId)
        {
            long? returnValue = null;
            if (!string.IsNullOrEmpty(alternateId) && memberId>-1)
            {
                returnValue = DbCtx.Instance.AlternateArticleIds
                                                 .Where(anAltId => anAltId.AlternateId == alternateId &&
                                                                   (memberId == null || anAltId.MemberId == memberId)
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


        static public List<long> FindOwnerList(string alternateId)
        {
            List<long> returnValue = new List<long>();
            if (!string.IsNullOrEmpty(alternateId))
            {
                returnValue = DbCtx.Instance.AlternateArticleIds
                                                 .Where(anAltId => anAltId.AlternateId == alternateId)
                                                 .Select(anAltId => anAltId.MemberId)
                                                 .ToList();
            }
            return returnValue;
        }
    }
}