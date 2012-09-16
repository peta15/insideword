using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderConversationVote : Provider
    {
        protected ConversationVote _entityConversationVote;

        public ProviderConversationVote() : base() { }
        public ProviderConversationVote(long id) : base(id) { }

        public bool IsFlag
        {
            get { return _entityConversationVote.IsFlag; }
            set
            {
                _entityConversationVote.IsFlag = value;
                IsDownVote = value;
            }
        }

        public bool IsUpVote
        {
            get { return _entityConversationVote.VoteWeight > 0; }
            set { _entityConversationVote.VoteWeight = value ? 1 : -1; }
        }

        public bool IsDownVote
        {
            get { return _entityConversationVote.VoteWeight < 0; }
            set { _entityConversationVote.VoteWeight = value ? -1 : 1; }
        }

        public bool IsShadowVote
        {
            get { return _entityConversationVote.IsShadowVote; }
            set { _entityConversationVote.IsShadowVote = value; }
        }

        public string Text
        {
            get { return _entityConversationVote.Text; }
            set { _entityConversationVote.Text = value; }
        }

        public bool IsHidden
        {
            get { return _entityConversationVote.IsHidden; }
            set { _entityConversationVote.IsHidden = value; }
        }

        public long ConversationId
        {
            get { return _entityConversationVote.ConversationId; }
            set { _entityConversationVote.ConversationId = value; }
        }

        public long? MemberId
        {
            get { return _entityConversationVote.MemberId; }
            set { _entityConversationVote.MemberId = value; }
        }

        override public bool Load(long id)
        {
            ConversationVote entityConversationVote = DbCtx.Instance.ConversationVotes
                                                            .Where(vote => vote.Id == id)
                                                            .First();

            return Load(entityConversationVote);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityConversationVote.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityConversationVote.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityConversationVote.SystemEditDate);
            sb.Append("\n\tIsFlag =\t" + _entityConversationVote.IsFlag);
            sb.Append("\n\tVoteWeight =\t" + _entityConversationVote.VoteWeight);
            sb.Append("\n\tText =\t" + _entityConversationVote.Text);
            sb.Append("\n\tIsHidden =\t" + _entityConversationVote.IsHidden);
            sb.Append("\n\tConversationId =\t" + _entityConversationVote.ConversationId);
            sb.Append("\n\tMemberId =\t" + _entityConversationVote.MemberId);
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
            ProviderConversationVote anConversationVote = (ProviderConversationVote)untyped;
            _entityConversationVote.IsHidden = anConversationVote._entityConversationVote.IsHidden;
            _entityConversationVote.IsFlag = anConversationVote._entityConversationVote.IsFlag;
            _entityConversationVote.VoteWeight = anConversationVote._entityConversationVote.VoteWeight;
            _entityConversationVote.Text = anConversationVote._entityConversationVote.Text;
            _entityConversationVote.IsHidden = anConversationVote._entityConversationVote.IsHidden;
            _entityConversationVote.ConversationId = anConversationVote._entityConversationVote.ConversationId;
            _entityConversationVote.MemberId = anConversationVote._entityConversationVote.MemberId;
            _entityObject = _entityConversationVote;
            return true;
        }*/

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderConversationVote(ConversationVote aConversationVote) : base(aConversationVote) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityConversationVote; }
            set { _entityConversationVote = (ConversationVote)value; }
        }

        override protected void EntityClear()
        {
            _entityConversationVote = new ConversationVote();
            _entityConversationVote.Id = -1;
            _entityConversationVote.SystemCreateDate = new DateTime();
            _entityConversationVote.SystemEditDate = new DateTime();
            _entityConversationVote.IsFlag = false;
            _entityConversationVote.VoteWeight = 0;
            _entityConversationVote.Text = String.Empty;
            _entityConversationVote.IsHidden = false;
            _entityConversationVote.ConversationId = -1;
            _entityConversationVote.MemberId = null;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<ConversationVote, ProviderConversationVote> _converterEntityToProvider = new Converter<ConversationVote, ProviderConversationVote>(_EntityToProvider);
        static protected ProviderConversationVote _EntityToProvider(ConversationVote conversationVoteEntity)
        {
            return new ProviderConversationVote(conversationVoteEntity);
        }

        static public List<ProviderConversationVote> LoadAll()
        {
            return DbCtx.Instance.ConversationVotes.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderConversationVote> LoadByConversationId(long conversationId)
        {
            return DbCtx.Instance.ConversationVotes.Where(vote => vote.ConversationId == conversationId).ToList().ConvertAll(_converterEntityToProvider);
        }

        static public bool HasVoted(long conversationId, long memberId)
        {
            bool result = false;
            if (memberId >= 0 && conversationId >= 0)
            {
                result = 0 < DbCtx.Instance.ConversationVotes
                                                .Where(vote => vote.ConversationId == conversationId && vote.MemberId == memberId)
                                                .Count();
            }

            return result;
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.ConversationVotes.Where(aVote => aVote.Id == id).Count() > 0;
        }
    }
}