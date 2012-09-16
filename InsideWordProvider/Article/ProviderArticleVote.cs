using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderArticleVote : Provider
    {
        protected ArticleVote _entityArticleVote;

        public ProviderArticleVote() : base() { }
        public ProviderArticleVote(long id) : base(id) { }

        public bool IsFlag
        {
            get { return _entityArticleVote.IsFlag; }
            set
            {
                _entityArticleVote.IsFlag = value;
                IsDownVote = value;
            }
        }

        public bool IsUpVote
        {
            get { return _entityArticleVote.VoteWeight > 0; }
            set { _entityArticleVote.VoteWeight = value ? 1 : -1; }
        }

        public bool IsDownVote
        {
            get { return _entityArticleVote.VoteWeight < 0; }
            set { _entityArticleVote.VoteWeight = value ? -1 : 1; }
        }

        public bool IsShadowVote
        {
            get { return _entityArticleVote.IsShadowVote; }
            set { _entityArticleVote.IsShadowVote = value; }
        }

        public string Text
        {
            get { return _entityArticleVote.Text; }
            set { _entityArticleVote.Text = value; }
        }

        public bool IsHidden
        {
            get { return _entityArticleVote.IsHidden; }
            set { _entityArticleVote.IsHidden = value; }
        }

        public long ArticleId
        {
            get { return _entityArticleVote.ArticleId; }
            set { _entityArticleVote.ArticleId = value; }
        }

        public DateTime ArticleSystemCreateDate
        {
            get { return _entityArticleVote.Article.SystemCreateDate; }
        }

        public long? MemberId
        {
            get { return _entityArticleVote.MemberId; }
            set { _entityArticleVote.MemberId = value; }
        }

        override public bool Load(long id)
        {
            ArticleVote entityArticleVote = DbCtx.Instance.ArticleVotes
                                                    .Where(vote => vote.Id == id)
                                                    .Single();
            return Load(entityArticleVote);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityArticleVote.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityArticleVote.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityArticleVote.SystemEditDate);
            sb.Append("\n\tIsFlag =\t" + _entityArticleVote.IsFlag);
            sb.Append("\n\tVoteWeight =\t" + _entityArticleVote.VoteWeight);
            sb.Append("\n\tText =\t" + _entityArticleVote.Text);
            sb.Append("\n\tIsHidden =\t" + _entityArticleVote.IsHidden);
            sb.Append("\n\tMemberId =\t" + _entityArticleVote.MemberId);
            sb.Append("\n");

            return sb.ToString();
        }

        /* TODO: revisit all copy functions
        override public bool Copy(Provider untyped)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            ProviderArticleVote anArticleVote = (ProviderArticleVote)untyped;
            _entityArticleVote.IsHidden = anArticleVote._entityArticleVote.IsHidden;
            _entityArticleVote.IsFlag = anArticleVote._entityArticleVote.IsFlag;
            _entityArticleVote.VoteWeight = anArticleVote._entityArticleVote.VoteWeight;
            _entityArticleVote.Text = anArticleVote._entityArticleVote.Text;
            _entityArticleVote.IsHidden = anArticleVote._entityArticleVote.IsHidden;
            _entityArticleVote.ArticleId = anArticleVote._entityArticleVote.ArticleId;
            _entityArticleVote.MemberId = anArticleVote._entityArticleVote.MemberId;
            _entityObject = _entityArticleVote;
            return true;
        }*/

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderArticleVote(ArticleVote anArticleVote) : base(anArticleVote) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityArticleVote; }
            set { _entityArticleVote = (ArticleVote)value; }
        }

        override protected void EntityClear()
        {
            _entityArticleVote = new ArticleVote();
            _entityArticleVote.Id = -1;
            _entityArticleVote.SystemCreateDate = new DateTime();
            _entityArticleVote.SystemEditDate = new DateTime();
            _entityArticleVote.IsFlag = false;
            _entityArticleVote.VoteWeight = 0;
            _entityArticleVote.Text = String.Empty;
            _entityArticleVote.IsHidden = false;
            _entityArticleVote.ArticleId = -1;
            _entityArticleVote.MemberId = null;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<ArticleVote, ProviderArticleVote> _converterEntityToProvider = new Converter<ArticleVote, ProviderArticleVote>(_EntityToProvider);
        static protected ProviderArticleVote _EntityToProvider(ArticleVote photoVoteEntity)
        {
            return new ProviderArticleVote(photoVoteEntity);
        }

        static public int CountFlagged()
        {
            return DbCtx.Instance.ArticleVotes
                                      .Where(aVote => aVote.IsFlag)
                                      .Count();
        }

        static public List<ProviderArticleVote> LoadAll()
        {
            return DbCtx.Instance.ArticleVotes
                                      .ToList()
                                      .ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderArticleVote> LoadBySystemEditDateTime(DateTime aDateTime)
        {
            return DbCtx.Instance.ArticleVotes
                                      .Include("Article")
                                      .Where(vote => vote.SystemEditDate > aDateTime)
                                      .OrderBy(score => score.ArticleId)
                                      .ToList()
                                      .ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderArticleVote> LoadByArticleIdDateTime(long articleId, DateTime aDateTime)
        {
            return DbCtx.Instance.ArticleVotes
                                      .Where(vote => vote.ArticleId == articleId && vote.SystemEditDate > aDateTime)
                                      .OrderBy(score => score.ArticleId)
                                      .ToList()
                                      .ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderArticleVote> LoadByArticleId(long articleId)
        {
            return DbCtx.Instance.ArticleVotes.Where(vote => vote.ArticleId == articleId).ToList().ConvertAll(_converterEntityToProvider);
        }

        static public long CountUpVotesByArticleId(long articleId)
        {
            return DbCtx.Instance.ArticleVotes.Where(vote => vote.ArticleId == articleId && vote.VoteWeight > 0).Count();
        }

        static public long CountDownVotesByArticleId(long articleId)
        {
            return DbCtx.Instance.ArticleVotes.Where(vote => vote.ArticleId == articleId && vote.VoteWeight < 0).Count();
        }

        static public bool HasVoted(long articleId, long memberId)
        {
            return 0 < DbCtx.Instance.ArticleVotes
                                        .Where(vote => vote.ArticleId == articleId && vote.MemberId == memberId)
                                        .Count();
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.ArticleVotes.Where(aVote => aVote.Id == id).Count() > 0;
        }
    }
}