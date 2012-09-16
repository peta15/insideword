using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderPermission : Provider
    {
        protected Permission _entityPermission;

        public ProviderPermission() : base() { }
        public ProviderPermission(long id) : base(id) { }

        public long? GroupId
        {
            get { return _entityPermission.GroupId; }
            set { _entityPermission.GroupId = value; }
        }

        public string Name
        {
            get { return _entityPermission.Name; }
            set { _entityPermission.Name = value; }
        }

        public string Description
        {
            get { return _entityPermission.Description; }
            set { _entityPermission.Description = value; }
        }

        public bool CanCreate
        {
            get { return _entityPermission.CanCreate; }
            set { _entityPermission.CanCreate = value; }
        }

        public bool CanEdit
        {
            get { return _entityPermission.CanEdit; }
            set { _entityPermission.CanEdit = value; }
        }

        public bool CanRead
        {
            get { return _entityPermission.CanRead; }
            set { _entityPermission.CanRead = value; }
        }

        public bool CanDelete
        {
            get { return _entityPermission.CanDelete; }
            set { _entityPermission.CanDelete = value; }
        }

        public long ObjectTypeId
        {
            get { return _entityPermission.ObjectTypeId; }
            set { _entityPermission.ObjectTypeId = value; }
        }

        public Type ObjectType
        {
            get
            {
                Type returnValue = null;
                ValidObjectTypeData.TryGetType(_entityPermission.ObjectTypeId, out returnValue);
                return returnValue;
            }
            set
            {
                _entityPermission.ObjectTypeId = ValidObjectTypeData[value];
            }
        }

        public bool IsGlobal
        {
            get { return !GroupId.HasValue; }
        }

        public override bool Load(long id)
        {
            Permission entityPermission = DbCtx.Instance.Permissions
                                              .Where(aPermission => aPermission.Id == id)
                                              .First();
            return Load(entityPermission);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityPermission.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityPermission.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityPermission.SystemEditDate);
            sb.Append("\n\tGroupId =\t" + _entityPermission.GroupId);
            sb.Append("\n\tName =\t" + _entityPermission.Name);
            sb.Append("\n\tDescription =\t" + _entityPermission.Description);
            sb.Append("\n\tObjectTypeId =\t" + _entityPermission.ObjectTypeId);
            sb.Append("\n\tCanCreate =\t" + _entityPermission.CanCreate);
            sb.Append("\n\tCanEdit =\t" + _entityPermission.CanEdit);
            sb.Append("\n\tCanRead =\t" + _entityPermission.CanRead);
            sb.Append("\n\tCanDelete =\t" + _entityPermission.CanDelete);
            sb.Append("\n");

            return sb.ToString();
        }

        /* TODO: revisit all copy functions
        public override bool Copy(Provider untyped)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            //Do not copy over the system times and only the business logic
            //times since the system times are specific to a given instance.
            ProviderPermission aRole = (ProviderPermission)untyped;
            _entityPermission.Name = aRole._entityPermission.Name;
            _entityPermission.Description = aRole._entityPermission.Description;
            _entityPermission.ObjectTypeId = aRole._entityPermission.ObjectTypeId;
            _entityPermission.CanCreate = aRole._entityPermission.CanCreate;
            _entityPermission.CanEdit = aRole._entityPermission.CanEdit;
            _entityPermission.CanRead = aRole._entityPermission.CanRead;
            _entityPermission.CanDelete = aRole._entityPermission.CanDelete;

            _entityObject = _entityPermission;
            return true;
        }
        */


        //=========================================================
        // PRIVATE
        //=========================================================
        protected ProviderPermission(Permission aPermission) : base(aPermission) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityPermission; }
            set { _entityPermission = (Permission)value; }
        }

        protected override void EntityClear()
        {
            _entityPermission = new Permission();
            _entityPermission.SystemCreateDate = new DateTime();
            _entityPermission.SystemEditDate = new DateTime();
            _entityPermission.GroupId = null;
            _entityPermission.Name = string.Empty;
            _entityPermission.Description = null;
            _entityPermission.ObjectTypeId = -1;
            _entityPermission.CanCreate = false;
            _entityPermission.CanEdit = false;
            _entityPermission.CanRead = false;
            _entityPermission.CanDelete = false;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================

        static protected Converter<Permission, ProviderPermission> _converterEntityToProvider = new Converter<Permission, ProviderPermission>(_EntityToProvider);
        static protected ProviderPermission _EntityToProvider(Permission permissionEntity)
        {
            return new ProviderPermission(permissionEntity);
        }

        static public List<ProviderPermission> LoadAll()
        {
            return DbCtx.Instance.Permissions.ToList().ConvertAll(_converterEntityToProvider);
        }
        
        static public ObjectTypeData ValidObjectTypeData { get; set; }

        static public long? Exists(long? groupId, long? objectTypeId, bool canCreate, bool canEdit, bool canRead, bool canDelete)
        {
            long? permissionId = DbCtx.Instance.Permissions
                                               .Where(aPermission => (!groupId.HasValue || aPermission.GroupId == groupId) && 
                                                                 aPermission.ObjectTypeId == objectTypeId &&
                                                                 aPermission.CanCreate == canCreate &&
                                                                 aPermission.CanEdit == canEdit &&
                                                                 aPermission.CanRead == canRead &&
                                                                 aPermission.CanDelete == canDelete)
                                               .Select(aPermission => aPermission.Id)
                                               .DefaultIfEmpty(-1)
                                               .Single();
            if (permissionId == -1)
            {
                permissionId = null;
            }
            return groupId;
        }

        public class ObjectTypeData
        {
            protected Dictionary<long, string> _objectIdsToDisplayNames;
            protected Dictionary<Type, long> _objectTypeToId;
            protected Dictionary<long, Type> _idToObjectType;

            public ObjectTypeData()
            {
                _objectIdsToDisplayNames = new Dictionary<long,string>();
                _objectTypeToId = new Dictionary<Type,long>();
                _idToObjectType = new Dictionary<long,Type>();
            }

            public void AddObjectTypeData(Type objectType, string displayName, long id)
            {
                _objectIdsToDisplayNames.Add(id, displayName);
                _objectTypeToId.Add(objectType, id);
                _idToObjectType.Add(id, objectType);
            }

            public Type this[long id]
            {
                get { return _idToObjectType[id]; } 
            }

            public long this[Type objectType]
            {
                get { return _objectTypeToId[objectType]; }
            }

            public bool TryGetType(long id, out Type objectType)
            {
                bool returnValue = true;
                Type outValue;
                if (_idToObjectType.TryGetValue(id, out outValue))
                {
                    objectType = outValue;
                }
                else
                {
                    objectType = null;
                    returnValue = false;
                }
                return returnValue;
            }

            public Dictionary<long, string> ObjectIdsToDisplayNames
            {
                get { return _objectIdsToDisplayNames; }
            }

            public Dictionary<Type, long> ObjectTypesToId
            {
                get { return _objectTypeToId; }
            }

            static public List<ProviderPermission> LoadByRoleId(long id)
            {
                return DbCtx.Instance.Roles
                    .Where(aRole => aRole.Id == id)
                    .SelectMany(aRole => aRole.Permissions)
                    .ToList()
                    .ConvertAll(_converterEntityToProvider);
            }
        }
    }
}