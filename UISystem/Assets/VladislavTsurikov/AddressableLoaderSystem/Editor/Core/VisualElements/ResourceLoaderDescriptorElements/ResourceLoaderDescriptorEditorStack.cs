#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class ResourceLoaderDescriptorEditorStack
    {
        private static readonly Dictionary<Type, Type> s_editorTypeMap = new();

        private static readonly Dictionary<ResourceLoaderTemplate, ResourceLoaderDescriptorElement> s_instanceMap =
            new Dictionary<ResourceLoaderTemplate, ResourceLoaderDescriptorElement>();

        static ResourceLoaderDescriptorEditorStack()
        {
            foreach (Type editorType in AllTypesDerivedFrom<ResourceLoaderDescriptorElement>.Types)
            {
                LoaderBaseTypeAttribute attr = editorType.GetAttribute<LoaderBaseTypeAttribute>();
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

        public static ResourceLoaderDescriptorElement GetElement(ResourceLoaderTemplate template)
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

            var newElement = (ResourceLoaderDescriptorElement)Activator.CreateInstance(editorElementType, template);

            s_instanceMap.Add(template, newElement);

            return newElement;
        }
    }
}
#endif
