using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public static class RunOnDirtyDataUtility
    {
        private static readonly Dictionary<Type, Type[]> s_dataTypesCache = new Dictionary<Type, Type[]>();

        public static Type[] GetDataTypes(Type actionType)
        {
            if (actionType == null)
            {
                return Array.Empty<Type>();
            }

            if (s_dataTypesCache.TryGetValue(actionType, out Type[] cached))
            {
                return cached;
            }

            RunOnDirtyDataAttribute attribute = actionType.GetAttribute<RunOnDirtyDataAttribute>();
            Type[] dataTypes = attribute != null ? attribute.DataTypes : Array.Empty<Type>();

            s_dataTypesCache[actionType] = dataTypes;
            return dataTypes;
        }

        public static bool MatchesDirtyType(Type actionType, Type dirtiedType)
        {
            if (actionType == null || dirtiedType == null)
            {
                return false;
            }

            Type[] dataTypes = GetDataTypes(actionType);
            if (dataTypes == null || dataTypes.Length == 0)
            {
                return false;
            }

            foreach (Type type in dataTypes)
            {
                if (type == dirtiedType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
