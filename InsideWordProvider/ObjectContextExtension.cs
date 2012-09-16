using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;

namespace InsideWordProvider
{
    static public class ObjectContextExtension
    {
        public static string GetEntitySetName(this ObjectContext context, EntityObject entity)
        {
            Type entityType = ObjectContext.GetObjectType(entity.GetType());
            if (entityType == null)
            {
                throw new InvalidOperationException("not an entity");
            }

            EntityContainer container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            return container.BaseEntitySets
                            .Where(anEntitySet => anEntitySet.ElementType.Name.Equals(entityType.Name))
                            .Select(anEntitySet => anEntitySet.Name)
                            .Single();
        }
    }
}
