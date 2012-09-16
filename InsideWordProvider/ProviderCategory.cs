using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects;
using InsideWordResource;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderCategory : Provider
    {
        protected Category _entityCategory;
        protected ChangeList<long> _pendingArticleIds;

        public ProviderCategory() : base() { }
        public ProviderCategory(long id) : base(id) { }

        public DateTime CreateDate
        {
            get { return _entityCategory.CreateDate; }
            set { _entityCategory.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityCategory.EditDate; }
            set { _entityCategory.EditDate = value; }
        }

        public string Title
        {
            get { return _entityCategory.Title; }
            set { _entityCategory.Title = value; }
        }

        public bool IsHidden
        {
            get { return _entityCategory.IsHidden; }
            set { _entityCategory.IsHidden = value; }
        }

        public long? ParentId
        {
            get
            {
                return (_entityCategory.ParentCategoryId.HasValue)
                       ?_entityCategory.ParentCategoryId
                       :-1;
            }
            set { _entityCategory.ParentCategoryId = value; }
        }

        public string ParentName
        {
            get { return (_entityCategory.ParentCategory == null)? "": _entityCategory.ParentCategory.Title; }
        }

        public int HierarchyLevel
        {
            get { return _entityCategory.HierarchyLevel; }
            protected set { _entityCategory.HierarchyLevel = value; }
        }

        public string FullPath
        {
            get { return _entityCategory.FullPath; }
        }

        /// <summary>
        /// function returns the children categories of the current category
        /// </summary>
        /// <param name="showHidden">
        /// If set to true then it will show all categories.
        /// If set to false then it will show only non-hidden categories.
        /// </param>
        /// <returns>returns a list of categories</returns>
        public List<ProviderCategory> Children(bool showHidden)
        {
            int level = _entityCategory.HierarchyLevel + 1;
            string path = _entityCategory.FullPath;

            return DbCtx.Instance.Categories
                                   .Where(aCategory=> aCategory.HierarchyLevel == level &&
                                                      (showHidden || aCategory.IsHidden == showHidden) &&
                                                      aCategory.FullPath.StartsWith(path))
                                   .ToList()
                                   .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// function returns only the non-hidden children categories of the current category.
        /// </summary>
        /// <returns>returns a list of categories</returns>
        public List<ProviderCategory> Children()
        {
            return Children(false);
        }

        public void AddArticle(long articleId)
        {
            _pendingArticleIds.Add(articleId);
        }

        public void RemoveArticle(long articleId)
        {
            _pendingArticleIds.Remove(articleId);
        }

        public bool IsRoot
        {
            get { return Id == RootId; }
        }

        override public bool Load(long id)
        {
            return Load(id, false);
        }

        public bool Load(long id, bool returnRootIfBadId)
        {
            Category category = DbCtx.Instance.Categories.Where(aCategory => aCategory.Id == id).FirstOrDefault();

            if (category == null && returnRootIfBadId)
            {
                category = GetRootCategoryEntity();
            }

            return Load(category);
        }

        override public void Save()
        {
            if (IsRoot)
            {
                throw new Exception("Root cannot be modified");
            }

            long? titleOwner = ExistsTitle(Title);

            // Don't save if we don't own the title
            if (titleOwner.HasValue && titleOwner.Value != Id)
            {
                throw new Exception("Title for category already taken");
            }
            
            if (IsNew)
            {
                // if it's a new category then we have to save it first before doing the relationships.
                base.Save();
            }

            //Handle the pending article ids
            foreach (long articleId in _pendingArticleIds.AddList)
            {
                Article anArticle = DbCtx.Instance.Articles.Where(article => article.Id == articleId).Single();
                _entityCategory.Articles.Add(anArticle);
                anArticle.Categories.Add(_entityCategory);//don't forget the relationship to the article or else it's one way
            }

            foreach (long articleId in _pendingArticleIds.RemoveList)
            {
                Article anArticle = DbCtx.Instance.Articles.Where(article => article.Id == articleId).Single();
                _entityCategory.Articles.Remove(anArticle);
                anArticle.Categories.Remove(_entityCategory);//don't forget the relationship to the article or else it's one way
            }
            _pendingArticleIds.Clear();

            base.Save();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityCategory.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityCategory.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityCategory.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityCategory.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityCategory.EditDate);
            sb.Append("\n\tIsHidden =\t" + _entityCategory.IsHidden);
            sb.Append("\n\tTitle =\t" + _entityCategory.Title);
            sb.Append("\n\tParentCategoryId =\t" + _entityCategory.ParentCategoryId);
            sb.Append("\n\tHierarchyLevel =\t" + _entityCategory.HierarchyLevel);
            sb.Append("\n\tFullPath =\t" + _entityCategory.FullPath);
            sb.Append("\n");

            return sb.ToString();
        }

        /* TODO: revisit all copy functions
        override public bool Copy(Provider untyped)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            ProviderCategory aCategory = (ProviderCategory)untyped;
            _entityObject                    = _entityCategory;
            _entityCategory.CreateDate       = aCategory._entityCategory.CreateDate;
            _entityCategory.EditDate         = aCategory._entityCategory.EditDate;
            _entityCategory.IsHidden         = aCategory._entityCategory.IsHidden;
            _entityCategory.Title            = aCategory._entityCategory.Title;
            _entityCategory.ParentCategoryId = aCategory._entityCategory.ParentCategoryId;
            _entityCategory.HierarchyLevel   = aCategory._entityCategory.HierarchyLevel;
            _entityCategory.FullPath         = aCategory._entityCategory.FullPath;
            _pendingArticleIds.Copy(aCategory._pendingArticleIds);
            return true;
        }*/

        override public bool Delete()
        {
            if (IsRoot)
            {
                throw new Exception("Root cannot be deleted");
            }

            return base.Delete();
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderCategory(Category aCategory) : base(aCategory) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityCategory; }
            set { _entityCategory = (Category)value; }
        }

        protected override void ClassClear()
        {
            base.ClassClear();
            _pendingArticleIds = new ChangeList<long>();
        }

        override protected void EntityClear()
        {
            _entityCategory = new Category();
            _entityCategory.Id = -1;
            _entityCategory.SystemCreateDate = new DateTime();
            _entityCategory.SystemEditDate = new DateTime();
            _entityCategory.CreateDate = new DateTime();
            _entityCategory.EditDate = new DateTime();
            _entityCategory.IsHidden = false;
            _entityCategory.Title = String.Empty;
            _entityCategory.ParentCategoryId = Root.Id;
            _entityCategory.HierarchyLevel = -1; // useful to get all categories in a certain level or check what level this category is on
            _entityCategory.FullPath = String.Empty; // used internally by the entity framework and database
        }
        
        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static public int TitleSize   { get { return 128; } }
        static public int IdDigitSize { get { return 18; } }

        static ProviderCategory()
        {
            // Always check on startup if the root exists. If it doesn't then create it.
            // Currently the way to check if the root exists is if the table is empty.
            // We don't currently handle the root being deleted by accident.
            using (InsideWordEntities dbCtx = new InsideWordEntities())
            {
                if (dbCtx.Categories.Count() == 0)
                {
                    CreateRoot(dbCtx);
                }
            }
        }

        static protected bool CreateRoot(InsideWordEntities dbCtx)
        {
            // Note: The root creation function behaves VERY differently than a normal provider save
            Category root = new Category();
            root.Id = -1;
            root.SystemCreateDate = DateTime.UtcNow;
            root.SystemEditDate = DateTime.UtcNow;
            root.CreateDate = DateTime.UtcNow;
            root.EditDate = DateTime.UtcNow;
            root.IsHidden = false;
            root.Title = "root";
            root.ParentCategoryId = null;
            root.HierarchyLevel = -1; // useful to get all categories in a certain level or check what level this category is on
            root.FullPath = String.Empty; // used internally by the entity framework and database

            try
            {
                dbCtx.Categories.AddObject(root);
                dbCtx.SaveChanges();
                // Since the categories use triggers to update their hierarchy it is mandatory that a refresh be done on the entity
                // otherwise the data in HierarchyLevel and FullPath can be incorrect after a save.
                dbCtx.Refresh(RefreshMode.StoreWins, root);
                
            }
            catch (Exception caughtException)
            {
                DbCtx.Instance.Detach(root);
                throw caughtException;
            }

            return true;
        }

        static private Category GetRootCategoryEntity()
        {
            return DbCtx.Instance.Categories.Where(c => c.HierarchyLevel == 0).Single();
        }

        static protected Converter<Category, ProviderCategory> _converterEntityToProvider = new Converter<Category, ProviderCategory>(_EntityToProvider);
        static protected ProviderCategory _EntityToProvider(Category categoryEntity)
        {
            return new ProviderCategory(categoryEntity);
        }

        static public List<ProviderCategory> LoadAll()
        {
            return DbCtx.Instance.Categories.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<long> LoadAllIds()
        {
            return DbCtx.Instance.Categories.Select(aCategory => aCategory.Id)
                                                 .ToList();
        }

        static protected ProviderCategory _root;
        static public ProviderCategory Root
        {
            get
            {
                if( _root == null)
                {
                    Category root = DbCtx.Instance.Categories
                                       .Where(category => category.HierarchyLevel == 0)
                                       .Single();
                    _root = new ProviderCategory(root);
                }
                return _root;
            }
        }

        static public long RootId { get { return Root.Id.Value; } }

        static public bool IsRootId(long id)
        {
            return RootId == id;
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.Categories.Any(aCategory => aCategory.Id == id);
        }

        /// <summary>
        /// Function that checks if a given title string is already taken by a category.
        /// Note, null and empty string will result in the function returning false.
        /// </summary>
        /// <param name="userName">title we'd like to check</param>
        /// <returns>long representing the id of the category with the title or null if it was not taken.</returns>
        static public long? ExistsTitle(string title)
        {
            long? categoryId = null;
            if (!string.IsNullOrEmpty(title))
            {
                categoryId = DbCtx.Instance.Categories
                                              .Where(aCategory => aCategory.Title == title)
                                              .Select(aCategory => aCategory.Id)
                                              .DefaultIfEmpty(-1)
                                              .Single();
                if (categoryId == -1)
                {
                    categoryId = null;
                }
            }
            return categoryId;
        }

        static public string GetFullPath(long categoryId)
        {
            string fullPath = null;
            if (categoryId>-1)
            {
                fullPath = DbCtx.Instance.Categories
                                              .Where(aCategory => aCategory.Id == categoryId)
                                              .Select(aCategory => aCategory.FullPath)
                                              .FirstOrDefault();
            }
            return fullPath;
        }
    }
}