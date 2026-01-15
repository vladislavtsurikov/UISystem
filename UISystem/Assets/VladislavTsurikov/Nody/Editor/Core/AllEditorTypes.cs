#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.Nody.Editor.Core
{
    public static class AllEditorTypes<T> where T : Element
    {
        public static Dictionary<Type, Type> Types = new(); // ElementType => EditorType

        static AllEditorTypes()
        {
            var componentTypes = AllTypesDerivedFrom<T>.Types.ToList();

            IEnumerable<Type> editorTypes = AllTypesDerivedFrom<ElementEditor>.Types
                .Where(
                    t => t.IsDefined(typeof(ElementEditorAttribute), false)
                         && !t.IsAbstract
                );

            foreach (Type type in editorTypes)
            {
                ElementEditorAttribute attribute = type.GetAttribute<ElementEditorAttribute>();

                if (componentTypes.Contains(attribute.Type))
                {
                    if (!Types.Keys.Contains(attribute.Type))
                    {
                        Types.Add(attribute.Type, type);

                        componentTypes.Remove(attribute.Type);
                    }
                }
            }
        }

        public static Type Get(Type elementType)
        {
            return Types[elementType];
        }

    }
}
#endif
