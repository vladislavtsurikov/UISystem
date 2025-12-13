#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ResourceLoaderDescriptorContainer
    {
        private readonly Dictionary<Type, ResourceLoaderTemplate> _generatorMap = new();

        public ResourceLoaderDescriptorContainer()
        {
            Refresh();
            if (_generatorMap.Count > 0)
            {
                KeyValuePair<Type, ResourceLoaderTemplate> first = _generatorMap.First();
                ActiveType = first.Key;
                ActiveTemplate = first.Value;
            }
        }

        public IReadOnlyList<ResourceLoaderTemplate> Generators => _generatorMap.Values.ToList();

        public Type ActiveType { get; private set; }
        public ResourceLoaderTemplate ActiveTemplate { get; private set; }

        public ResourceLoaderTemplate GetFilled(Type resourceType)
        {
            if (resourceType == null)
            {
                return null;
            }

            ResourceLoaderTemplate templateTemplate = GetByBaseType(resourceType);
            if (templateTemplate != null)
            {
                return CreateFilledDescriptor(templateTemplate, resourceType);
            }

            Type baseType = resourceType.BaseType;
            if (baseType == null)
            {
                return null;
            }

            templateTemplate = GetByBaseType(baseType);
            if (templateTemplate != null)
            {
                return CreateFilledDescriptor(templateTemplate, resourceType);
            }

            return null;
        }

        private static ResourceLoaderTemplate CreateFilledDescriptor(ResourceLoaderTemplate template, Type loaderType)
        {
            Type descriptorType = template.GetType();
            var descriptor = (ResourceLoaderTemplate)Activator.CreateInstance(descriptorType);

            descriptor.BuildFrom(loaderType);

            return descriptor;
        }

        public void Refresh()
        {
            _generatorMap.Clear();

            foreach (ResourceLoaderTemplate descriptor in ReflectionFactory.CreateAllInstances<ResourceLoaderTemplate>())
            {
                _generatorMap.TryAdd(descriptor.BaseType, descriptor);
            }
        }

        public List<string> GetBaseTypeNames() =>
            _generatorMap.Keys
                .Select(t => t.Name)
                .Distinct()
                .ToList();

        public ResourceLoaderTemplate GetByBaseTypeName(string baseTypeName)
        {
            KeyValuePair<Type, ResourceLoaderTemplate> pair =
                _generatorMap.FirstOrDefault(x => x.Key.Name == baseTypeName);
            return pair.Value;
        }

        public ResourceLoaderTemplate GetByBaseType(Type baseType)
        {
            if (baseType == null)
            {
                return null;
            }

            return _generatorMap.GetValueOrDefault(baseType);
        }

        public void SetActiveByBaseTypeName(string baseTypeName)
        {
            KeyValuePair<Type, ResourceLoaderTemplate> pair =
                _generatorMap.FirstOrDefault(x => x.Key.Name == baseTypeName);
            if (pair.Key == null)
            {
                return;
            }

            ActiveType = pair.Key;
            ActiveTemplate = pair.Value;
        }

        public void ChangeResourceLoaderDescriptor(ResourceLoaderTemplate template)
        {
            _generatorMap[template.BaseType] = template;
        }
    }
}
#endif
