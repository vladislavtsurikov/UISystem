#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class ResourceLoaderTemplateEditorStack
    {
        private static readonly Dictionary<Type, Type> s_editorTypeMap = new();

        private static readonly Dictionary<ResourceLoaderTemplate, ResourceLoaderTemplateElement> s_instanceMap =
            new Dictionary<ResourceLoaderTemplate, ResourceLoaderTemplateElement>();

        static ResourceLoaderTemplateEditorStack()
        {
            foreach (Type editorType in AllTypesDerivedFrom<ResourceLoaderTemplateElement>.Types)
            {
                ElementEditorAttribute attr = editorType.GetAttribute<ElementEditorAttribute>();
                if (attr == null)
                {
                    continue;
                }

                Type descriptorType = attr.Type;

                if (!s_editorTypeMap.ContainsKey(descriptorType))
                {
                    s_editorTypeMap.Add(descriptorType, editorType);
                }
            }
        }

        public static ResourceLoaderTemplateElement GetElement(ResourceLoaderTemplate template)
        {
            if (template == null)
            {
                return null;
            }

            if (s_instanceMap.TryGetValue(template, out var existing))
            {
                return existing;
            }

            Type descriptorType = template.GetType();

            if (!s_editorTypeMap.TryGetValue(descriptorType, out Type editorElementType))
            {
                return null;
            }

            var newElement = (ResourceLoaderTemplateElement)Activator.CreateInstance(editorElementType, template);

            s_instanceMap.Add(template, newElement);

            return newElement;
        }
    }
}
#endif
