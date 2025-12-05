using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class ActiveResourceLoaderRegistry
    {
        private static readonly Dictionary<ResourceLoader, int> _refCounts = new();
        private static readonly HashSet<ResourceLoader> _activeLoaders = new();

        public static IReadOnlyCollection<ResourceLoader> ActiveLoaders => _activeLoaders;

        public static async UniTask RequestLoadAndRun(ResourceLoader loader, CancellationToken token = default)
        {
            RequestLoad(loader);
            await Run(token);
        }

        public static async UniTask RequestUnloadAndRun(ResourceLoader loader, CancellationToken token = default)
        {
            RequestUnload(loader);
            await Run(token);
        }

        public static void RequestLoad(ResourceLoader loader)
        {
            if (loader == null)
            {
                Debug.LogError("[ActiveResourceLoaderRegistry] Tried to RequestLoad with null loader");
                return;
            }

            if (_refCounts.TryGetValue(loader, out int count))
            {
                _refCounts[loader] = count + 1;
            }
            else
            {
                _refCounts[loader] = 1;
            }
        }

        public static void RequestUnload(ResourceLoader loader)
        {
            if (loader == null)
            {
                Debug.LogError("[ActiveResourceLoaderRegistry] Tried to RequestUnload with null loader");
                return;
            }

            if (_refCounts.TryGetValue(loader, out int count))
            {
                _refCounts[loader] = Mathf.Max(0, count - 1);
            }
        }

        public static async UniTask Run(CancellationToken token)
        {
#if ADDRESSABLE_LOADER_LOGS
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            var loadTasks = new List<UniTask>();
            var unloadTasks = new List<UniTask>();

            foreach (var kvp in _refCounts)
            {
                ResourceLoader loader = kvp.Key;
                int count = kvp.Value;

                if (count == 0)
                {
                    if (_activeLoaders.Contains(loader))
                    {
                        unloadTasks.Add(loader.Unload(token));
                        _activeLoaders.Remove(loader);
                    }
                    continue;
                }

                if (!_activeLoaders.Contains(loader))
                {
                    loadTasks.Add(loader.LoadResourceLoader(token));
                    _activeLoaders.Add(loader);
                }
            }

            if (loadTasks.Count > 0)
                await UniTask.WhenAll(loadTasks);

            if (unloadTasks.Count > 0)
                await UniTask.WhenAll(unloadTasks);

#if ADDRESSABLE_LOADER_LOGS
            stopwatch.Stop();
            Debug.Log($"[ActiveResourceLoaderRegistry] Run() completed in {stopwatch.Elapsed.ToReadableString()}");
#endif
        }

        public static void Clear()
        {
            _refCounts.Clear();
            _activeLoaders.Clear();
#if ADDRESSABLE_LOADER_LOGS
            Debug.Log("[ActiveResourceLoaderRegistry] Cleared all tracked loaders.");
#endif
        }
    }
}
