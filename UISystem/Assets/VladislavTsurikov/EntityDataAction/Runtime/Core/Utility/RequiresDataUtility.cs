using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public static class RequiresDataUtility
    {
        private static readonly Dictionary<Type, Type[]> s_requiredTypesCache = new Dictionary<Type, Type[]>();

        public static bool IsRequirementsMet(NodeStackOnlyDifferentTypes<ComponentData> data, Type actionType)
        {
            if (data == null || actionType == null)
            {
                return true;
            }

            Type[] requiredTypes = GetRequiredDataTypes(actionType);

            for (int i = 0; i < requiredTypes.Length; i++)
            {
                if (!data.HasType(requiredTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static Type[] GetRequiredDataTypes(Type actionType)
        {
            if (actionType == null)
            {
                return Array.Empty<Type>();
            }

            if (s_requiredTypesCache.TryGetValue(actionType, out Type[] cached))
            {
                return cached;
            }

            RequiresDataAttribute attribute = actionType.GetAttribute<RequiresDataAttribute>();

            Type[] requiredTypes = attribute != null ? attribute.RequiredTypes : Array.Empty<Type>();

            s_requiredTypesCache[actionType] = requiredTypes;
            return requiredTypes;
        }

        public static bool RequiresType(Type actionType, Type dirtiedType)
        {
            if (actionType == null || dirtiedType == null)
            {
                return false;
            }

            Type[] requiredTypes = GetRequiredDataTypes(actionType);
            if (requiredTypes == null || requiredTypes.Length == 0)
            {
                return false;
            }

            foreach (Type type in requiredTypes)
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
