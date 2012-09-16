using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderPhotoVote : Provider
    {
        protected PhotoVote _entityPhotoVote;

        public ProviderPhotoVote() : base() { }
        public ProviderPhotoVote(long id) : base(id) { }

        public bool IsFlag
        {
            get { return _entityPhotoVote.IsFlag; }
            set
            {
                _entityPhotoVote.IsFlag = value;
                IsDownVote = value;
            }
        }

        public bool IsUpVote
        {
            get { return _entityPhotoVote.VoteWeight > 0; }
            set { _entityPhotoVote.VoteWeight = value ? 1 : -1; }
        }

        public bool IsDownVote
        {
            get { return _entityPhotoVote.VoteWeight < 0; }
            set { _entityPhotoVote.VoteWeight = value ? -1 : 1; }
        }

        public bool IsShadowVote
        {
            get { return _entityPhotoVote.IsShadowVote; }
            set { _entityPhotoVote.IsShadowVote = value; }
        }

        public string Text
        {
            get { return _entityPhotoVote.Text; }
            set { _entityPhotoVote.Text = value; }
        }

        public bool IsHidden
        {
            get { return _entityPhotoVote.IsHidden; }
            set { _entityPhotoVote.IsHidden = value; }
        }

        public long PhotoId
        {
            get { return _entityPhotoVote.PhotoId; }
            set { _entityPhotoVote.PhotoId = value; }
        }

        public long? MemberId
        {
            get { return _entityPhotoVote.MemberId; }
            set { _entityPhotoVote.MemberId = value; }
        }

        override public bool Load(long id)
        {
            PhotoVote entityPhotoVote = DbCtx.Instance.PhotoVotes
                                                    .Where(vote => vote.Id == id)
                                                    .Single();
            return Load(entityPhotoVote);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityPhotoVote.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityPhotoVote.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityPhotoVote.SystemEditDate);
            sb.Append("\n\tIsFlag =\t" + _entityPhotoVote.IsFlag);
            sb.Append("\n\tVoteWeight =\t" + _entityPhotoVote.VoteWeight);
            sb.Append("\n\tText =\t" + _entityPhotoVote.Text);
            sb.Append("\n\tIsHidden =\t" + _entityPhotoVote.IsHidden);
            sb.Append("\n\tPhotoId =\t" + _entityPhotoVote.PhotoId);
            sb.Append("\n\tMemberId =\t" + _entityPhotoVote.MemberId);
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
            ProviderPhotoVote aPhotoVote = (ProviderPhotoVote)untyped;
            _entityPhotoVote.IsHidden = aPhotoVote._entityPhotoVote.IsHidden;
            _entityPhotoVote.IsFlag = aPhotoVote._entityPhotoVote.IsFlag;
            _entityPhotoVote.VoteWeight = aPhotoVote._entityPhotoVote.VoteWeight;
            _entityPhotoVote.Text = aPhotoVote._entityPhotoVote.Text;
            _entityPhotoVote.IsHidden = aPhotoVote._entityPhotoVote.IsHidden;
            _entityPhotoVote.PhotoId = aPhotoVote._entityPhotoVote.PhotoId;
            _entityPhotoVote.MemberId = aPhotoVote._entityPhotoVote.MemberId;
            _entityObject = _entityPhotoVote;
            return true;
        }
        */

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderPhotoVote(PhotoVote aPhotoVote) : base(aPhotoVote) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityPhotoVote; }
            set { _entityPhotoVote = (PhotoVote)value; }
        }

        protected override void EntityClear()
        {
            _entityPhotoVote = new PhotoVote();
            _entityPhotoVote.Id = -1;
            _entityPhotoVote.SystemCreateDate = new DateTime();
            _entityPhotoVote.SystemEditDate = new DateTime();
            _entityPhotoVote.IsFlag = false;
            _entityPhotoVote.VoteWeight = 0;
            _entityPhotoVote.Text = String.Empty;
            _entityPhotoVote.IsHidden = false;
            _entityPhotoVote.PhotoId = -1;
            _entityPhotoVote.MemberId = null;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<PhotoVote, ProviderPhotoVote> _converterEntityToProvider = new Converter<PhotoVote, ProviderPhotoVote>(_EntityToProvider);
        static protected ProviderPhotoVote _EntityToProvider(PhotoVote photoVoteEntity)
        {
            return new ProviderPhotoVote(photoVoteEntity);
        }

        static public List<ProviderPhotoVote> LoadAll()
        {
            return DbCtx.Instance.PhotoVotes.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderPhotoVote> LoadByPhotoId(long photoId)
        {
            return DbCtx.Instance.PhotoVotes.Where(vote => vote.PhotoId == photoId).ToList().ConvertAll(_converterEntityToProvider);
        }

        static public bool HasVoted(long photoId, long memberId)
        {
            bool result = false;
            if (memberId >= 0 && photoId >= 0)
            {
                result = 0 < DbCtx.Instance.PhotoVotes
                                                .Where(vote => vote.PhotoId == photoId && vote.MemberId == memberId)
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
            return id >= 0 && DbCtx.Instance.PhotoVotes.Any(aVote => aVote.Id == id);
        }
    }
}