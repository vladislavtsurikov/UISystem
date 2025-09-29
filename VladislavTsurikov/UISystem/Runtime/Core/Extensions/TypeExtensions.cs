using System;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class TypeExtensions
    {
        public static bool MatchesAnyFilter(this Type type, Func<FilterAttribute, bool> predicate)
        {
            FilterAttribute[] filters = type
                .GetCustomAttributes(typeof(FilterAttribute), true)
                .Cast<FilterAttribute>()
                .ToArray();

            if (filters.Length == 0)
            {
                Debug.LogError(
                    $"[UIHandlerManager] Type `{type.FullName}` does not have any FilterAttribute. This is required.");
                return false;
            }

            return filters.Any(predicate);
        }
    }
}
