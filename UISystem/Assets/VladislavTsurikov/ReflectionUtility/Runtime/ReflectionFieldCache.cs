using System;
using System.Collections.Generic;
using System.Reflection;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    public static class ReflectionFieldCache
    {
        private static readonly Dictionary<Type, FieldInfo[]> _fieldCache = new();

        public static FieldInfo[] GetCachedFields(Type type, BindingFlags flags)
        {
            if (!_fieldCache.TryGetValue(type, out FieldInfo[] fields))
            {
                fields = type.GetFields(flags);
                _fieldCache[type] = fields;
            }

            return fields;
        }
    }
}
