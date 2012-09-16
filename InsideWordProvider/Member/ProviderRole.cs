using System;
using System.Linq;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Collections.Generic;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderRole : Provider
    {
        protected Role _entityRole;

        public ProviderRole() : base() { }
        public ProviderRole(long id) : base(id) { }

        public long? GroupId
        {
            get { return _entityRole.GroupId; }
            set { _entityRole.GroupId = value; }
        }

        public string Name
        {
            get { return _entityRole.Name; }
            set { _entityRole.Name = value; }
        }

        public string Description
        {
            get { return _entityRole.Description; }
            set { _entityRole.Description = value; }
        }

        public bool IsGlobal
        {
            get { return !GroupId.HasValue; }
        }

        public bool Exists()
        {
            return DbCtx.Instance.Roles
                                      .Any(aRole => aRole.GroupId == GroupId && aRole.Name == Name);
        }

        public override bool Load(long id)
        {
            Role entityRole = DbCtx.Instance.Roles
                                            .Where(aRole => aRole.Id == id)
                                            .Single();
            return Load(entityRole);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityRole.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityRole.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityRole.SystemEditDate);
            sb.Append("\n\tGroupId =\t" + _entityRole.GroupId);
            sb.Append("\n\tName =\t" + _entityRole.Name);
            sb.Append("\n\tDescription =\t" + _entityRole.Description);
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
            ProviderRole aRole = (ProviderRole)untyped;
            _entityRole.Name = aRole._entityRole.Name;
            _entityRole.Description = aRole._entityRole.Description;

            _entityObject = _entityRole;
            return true;
        }
        */


        //=========================================================
        // PRIVATE
        //=========================================================
        protected ProviderRole(Role aRole) : base(aRole) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityRole; }
            set { _entityRole = (Role)value; }
        }

        protected override void EntityClear()
        {
            _entityRole = new Role();
            _entityRole.SystemCreateDate = new DateTime();
            _entityRole.SystemEditDate = new DateTime();
            _entityRole.GroupId = null;
            _entityRole.Name = string.Empty;
            _entityRole.Description = null;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================

        static protected Converter<Role, ProviderRole> _converterEntityToProvider = new Converter<Role, ProviderRole>(_EntityToProvider);
        static protected ProviderRole _EntityToProvider(Role roleEntity)
        {
            return new ProviderRole(roleEntity);
        }

        static public List<ProviderRole> LoadAll()
        {
            return DbCtx.Instance.Roles.ToList().ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderRole> LoadByGroupId(long id)
        {
            return DbCtx.Instance.Groups
                .Where(aGroup => aGroup.Id == id)
                .SelectMany(aGroup => aGroup.Roles)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderRole> LoadGlobalRoles()
        {
            return DbCtx.Instance.Roles
                .Where(aRole => !aRole.GroupId.HasValue)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderRole> LoadByGroupIdWithGlobal(long groupId)
        {
            return DbCtx.Instance.Roles
                .Where(aRole => aRole.GroupId == groupId || aRole.GroupId == null)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }

        static public List<ProviderRole> LoadMemberRolesWithinGroup(long groupId, long memberId)
        {
            return DbCtx.Instance.Memberships
                .Where(aMember => aMember.MemberId == memberId && aMember.GroupId == groupId)
                .SelectMany(aMember => aMember.Roles)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }
    }
}