using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderComment : Provider
    {
        protected Comment _entityComment;

        public ProviderComment() : base() { }
        public ProviderComment(long id) : base(id) { }

        public DateTime CreateDate
        {
            get { return _entityComment.CreateDate; }
            set { _entityComment.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityComment.EditDate; }
            set { _entityComment.EditDate = value; }
        }

        public bool IsFlagged
        {
            get { return _entityComment.IsFlagged > 0; }
        }

        public int CountFlags
        {
            get { return _entityComment.IsFlagged; }
        }

        public int Flag()
        {
            return (_entityComment.IsFlagged++);
        }

        public string Text
        {
            get { return _entityComment.Text; }
            set { _entityComment.Text = value; }
        }

        public bool IsHidden
        {
            get { return _entityComment.IsHidden; }
            set { _entityComment.IsHidden = value; }
        }

        public long ConversationId
        {
            get { return _entityComment.ConversationId; }
            set { _entityComment.ConversationId = value; }
        }

        public long? MemberId
        {
            get { return _entityComment.MemberId; }
            set { _entityComment.MemberId = value; }
        }

        public bool IgnoreFlags
        {
            get { return _entityComment.IgnoreFlags; }
            set { _entityComment.IgnoreFlags = value; }
        }

        public ProviderMember Author
        {
            get
            {
                ProviderMember author = null;
                if (_entityComment.MemberId.HasValue)
                {
                    author = new ProviderMember(_entityComment.Member);
                }
                return author;
            }
        }

        override public bool Load(long id)
        {
            Comment entityComment = DbCtx.Instance.Comments
                                                .Where(comment => comment.Id == id)
                                                .First();

            return Load(entityComment);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityComment.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityComment.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityComment.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityComment.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityComment.EditDate);
            sb.Append("\n\tIsFlagged =\t" + _entityComment.IsFlagged);
            sb.Append("\n\tText =\t" + _entityComment.Text);
            sb.Append("\n\tIsHidden =\t" + _entityComment.IsHidden);
            sb.Append("\n\tMemberId =\t" + _entityComment.MemberId);
            sb.Append("\n\tConversationId =\t" + _entityComment.ConversationId);
            sb.Append("\n\tIgnoreFlags =\t" + _entityComment.IgnoreFlags);
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
            ProviderComment aComment = (ProviderComment)untyped;
            _entityComment.CreateDate = aComment._entityComment.CreateDate;
            _entityComment.EditDate = aComment._entityComment.EditDate;
            _entityComment.IsHidden = aComment._entityComment.IsHidden;
            _entityComment.IsFlagged = aComment._entityComment.IsFlagged;
            _entityComment.Text = aComment._entityComment.Text;
            _entityComment.IsHidden = aComment._entityComment.IsHidden;
            _entityComment.ConversationId = aComment._entityComment.ConversationId;
            _entityComment.MemberId = aComment._entityComment.MemberId;
            _entityComment.IgnoreFlags = aComment._entityComment.IgnoreFlags;
            _entityObject = _entityComment;
            return true;
        }*/

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderComment(Comment aComment) : base(aComment) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityComment; }
            set { _entityComment = (Comment)value; }
        }

        override protected void EntityClear()
        {
            _entityComment = new Comment();
            _entityComment.Id = -1;
            _entityComment.SystemCreateDate = new DateTime();
            _entityComment.SystemEditDate = new DateTime();
            _entityComment.CreateDate = new DateTime();
            _entityComment.EditDate = new DateTime();
            _entityComment.IsFlagged = 0;
            _entityComment.Text = String.Empty;
            _entityComment.IsHidden = false;
            _entityComment.ConversationId = -1;
            _entityComment.IgnoreFlags = false;
            _entityComment.MemberId = null;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }
        static public int TextSize { get { return 512; } }

        static protected Converter<Comment, ProviderComment> _converterEntityToProvider = new Converter<Comment, ProviderComment>(_EntityToProvider);
        static protected ProviderComment _EntityToProvider(Comment commentEntity)
        {
            return new ProviderComment(commentEntity);
        }

        static public List<ProviderComment> LoadAll()
        {
            return DbCtx.Instance.Comments.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderComment> LoadByConversationId(long conversationId)
        {
            return DbCtx.Instance.Comments.Where(comment => comment.ConversationId == conversationId).ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderComment> Load(IProviderCommentFilter filter)
        {
            int m = (filter.Page - 1);
            if (m < 0) m = 0;
            int skip = filter.Rows * m;
            IQueryable<Comment> query = DbCtx.Instance.Comments;
            query = CommentFilter(filter, query);
            query = CommentSort(filter, query);
            return query.Skip(skip)
                        .Take(filter.Rows)
                        .ToList()
                        .ConvertAll(_converterEntityToProvider);
        }

        static public int Count(IProviderCommentFilter filter)
        {
            IQueryable<Comment> query = DbCtx.Instance.Comments;
            query = CommentFilter(filter, query);
            return query.Count();
        }

        static protected IQueryable<Comment> CommentSort(IProviderCommentFilter filter, IQueryable<Comment> query)
        {
            // Sort grid data.  Sord is sort order, Sidx is the index to sort on
            if (filter.Sord == "desc")
            {
                // Sort by date if we don't know what this was
                query = query.OrderByDescending(aComment => aComment.EditDate);
            }
            else if (filter.Sord == "asc")
            {
                // Sort by date if we don't know what this was
                query = query.OrderBy(aComment => aComment.EditDate);
            }

            return query;
        }

        static protected IQueryable<Comment> CommentFilter(IProviderCommentFilter filter, IQueryable<Comment> query)
        {
            return query.Where(aComment => aComment.IsFlagged >= filter.CountFlags &&
                                            (filter.Id == null || aComment.Id == filter.Id) &&
                                            (filter.IsHidden == null || aComment.IsHidden == filter.IsHidden) &&
                                            (filter.IgnoreFlags == null || aComment.IgnoreFlags == filter.IgnoreFlags)
                            );
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.Comments.Where(aComment => aComment.Id == id).Count() > 0;
        }
    }
}