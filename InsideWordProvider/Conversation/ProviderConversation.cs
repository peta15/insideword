using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderConversation : Provider
    {
        protected Conversation _entityConversation;

        public ProviderConversation() : base() { }
        public ProviderConversation(long id) : base(id) { }

        public DateTime CreateDate
        {
            get { return _entityConversation.CreateDate; }
            set { _entityConversation.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityConversation.EditDate; }
            set { _entityConversation.EditDate = value; }
        }

        public string AuthorName
        {
            get
            {
                string value = null;
                if (_entityConversation.MemberId.HasValue)
                {
                    List<ProviderUserName> userNameList = ProviderUserName.LoadBy(_entityConversation.MemberId.Value);
                    if(userNameList.Count > 0)
                    {
                        value = userNameList[0].UserName;
                    }
                }
                return value;
            }
        }

        public long? MemberId
        {
            get { return _entityConversation.MemberId; }
            set { _entityConversation.MemberId = value; }
        }

        public long? ArticleId
        {
            get { return _entityConversation.ArticleId; }
            set { _entityConversation.ArticleId = value; }
        }

        public long? PhotoId
        {
            get { return _entityConversation.Photo.Id; }
            set { _entityConversation.PhotoId = value; }
        }

        public List<ProviderComment> Comments
        {
            get { return ProviderComment.LoadByConversationId(Id.Value); }
        }

        public int CommentCount
        {
            get { return _entityConversation.Comments.Count; }
        }

        public bool IsHidden
        {
            get { return _entityConversation.IsHidden; }
            set { _entityConversation.IsHidden = value; }
        }

        override public bool Load(long id)
        {
            Conversation entityConversation = DbCtx.Instance.Conversations
                                                        .Where(conversation => conversation.Id == id)
                                                        .First();
            return Load(entityConversation);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityConversation.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityConversation.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityConversation.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityConversation.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityConversation.EditDate);
            sb.Append("\n\tArticleId =\t" + _entityConversation.ArticleId);
            sb.Append("\n\tMemberId =\t" + _entityConversation.MemberId);
            sb.Append("\n\tPhotoId =\t" + _entityConversation.PhotoId);
            sb.Append("\n\tIsHidden =\t" + _entityConversation.IsHidden);
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
            ProviderConversation aConversation = (ProviderConversation)untyped;
            _entityConversation.CreateDate = aConversation._entityConversation.CreateDate;
            _entityConversation.EditDate = aConversation._entityConversation.EditDate;
            _entityConversation.ArticleId = aConversation._entityConversation.ArticleId;
            _entityConversation.MemberId = aConversation._entityConversation.MemberId;
            _entityConversation.PhotoId = aConversation._entityConversation.PhotoId;
            _entityConversation.IsHidden = aConversation._entityConversation.IsHidden;
            _entityObject = _entityConversation;
            return true;
        }*/

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderConversation(Conversation aConversation) : base(aConversation) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityConversation; }
            set { _entityConversation = (Conversation)value; }
        }

        override protected void EntityClear()
        {
            _entityConversation = new Conversation();
            _entityConversation.Id = -1;
            _entityConversation.CreateDate = new DateTime();
            _entityConversation.EditDate = new DateTime();
            _entityConversation.ArticleId = null;
            _entityConversation.MemberId = null;
            _entityConversation.PhotoId = null;
            _entityConversation.IsHidden = false;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int IdDigitSize { get { return 18; } }

        static protected Converter<Conversation, ProviderConversation> _converterEntityToProvider = new Converter<Conversation, ProviderConversation>(_EntityToProvider);
        static protected ProviderConversation _EntityToProvider(Conversation conversationEntity)
        {
            return new ProviderConversation(conversationEntity);
        }

        static public List<ProviderConversation> LoadAll()
        {
            return DbCtx.Instance.Conversations.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderConversation> LoadByArticleId(long articleId)
        {
            return DbCtx.Instance.Conversations.Where(conversation => conversation.ArticleId == articleId)
                                                   .OrderByDescending(conversation => conversation.EditDate)
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
            return id >= 0 && DbCtx.Instance.Conversations.Where(aConversation => aConversation.Id == id).Count() > 0;
        }
    }
}