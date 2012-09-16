using System;
using System.Linq;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Collections.Generic;

namespace InsideWordProvider
{
    public class ProviderGroup : Provider
    {
        protected Group _entityGroup;

        public ProviderGroup() : base() { }
        public ProviderGroup(long id) : base(id) { }

        public DateTime CreateDate
        {
            get { return _entityGroup.CreateDate; }
            set { _entityGroup.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityGroup.EditDate; }
            set { _entityGroup.EditDate = value; }
        }

        public string Name
        {
            get { return _entityGroup.Name; }
            set { _entityGroup.Name = value; }
        }

        public bool IsHidden
        {
            get { return _entityGroup.IsHidden; }
            set { _entityGroup.IsHidden = value; }
        }

        public bool IsBanned
        {
            get { return _entityGroup.IsBanned; }
            set { _entityGroup.IsBanned = value; }
        }

        public List<ProviderMember> Members
        {
            get { return ProviderMember.LoadByGroupId(_entityGroup.Id); }
        }

        public List<ProviderRole> RolesWithoutGlobal
        {
            get { return ProviderRole.LoadByGroupId(_entityGroup.Id); }
        }

        public List<ProviderRole> RolesGloballyInherited
        {
            get { return ProviderRole.LoadGlobalRoles(); }
        }

        public List<ProviderRole> RolesWithGlobal
        {
            get { return ProviderRole.LoadByGroupIdWithGlobal(_entityGroup.Id); }
        }

        public List<ProviderArticle> Articles
        {
            get { return ProviderArticle.LoadByGroupId(_entityGroup.Id); }
        }

        public List<ProviderPhotoRecord> Photos
        {
            get { return ProviderPhotoRecord.LoadByGroupId(_entityGroup.Id); }
        }

        public bool Exists()
        {
            return DbCtx.Instance.Groups
                                      .Where(aGroup => aGroup.Id == Id ||
                                                     (Name != null && aGroup.Name == Name))
                                      .Count() > 1;
        }

        public override bool Load(long id)
        {
            Group entityGroup = DbCtx.Instance.Groups
                                            .Where(aGroup => aGroup.Id == id)
                                            .First();
            return Load(entityGroup);
        }

        /* TODO: revisit all copy functions
        public override bool Copy(Provider untyped)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            //Do not copy over the system times and only the business logic
            //times since the system times are specific to a given instance.
            ProviderGroup aGroup = (ProviderGroup)untyped;
            _entityGroup.CreateDate = aGroup._entityGroup.CreateDate;
            _entityGroup.EditDate = aGroup._entityGroup.EditDate;
            _entityGroup.Name = aGroup._entityGroup.Name;
            _entityGroup.IsHidden = aGroup._entityGroup.IsHidden;
            _entityGroup.IsBanned = aGroup._entityGroup.IsBanned;

            _entityObject = _entityGroup;
            return true;
        }*/


        //=========================================================
        // PRIVATE
        //=========================================================
        protected ProviderGroup(Group aGroup) : base(aGroup) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityGroup; }
            set { _entityGroup = (Group)value; }
        }
        
        protected override void EntityClear()
        {
            _entityGroup = new Group();
            _entityGroup.SystemCreateDate = new DateTime();
            _entityGroup.SystemEditDate = new DateTime();
            _entityGroup.CreateDate = new DateTime();
            _entityGroup.EditDate = new DateTime();
            _entityGroup.Id = -1;
            _entityGroup.Name = String.Empty;
            _entityGroup.IsHidden = false;
            _entityGroup.IsBanned = false;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        public const int NameSize = 32;

        static protected Converter<Group, ProviderGroup> _converterEntityToProvider = new Converter<Group, ProviderGroup>(_EntityToProvider);
        static protected ProviderGroup _EntityToProvider(Group groupEntity)
        {
            return new ProviderGroup(groupEntity);
        }

        static public List<ProviderGroup> LoadAll()
        {
            return DbCtx.Instance.Groups.ToList().ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Function that checks if a given Name string is already taken by an active or inactive Group.
        /// Note, null and empty string will result in the function returning false.
        /// </summary>
        /// <param name="Name">group name we'd like to check</param>
        /// <returns>long representing the id of the group who owns the name or null if it was not taken.</returns>
        static public long? ExistsName(string name)
        {
            return ExistsName(name, null);
        }

        /// <summary>
        /// Function that checks if a given Name string is already taken by a group.
        /// Note, null and empty string will result in the function returning false.
        /// </summary>
        /// <param name="Name">group name we'd like to check</param>
        /// <param name="isActive">If true then we will check only active groups. If false then only inactive groups. If null then we will check all members.</param>
        /// <returns>long representing the id of the group who owns the name or null if it was not taken.</returns>
        static public long? ExistsName(string name, bool? isActive)
        {
            long? groupId = null;
            if (!string.IsNullOrEmpty(name))
            {
                groupId = DbCtx.Instance.Groups
                                             .Where(aGroup => aGroup.Name == name)
                                             .Select(aGroup => aGroup.Id)
                                             .DefaultIfEmpty(-1)
                                             .Single();
                if (groupId == -1)
                {
                    groupId = null;
                }
            }
            return groupId;
        }

        static public List<ProviderGroup> LoadByMemberId(long memberId)
        {
            return DbCtx.Instance.Memberships
                .Where(aMembership => aMembership.MemberId == memberId)
                .Select(aMemberShip => aMemberShip.Group)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }
    }
}