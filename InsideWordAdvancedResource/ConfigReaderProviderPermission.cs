using System;
using System.Collections.Generic;
using InsideWordProvider;

namespace InsideWordAdvancedResource.Config
{
    public static class ConfigReaderProviderPermission
    {
        static public void Parse(ObjectTypeElement anElement, InsideWordProvider.ProviderPermission.ObjectTypeData objectTypeData)
        {
            objectTypeData.AddObjectTypeData(Type.GetType(anElement.ObjectType),
                                             anElement.DisplayName,
                                             anElement.Id);
        }

        static public ProviderPermission Parse(ProviderPermissionElement anElement)
        {
            ProviderPermission aPermission = new ProviderPermission();
            aPermission.Name = anElement.Name;
            aPermission.Description = anElement.Description;
            aPermission.ObjectType = Type.GetType(anElement.ObjectType);
            aPermission.CanCreate = anElement.CanCreate;
            aPermission.CanEdit = anElement.CanEdit;
            aPermission.CanRead = anElement.CanRead;
            aPermission.CanDelete = anElement.CanDelete;
            return aPermission;
        }

        static public Dictionary<string, ProviderPermission> Parse(ProviderPermissionConfigSection aSection)
        {
            ProviderPermission.ObjectTypeData validObjectTypeData = new ProviderPermission.ObjectTypeData();
            foreach (ObjectTypeElement anElement in aSection.ObjectTypeList)
            {
                Parse(anElement, validObjectTypeData);
            }
            ProviderPermission.ValidObjectTypeData = validObjectTypeData;

            Dictionary<string, ProviderPermission> configPermissions = new Dictionary<string,ProviderPermission>();
            foreach(ProviderPermissionElement anElement in aSection.ProviderPermissionList)
            {
                ProviderPermission aPermission = Parse(anElement);
                configPermissions.Add(aPermission.Name, aPermission);
            }

            return configPermissions;
        }
    }
}
