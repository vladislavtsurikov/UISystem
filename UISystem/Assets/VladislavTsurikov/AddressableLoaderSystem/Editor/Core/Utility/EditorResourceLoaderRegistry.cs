#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class EditorResourceLoaderRegistry
    {
        private static readonly List<ResourceLoaderTemplate> _templates = new List<ResourceLoaderTemplate>();
        public static IReadOnlyList<ResourceLoaderTemplate> Templates => _templates;

        public static void Refresh()
        {
            _templates.Clear();

            Type[] loaderTypes = AllTypesDerivedFrom<ResourceLoader>.Types;

            foreach (var loaderType in loaderTypes)
            {
                ResourceLoaderTemplate template = ResourceLoaderTemplateTypeRegistry.CreateByResourceType(loaderType);
                if (template == null)
                {
                    Debug.LogWarning($"[AddressableLoaderSystem][EditorResourceLoaderRegistry.Refresh] Unable to create template for loader type '{loaderType?.Name ?? "Unknown"}'.");
                    continue;
                }

                template.BuildFrom(loaderType);

                _templates.Add(template);
            }
        }
    }
}
#endif
