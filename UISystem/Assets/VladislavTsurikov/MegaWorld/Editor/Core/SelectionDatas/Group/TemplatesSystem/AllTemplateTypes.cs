#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
    public static class AllTemplateTypes
    {
        public static List<Type> TypeList = new();

        static AllTemplateTypes()
        {
            IEnumerable<Type> types = AllTypesDerivedFrom<Template>.Types
                .Where(
                    t => t.IsDefined(typeof(TemplateAttribute), false)
                         && !t.IsAbstract
                );

            foreach (Type type in types)
            {
                TypeList.Add(type);
            }
        }
    }
}
#endif
