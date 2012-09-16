using System.Configuration;

namespace InsideWordAdvancedResource.Config
{
    public class ObjectTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("objectType", IsRequired = true)]
        public string ObjectType
        {
            get { return (string)this["objectType"]; }
            set { this["objectType"] = value; }
        }

        [ConfigurationProperty("displayName", IsRequired = true)]
        public string DisplayName
        {
            get { return (string)this["displayName"]; }
            set { this["displayName"] = value; }
        }

        [ConfigurationProperty("id", IsRequired = true)]
        public long Id
        {
            get { return (long)this["id"]; }
            set { this["id"] = value; }
        }
    }

    public class ProviderPermissionElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        
        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get { return (string)this["description"]; }
            set { this["description"] = value; }
        }

        [ConfigurationProperty("objectType", IsRequired = true)]
        public string ObjectType
        {
            get { return (string)this["objectType"]; }
            set { this["objectType"] = value; }
        }

        [ConfigurationProperty("canCreate", DefaultValue = false, IsRequired = false)]
        public bool CanCreate
        {
            get { return (bool)this["canCreate"]; }
            set { this["canCreate"] = value; }
        }

        [ConfigurationProperty("canEdit", DefaultValue = false, IsRequired = false)]
        public bool CanEdit
        {
            get { return (bool)this["canEdit"]; }
            set { this["canEdit"] = value; }
        }

        [ConfigurationProperty("canRead", DefaultValue = false, IsRequired = false)]
        public bool CanRead
        {
            get { return (bool)this["canRead"]; }
            set { this["canRead"] = value; }
        }

        [ConfigurationProperty("canDelete", DefaultValue = false, IsRequired = false)]
        public bool CanDelete
        {
            get { return (bool)this["canDelete"]; }
            set { this["canDelete"] = value; }
        }
    }

    [ConfigurationCollection(typeof(ObjectTypeElement), AddItemName="objectTypeElement", CollectionType=ConfigurationElementCollectionType.BasicMap)]
    public class ObjectTypeCollection : ConfigurationElementCollection
    {
        public ObjectTypeElement this[int index]
        {
            get { return (ObjectTypeElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ObjectTypeElement this[string name]
        {
            get { return (ObjectTypeElement)base.BaseGet(name); }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "objectTypeElement"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ObjectTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ObjectTypeElement).DisplayName;
        }
    }

    [ConfigurationCollection(typeof(ProviderPermissionElement), AddItemName = "providerPermissionElement", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ProviderPermissionCollection : ConfigurationElementCollection
    {
        public ProviderPermissionElement this[int index]
        {
            get { return (ProviderPermissionElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ProviderPermissionElement this[string name]
        {
            get { return (ProviderPermissionElement)base.BaseGet(name); }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "providerPermissionElement"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderPermissionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ProviderPermissionElement).Name;
        }
    }

    public class ProviderPermissionConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("objectTypeList", IsRequired = true)]
        public ObjectTypeCollection ObjectTypeList
        {
            get { return (ObjectTypeCollection)this["objectTypeList"]; }
        }

        [ConfigurationProperty("providerPermissionList", IsRequired = false)]
        public ProviderPermissionCollection ProviderPermissionList
        {
            get { return (ProviderPermissionCollection)this["providerPermissionList"]; }
        }
    }
}
