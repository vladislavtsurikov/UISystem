#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ResourceLoaderDescriptorContainer
    {
        private readonly Dictionary<Type, ResourceLoaderDescriptor> _generatorMap = new();

        public ResourceLoaderDescriptorContainer()
        {
            Refresh();
            if (_generatorMap.Count > 0)
            {
                KeyValuePair<Type, ResourceLoaderDescriptor> first = _generatorMap.First();
                ActiveType = first.Key;
                ActiveDescriptor = first.Value;
            }
        }

        public IReadOnlyList<ResourceLoaderDescriptor> Generators => _generatorMap.Values.ToList();

        public Type ActiveType { get; private set; }
        public ResourceLoaderDescriptor ActiveDescriptor { get; private set; }

        public void Refresh()
        {
            _generatorMap.Clear();

            foreach (ResourceLoaderDescriptor descriptor in ReflectionFactory.CreateAllInstances<ResourceLoaderDescriptor>())
            {
                _generatorMap.TryAdd(descriptor.BaseType, descriptor);
            }
        }

        public List<string> GetBaseTypeNames() =>
            _generatorMap.Keys
                .Select(t => t.Name)
                .Distinct()
                .ToList();

        public ResourceLoaderDescriptor GetByBaseTypeName(string baseTypeName)
        {
            KeyValuePair<Type, ResourceLoaderDescriptor> pair =
                _generatorMap.FirstOrDefault(x => x.Key.Name == baseTypeName);
            return pair.Value;
        }

        public void SetActiveByBaseTypeName(string baseTypeName)
        {
            KeyValuePair<Type, ResourceLoaderDescriptor> pair =
                _generatorMap.FirstOrDefault(x => x.Key.Name == baseTypeName);
            if (pair.Key == null)
            {
                return;
            }

            ActiveType = pair.Key;
            ActiveDescriptor = pair.Value;
        }

        public void ChangeResourceLoaderDescriptor(ResourceLoaderDescriptor descriptor)
        {
            _generatorMap[descriptor.BaseType] = descriptor;
        }
    }
}
#endif
