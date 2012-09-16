using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderArticleScore : Provider
    {
        protected ArticleScore _entityArticleScore;

        public ProviderArticleScore() : base() { }
        public ProviderArticleScore(long id) : base(id) { }

        public double Score
        {
            get { return _entityArticleScore.Score; }
            set { _entityArticleScore.Score = value; }
        }

        public ScoreTypeEnum ScoreType
        {
            get { return (ScoreTypeEnum)_entityArticleScore.ScoreType; }
            set { _entityArticleScore.ScoreType = (int)value; }
        }

        public long ArticleId
        {
            get { return _entityArticleScore.ArticleId; }
            set { _entityArticleScore.ArticleId = value; }
        }

        public DateTime ArticleSystemCreateDate
        {
            get { return _entityArticleScore.Article.SystemCreateDate; }
        }

        override public bool Load(long id)
        {
            ArticleScore entityArticleScore = DbCtx.Instance.ArticleScores
                                                     .Where(score => score.Id == id)
                                                     .FirstOrDefault();
            return Load(entityArticleScore);
        }

        public bool LoadBy(long articleId, ScoreTypeEnum aType)
        {
            ArticleScore entityArticleScore = DbCtx.Instance.ArticleScores
                                                     .Where(score => score.ArticleId == articleId && score.ScoreType == (int)aType)
                                                     .FirstOrDefault();
            return Load(entityArticleScore);
        }

        public bool LoadOrCreate(long articleId, ScoreTypeEnum aType, double startScore = 0.0, bool save=false)
        {
            bool returnValue = LoadBy(articleId, aType);
            if (!returnValue)
            {
                this.ArticleId = articleId;
                this.Score = startScore;
                this.ScoreType = aType;
                if (save)
                {
                    this.Save();
                }
            }

            return returnValue;
        }

        public override void Save()
        {
            if (_entityArticleScore.ScoreType == (int)ScoreTypeEnum.None)
            {
                throw new Exception("ScoreType must be set to a value other than None.");
            }
            base.Save();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityArticleScore.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityArticleScore.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityArticleScore.SystemEditDate);
            sb.Append("\n\tArticleId =\t" + _entityArticleScore.ArticleId);
            sb.Append("\n\tScore =\t" + _entityArticleScore.Score);
            sb.Append("\n\tScoreType =\t" + _entityArticleScore.ScoreType);
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
            ProviderArticleScore aArticleScore = (ProviderArticleScore)untyped;
            _entityArticleScore.Score = aArticleScore._entityArticleScore.Score;
            _entityArticleScore.ArticleId = aArticleScore._entityArticleScore.ArticleId;
            _entityObject = _entityArticleScore;
            return true;
        }*/

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderArticleScore(ArticleScore anArticleScore) : base(anArticleScore) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityArticleScore; }
            set { _entityArticleScore = (ArticleScore)value; }
        }

        protected override void EntityClear()
        {
            _entityArticleScore = new ArticleScore();
            _entityArticleScore.Id = -1;
            _entityArticleScore.SystemCreateDate = new DateTime();
            _entityArticleScore.SystemEditDate = new DateTime();
            _entityArticleScore.ArticleId = -1;
            _entityArticleScore.Score = 0;
            _entityArticleScore.ScoreType = (int)ScoreTypeEnum.None;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<ArticleScore, ProviderArticleScore> _converterEntityToProvider = new Converter<ArticleScore, ProviderArticleScore>(_EntityToProvider);
        static protected ProviderArticleScore _EntityToProvider(ArticleScore ArticleScoreEntity)
        {
            return new ProviderArticleScore(ArticleScoreEntity);
        }

        static public List<ProviderArticleScore> LoadAll()
        {
            return DbCtx.Instance.ArticleScores.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderArticleScore> LoadBy(ScoreTypeEnum aType)
        {
            return DbCtx.Instance.ArticleScores.Include("Article")
                                            .Where(score => score.ScoreType == (int)aType)
                                            .ToList()
                                            .ConvertAll(_converterEntityToProvider);
        }

        static public Dictionary<long, int> GetRankDict(ScoreTypeEnum selectType)
        {
            return DbCtx.Instance.ArticleScores
                                .Where(score => score.ScoreType == (int)selectType)
                                .OrderByDescending(score => score.Score)
                                .Select(score => new {score.ArticleId})
                                .AsEnumerable()
                                .Select((articleId, index) => new { articleId.ArticleId, Index = index + 1 })
                                .ToDictionary(aPair => aPair.ArticleId, aPair => aPair.Index);
        }

        static public DateTime GetMaxSystemEditDate()
        {
            DateTime minDateTime = DateTime.MinValue;
            if (DbCtx.Instance.ArticleScores.Count() > 0)
            {
                minDateTime = DbCtx.Instance.ArticleScores.Max(score => score.SystemEditDate);
            }
            return minDateTime;
        }

        static public double GetScoreByArticleId(long articleId, ScoreTypeEnum selectType)
        {
            int intSelectType = (int)selectType;
            return DbCtx.Instance.ArticleScores.Where(score => score.ArticleId == articleId && score.ScoreType == intSelectType)
                                                    .Select(score => score.Score)
                                                    .FirstOrDefault();
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.ArticleScores.Where(aScore => aScore.Id == id).Count() > 0;
        }

        /// <summary>
        /// Indicates if an object with a given Article Id exists
        /// </summary>
        /// <param name="id">Article Id of the object whose existence we wish to check</param>
        /// <returns>The object Id that has the matching Article Id</returns>
        static public long? ExistsByArticleId(long articleId, ScoreTypeEnum aType)
        {
            long? returnValue = null;
            if (articleId > -1)
            {
                long scoreId = DbCtx.Instance.ArticleScores
                                                  .Where(aScore => aScore.ArticleId == articleId && aScore.ScoreType == (int)aType)
                                                  .Select(aScore => aScore.Id)
                                                  .DefaultIfEmpty(-1)
                                                  .Single();
                if (scoreId != -1)
                {
                    returnValue = scoreId;   
                }
            }
            return returnValue;
        }

        public enum ScoreTypeEnum
        {
            None = -1,
            SparrowRank = 0,
            PublicVoteSum = 1,
            CommentSum = 2
        }
    }
}