#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public static class EditorResourceLoaderRegistry
    {
        private static readonly List<ResourceLoaderTemplate> _templates = new List<ResourceLoaderTemplate>();
        public static IReadOnlyList<ResourceLoaderTemplate> Templates => _templates;

        public static void Refresh()
        {
            _templates.Clear();

            Type[] loaderTypes = AllTypesDerivedFrom<ResourceLoader>.Types;

            for (int i = 0; i < loaderTypes.Length; i++)
            {
                Type loaderType = loaderTypes[i];

                ResourceLoaderTemplate template = ResourceLoaderTemplateTypeRegistry.CreateByResourceType(loaderType);
                if (template == null)
                {
                    continue;
                }

                template.BuildFrom(loaderType);

                _templates.Add(template);
            }
        }
    }
}
#endif
