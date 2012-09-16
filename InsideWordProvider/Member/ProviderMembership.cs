using System;
using System.Linq;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderMembership : Provider
    {
        protected Membership _entityMembership;

        public ProviderMembership() : base() { }
        public ProviderMembership(long id) : base(id) { }

        public DateTime CreateDate
        {
            get { return _entityMembership.CreateDate; }
            set { _entityMembership.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityMembership.EditDate; }
            set { _entityMembership.EditDate = value; }
        }

        public long MemberId
        {
            get { return _entityMembership.MemberId;}
            set { _entityMembership.MemberId = value;}
        }

        public long GroupId
        {
            get { return _entityMembership.GroupId; }
            set { _entityMembership.GroupId = value; }
        }

        public bool Exists()
        {
            return DbCtx.Instance.Memberships
                                      .Any(aMembership => aMembership.GroupId == GroupId && aMembership.MemberId == MemberId);
        }

        public override bool Load(long id)
        {
            Membership entityMembership = DbCtx.Instance.Memberships
                                            .Where(aMembership => aMembership.Id == id)
                                            .First();
            return Load(entityMembership);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityMembership.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityMembership.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityMembership.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityMembership.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityMembership.EditDate);
            sb.Append("\n\tGroupId =\t" + _entityMembership.GroupId);
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
            ProviderMembership aMembership = (ProviderMembership)untyped;
            _entityMembership.CreateDate = aMembership._entityMembership.CreateDate;
            _entityMembership.EditDate = aMembership._entityMembership.EditDate;

            _entityObject = _entityMembership;
            return true;
        }
        */

        //=========================================================
        // PRIVATE
        //=========================================================
        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityMembership; }
            set { _entityMembership = (Membership)value; }
        }

        protected override void EntityClear()
        {
            _entityMembership = new Membership();
            _entityMembership.SystemCreateDate = new DateTime();
            _entityMembership.SystemEditDate = new DateTime();
            _entityMembership.CreateDate = new DateTime();
            _entityMembership.EditDate = new DateTime();
            _entityMembership.GroupId = -1;
            _entityMembership.Id = -1;
        }
        
    }
}