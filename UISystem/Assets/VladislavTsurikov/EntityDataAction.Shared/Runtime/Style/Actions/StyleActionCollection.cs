using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    public sealed class StyleActionCollection : EntityActionCollection
    {
        protected override bool AllowCreate(Type type)
        {
            if (!typeof(EntityAction).IsAssignableFrom(type))
            {
                return false;
            }

            NameAttribute nameAttribute = type.GetAttribute<NameAttribute>();
            if (nameAttribute == null)
            {
                return false;
            }

            return nameAttribute.Name.StartsWith("UI/Common/Style/", StringComparison.Ordinal);
        }
    }
}
