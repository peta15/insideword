using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderConversationScore : Provider
    {
        protected ConversationScore _entityConversationScore;

        public ProviderConversationScore() : base() { }
        public ProviderConversationScore(long id) : base(id) { }

        public double Score
        {
            get { return _entityConversationScore.Score; }
            set { _entityConversationScore.Score = value; }
        }

        public long ConversationId
        {
            get { return _entityConversationScore.ConversationId; }
            set { _entityConversationScore.ConversationId = value; }
        }

        override public bool Load(long id)
        {
            ConversationScore entityConversationScore = DbCtx.Instance.ConversationScores
                                                            .Where(score => score.Id == id)
                                                            .Single();
            return Load(entityConversationScore);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityConversationScore.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityConversationScore.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityConversationScore.SystemEditDate);
            sb.Append("\n\tConversationId =\t" + _entityConversationScore.ConversationId);
            sb.Append("\n\tScore =\t" + _entityConversationScore.Score);
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
            ProviderConversationScore aConversationScore = (ProviderConversationScore)untyped;
            _entityConversationScore.Score = aConversationScore._entityConversationScore.Score;
            _entityConversationScore.ConversationId = aConversationScore._entityConversationScore.ConversationId;
            _entityObject = _entityConversationScore;
            return true;
        }
        */

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderConversationScore(ConversationScore aConversationScore) : base(aConversationScore) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityConversationScore; }
            set { _entityConversationScore = (ConversationScore)value; }
        }

        override protected void EntityClear()
        {
            _entityConversationScore = new ConversationScore();
            _entityConversationScore.Id = -1;
            _entityConversationScore.SystemCreateDate = new DateTime();
            _entityConversationScore.SystemEditDate = new DateTime();
            _entityConversationScore.ConversationId = -1;
            _entityConversationScore.Score = 0;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<ConversationScore, ProviderConversationScore> _converterEntityToProvider = new Converter<ConversationScore, ProviderConversationScore>(_EntityToProvider);
        static protected ProviderConversationScore _EntityToProvider(ConversationScore ConversationScoreEntity)
        {
            return new ProviderConversationScore(ConversationScoreEntity);
        }

        static public List<ProviderConversationScore> LoadAll()
        {
            return DbCtx.Instance.ConversationScores.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderConversationScore> LoadByArticleId(long articleId)
        {
            return DbCtx.Instance.ConversationScores.Where(score => score.Conversation.ArticleId == articleId)
                                                        .ToList()
                                                        .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.ConversationScores.Any(aScore => aScore.Id == id);
        }
    }
}