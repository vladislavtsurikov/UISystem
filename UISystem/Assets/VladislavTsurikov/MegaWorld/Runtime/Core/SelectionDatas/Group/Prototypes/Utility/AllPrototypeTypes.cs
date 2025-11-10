using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility
{
    public static class AllPrototypeTypes
    {
        public static readonly List<Type> TypeList;

        static AllPrototypeTypes()
        {
            IEnumerable<Type> types = AllTypesDerivedFrom<Prototype>.Types
                .Where(
                    t => t.IsDefined(typeof(NameAttribute), false) && !t.IsAbstract
                );

            TypeList = types.ToList();
        }
    }
}
