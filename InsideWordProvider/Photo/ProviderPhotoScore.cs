using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderPhotoScore : Provider
    {
        protected PhotoScore _entityPhotoScore;

        public ProviderPhotoScore() : base() { }
        public ProviderPhotoScore(long id) : base(id) { }

        public double Score
        {
            get { return _entityPhotoScore.Score; }
            set { _entityPhotoScore.Score = value; }
        }

        public long PhotoId
        {
            get { return _entityPhotoScore.PhotoId; }
            set { _entityPhotoScore.PhotoId = value; }
        }

        public override bool Load(long id)
        {
            PhotoScore entityPhotoScore = DbCtx.Instance.PhotoScores
                                                    .Where(score => score.Id == id)
                                                    .Single();
            return Load(_entityPhotoScore);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityPhotoScore.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityPhotoScore.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityPhotoScore.SystemEditDate);
            sb.Append("\n\tPhotoId =\t" + _entityPhotoScore.PhotoId);
            sb.Append("\n\tScore =\t" + _entityPhotoScore.Score);
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
            ProviderPhotoScore aPhotoScore = (ProviderPhotoScore)untyped;
            _entityPhotoScore.Score = aPhotoScore._entityPhotoScore.Score;
            _entityPhotoScore.PhotoId = aPhotoScore._entityPhotoScore.PhotoId;
            _entityObject = _entityPhotoScore;
            return true;
        }
        */

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderPhotoScore(PhotoScore aPhotoScore) : base(aPhotoScore)
        {

        }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityPhotoScore; }
            set { _entityPhotoScore = (PhotoScore)value; }
        }

        override protected void EntityClear()
        {
            _entityPhotoScore = new PhotoScore();
            _entityPhotoScore.Id = -1;
            _entityPhotoScore.SystemCreateDate = new DateTime();
            _entityPhotoScore.SystemEditDate = new DateTime();
            _entityPhotoScore.PhotoId = -1;
            _entityPhotoScore.Score = 0;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<PhotoScore, ProviderPhotoScore> _converterEntityToProvider = new Converter<PhotoScore, ProviderPhotoScore>(_EntityToProvider);
        static protected ProviderPhotoScore _EntityToProvider(PhotoScore PhotoScoreEntity)
        {
            return new ProviderPhotoScore(PhotoScoreEntity);
        }

        static public List<ProviderPhotoScore> LoadAll()
        {
            return DbCtx.Instance.PhotoScores.ToList().ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.PhotoScores.Any(aScore => aScore.Id == id);
        }
    }
}