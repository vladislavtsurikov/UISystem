using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static partial class ResourceLoaderExtensions
    {
        public static Dictionary<Type, List<ResourceLoader>> GroupByBehaviorType(this IEnumerable<ResourceLoader> loaders)
        {
            var dict = new Dictionary<Type, List<ResourceLoader>>();

            foreach (var loader in loaders)
            {
                if (loader == null)
                {
                    continue;
                }

                var attr = (BehaviorAttribute)loader.GetType()
                    .GetCustomAttributes(typeof(BehaviorAttribute), false)
                    .FirstOrDefault();

                if (attr == null || attr.BehaviorType == null)
                {
                    continue;
                }

                if (!dict.TryGetValue(attr.BehaviorType, out var list))
                {
                    list = new List<ResourceLoader>();
                    dict[attr.BehaviorType] = list;
                }

                list.Add(loader);
            }

            return dict;
        }
    }
}
