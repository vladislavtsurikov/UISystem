#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements
{
    public static class ResourceLoaderDescriptorEditorStack
    {
        private static readonly Dictionary<Type, Type> s_editorTypeMap = new();

        private static readonly Dictionary<ResourceLoaderDescriptor, ResourceLoaderDescriptorElement> s_instanceMap =
            new Dictionary<ResourceLoaderDescriptor, ResourceLoaderDescriptorElement>();

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

        public static ResourceLoaderDescriptorElement GetElement(ResourceLoaderDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return null;
            }

            if (s_instanceMap.TryGetValue(descriptor, out var existing))
            {
                return existing;
            }

            Type descriptorType = descriptor.GetType();

            if (!s_editorTypeMap.TryGetValue(descriptorType, out Type editorElementType))
            {
                return null;
            }

            var newElement = (ResourceLoaderDescriptorElement)Activator.CreateInstance(editorElementType, descriptor);

            s_instanceMap.Add(descriptor, newElement);

            return newElement;
        }
    }
}
#endif
