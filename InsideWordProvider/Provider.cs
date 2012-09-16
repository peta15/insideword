using System;
using System.Linq;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.Metadata.Edm;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using log4net;
using log4net.Config;

namespace InsideWordProvider
{
    [Serializable]
    public abstract class Provider
    {
        public Provider()
        {
            Clear();
        }

        public Provider(long id)
        {
            if (!Load(id))
            {
                throw new Exception("Failed to load "+this.GetType().Name+" with id " + id);
            }
        }

        public long? Id
        {
            get
            {
                if (UnderlyingEntity.Id < 0)
                {
                    return null;
                }
                else
                {
                    return UnderlyingEntity.Id;
                }
            }
        }

        public DateTime SystemCreateDate
        {
            get { return new DateTime(UnderlyingEntity.SystemCreateDate.Ticks); }
            protected set { UnderlyingEntity.SystemCreateDate = value; }
        }

        public DateTime SystemEditDate
        {
            get { return new DateTime(UnderlyingEntity.SystemEditDate.Ticks); }
            protected set { UnderlyingEntity.SystemEditDate = value; }
        }

        abstract public bool Load(long id);
        /*abstract public bool Copy(Provider aProvider); TODO revisit All Copy functions*/

        // if Clear fails, throw an exception
        virtual public void Clear()
        {
            ClassClear();
            EntityClear();
        }

        public bool IsNew { get { return !Id.HasValue; } }

        virtual public void Save()
        {
            try
            {
                if (IsNew)
                {
                    SystemCreateDate = DateTime.UtcNow;
                    string entitySetName = DbCtx.Instance.GetEntitySetName((EntityObject)UnderlyingEntity);
                    DbCtx.Instance.AddObject(entitySetName, UnderlyingEntity);
                }
                SystemEditDate = DateTime.UtcNow;

                DbCtx.Instance.SaveChanges();
                // Performing a refresh on an entity is important as it makes sure that all data is reloaded.
                // This is important if the database uses triggers.
                DbCtx.Instance.Refresh(RefreshMode.StoreWins, UnderlyingEntity);
            }
            catch (Exception caughtException)
            {
                try { DbCtx.Instance.Detach(UnderlyingEntity); }
                catch(Exception ignoredException)
                {
                    // leave the exception for debugging "step through" purposes.
                    // but ultimately we don't care about whether the object successfully detached or not.
                }
                _logger.Error("Save Failed:\n" +ToString(), caughtException);
                throw caughtException;
            }
        }

        virtual public bool Delete()
        {
            if (!IsNew)
            {
                DbCtx.Instance.DeleteObject(UnderlyingEntity);
                DbCtx.Instance.SaveChanges();
            }
            Clear();

            return true;
        }

        //=========================================================
        // PRIVATE
        //=========================================================
        protected Provider(IInsideWordEntity entityObject)
        {
            Load(entityObject);
        }
        
        // If EntityClear fails, throw an exception
        abstract protected void EntityClear();

        // If ClassClear fails, throw an exception
        virtual protected void ClassClear() { }

        virtual protected bool Load(IInsideWordEntity entityObject)
        {
            ClassClear();
            bool returnValue = true;
            if (entityObject == null)
            {
                EntityClear();
                returnValue = false;
            }
            else
            {
                UnderlyingEntity = entityObject;
            }
            return returnValue;
        }

        abstract protected IInsideWordEntity UnderlyingEntity { get; set; }

        //=========================================================
        // STATIC
        //=========================================================

        static protected ILog _logger;
        static protected Dictionary<string, int?> _maxFieldValues;
        static protected IEnumerable<GlobalItem> _entityTypeList;

        // static constructor
        static Provider()
        {
            _logger = LogManager.GetLogger("InsideWordProvider");
            XmlConfigurator.Configure();
            _maxFieldValues = new Dictionary<string, int?>();
        }

        static public IObjectContextManager DbCtx { get; set; }

        /// <summary>
        /// Function returns the max length of an entity's string attributes.
        /// </summary>
        /// <param name="entityName">entity whose attributes we want to check.</param>
        /// <param name="attributeName">attribute from the entity whose max length we want to get.</param>
        /// <returns>int value for the max length of the string attribute.</returns>
        static protected int? GetMaxLength(string entityName,  string attributeName)
        {
            if (_entityTypeList == null)
            {
                _entityTypeList = DbCtx.Instance.MetadataWorkspace
                                                 .GetItems(DataSpace.CSpace)
                                                 .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType);
            }

            int? maxValue = null;
            string key = entityName + " - " + attributeName;
            if(!_maxFieldValues.TryGetValue(key, out maxValue))
            {
                MetadataProperty attribute = _entityTypeList.Where(m => m.GetType().Name == entityName)
                                                             .Single()
                                                             .MetadataProperties
                                                             .Where(property => property.Name == attributeName &&
                                                                                property.TypeUsage.EdmType.Name == "String")
                                                             .Single();

                maxValue = Convert.ToInt32(attribute.TypeUsage.Facets["MaxLength"].Value);
                _maxFieldValues.Add(key, maxValue);
            }
            
                
            return maxValue;
        }
    }
}