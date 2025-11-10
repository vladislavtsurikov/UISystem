#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    internal static class AddressableAssetTracker
    {
        private static readonly Dictionary<object, TrackedEntry> _registry = new();

        internal static async UniTask<T> TrackAndLoad<T>(object key, ResourceLoader owner,
            CancellationToken cancellationToken) where T : Object
        {
            if (key == null)
            {
                Debug.LogError("Key is null.");
                return null;
            }

            if (_registry.TryGetValue(key, out TrackedEntry existing))
            {
                existing.Owners.Add(owner);
                return (T)existing.Handle.Result;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask(cancellationToken: cancellationToken);

            return TrackLoadedHandle(key, owner, handle);
        }

        internal static async UniTask<T> TrackAndLoad<T>(AssetReference reference, ResourceLoader owner,
            CancellationToken cancellationToken) where T : Object
        {
            if (reference == null)
            {
                Debug.LogError("AssetReference is null.");
                return null;
            }

            if (reference.OperationHandle.IsValid())
            {
                return (T)reference.OperationHandle.Result;
            }

            AsyncOperationHandle handle;

            if (reference is AssetReferenceSprite spriteReference)
            {
                handle = spriteReference.LoadAssetAsync();
            }
            else
            {
                handle = reference.LoadAssetAsync<T>();
            }

            await handle.ToUniTask(cancellationToken: cancellationToken);

            var key = reference.RuntimeKey;

            if (_registry.TryGetValue(key, out TrackedEntry existing))
            {
                existing.Owners.Add(owner);
                return (T)existing.Handle.Result;
            }

            return (T)TrackLoadedHandle(key, owner, handle);
        }

        internal static void UnloadIfUnused(ResourceLoader owner)
        {
            List<object> toRemove = new();

            foreach (KeyValuePair<object, TrackedEntry> pair in _registry)
            {
                TrackedEntry entry = pair.Value;

                if (entry.Owners.Remove(owner) && entry.Owners.Count == 0)
                {
                    Debug.Log($"[AddressableAssetTracker] Unloaded asset '{entry.Handle.DebugName}'");
                    Addressables.Release(entry.Handle);
                    //AssetBundleGroupTracker.UnloadIfUnused(owner);
                    toRemove.Add(pair.Key);
                }
            }

            foreach (var key in toRemove)
            {
                _registry.Remove(key);
            }
        }

        internal static ResourceLoader TryGetSingleOwner(AssetReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            var key = reference.RuntimeKey;

            if (_registry.TryGetValue(key, out TrackedEntry entry) && entry.Owners.Count == 1)
            {
                foreach (ResourceLoader owner in entry.Owners)
                {
                    return owner;
                }
            }

            return null;
        }

        private static Object TrackLoadedHandle(object key, ResourceLoader owner, AsyncOperationHandle handle)
        {
            var entry = new TrackedEntry { Handle = handle };

            entry.Owners.Add(owner);
            _registry[key] = entry;

            return (Object)handle.Result;
        }

        private static T TrackLoadedHandle<T>(object key, ResourceLoader owner, AsyncOperationHandle<T> handle)
            where T : Object
        {
            var entry = new TrackedEntry { Handle = handle };

            entry.Owners.Add(owner);
            _registry[key] = entry;

            return handle.Result;
        }

        private class TrackedEntry
        {
            public readonly HashSet<ResourceLoader> Owners = new();
            public AsyncOperationHandle Handle;
        }
    }
}
#endif
