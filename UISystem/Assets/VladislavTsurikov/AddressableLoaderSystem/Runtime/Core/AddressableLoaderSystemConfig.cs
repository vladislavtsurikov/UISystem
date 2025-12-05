using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using UnityEditor;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    [LocationAsset("AddressableLoaderSystem/AddressableLoaderSystemConfig")]
    public class AddressableLoaderSystemConfig : SerializedScriptableObjectSingleton<AddressableLoaderSystemConfig>
    {
        public enum LoaderSourceMode
        {
            Cache,
            Reflection
        }

        [OdinSerialize]
        private List<LoaderBehavior> _cachedBehaviors = new();

        [OdinSerialize]
        private List<ResourceLoader> _cachedLoaders = new();

        [NonSerialized]
        private bool _initialized;

        [OdinSerialize]
        internal LoaderSourceMode _mode = LoaderSourceMode.Cache;

        public IEnumerable<ResourceLoader> GetAllLoaders()
        {
            EnsureInitialized();

            for (int i = 0; i < _cachedLoaders.Count; i++)
            {
                var loader = _cachedLoaders[i];
                if (loader != null)
                {
                    yield return loader;
                }
                else
                {
                    UnityEngine.Debug.LogError($"[AddressableLoaderSystemConfig] Null ResourceLoader detected at index {i}.");
                }
            }
        }

        public IEnumerable<LoaderBehavior> GetAllBehaviors()
        {
            EnsureInitialized();

            for (int i = 0; i < _cachedBehaviors.Count; i++)
            {
                var behavior = _cachedBehaviors[i];
                if (behavior != null)
                {
                    yield return behavior;
                }
                else
                {
                    UnityEngine.Debug.LogError($"[AddressableLoaderSystemConfig] Null LoaderBehavior detected at index {i}.");
                }
            }
        }

        private void EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }

            if (_mode == LoaderSourceMode.Reflection)
            {
                Refresh();
            }

            _initialized = true;
        }

        public void Refresh()
        {
            RefreshLoadersCache();
            RefreshBehaviorsCache();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void RefreshLoadersCache()
        {
            _cachedLoaders.Clear();

            IEnumerable<ResourceLoaderRegistrar> registrars =
                ReflectionFactory.CreateAllInstances<ResourceLoaderRegistrar>();
            foreach (ResourceLoaderRegistrar registrar in registrars)
            {
                foreach (ResourceLoader loader in registrar.GetLoaders())
                {
                    if (loader != null && !_cachedLoaders.Any(l => l != null && l.GetType() == loader.GetType()))
                    {
                        _cachedLoaders.Add(loader);
                    }
                }
            }
        }

        private void RefreshBehaviorsCache()
        {
            _cachedBehaviors.Clear();

            Dictionary<Type, List<ResourceLoader>> grouped = _cachedLoaders.GroupByBehaviorType();

            foreach ((Type behaviorType, List<ResourceLoader> loaders) in grouped)
            {
                if (behaviorType == null || loaders.Count == 0)
                {
                    continue;
                }

                var behavior = (LoaderBehavior)Activator.CreateInstance(behaviorType, loaders);
                if (behavior != null)
                {
                    _cachedBehaviors.Add(behavior);
                }
            }
        }
    }
}
