#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class ResourceLoaderExtensions
    {
        public static UniTask<GameObject> InstantiateWithAutoLoad(this ResourceLoader loader,
            AssetReferenceGameObject prefabRef, Transform parent = null, CancellationToken cancellationToken = default)
        {
            if (prefabRef == null)
            {
                Debug.LogError("Prefab reference is null.");
                return UniTask.FromResult<GameObject>(null);
            }

            AsyncOperationHandle<GameObject> handle = parent != null
                ? prefabRef.InstantiateAsync(parent)
                : prefabRef.InstantiateAsync();

            var completionSource = new UniTaskCompletionSource<GameObject>();

            handle.Completed += async operation =>
            {
                GameObject instance = operation.Result;

                if (instance != null)
                {
                    await AssetReferenceReflectionLoader.LoadAssetReferencesRecursive(instance, loader,
                        cancellationToken);
                    completionSource.TrySetResult(instance);
                }
                else
                {
                    Debug.LogError("Failed to instantiate prefab.");
                    completionSource.TrySetResult(null);
                }
            };

            return completionSource.Task;
        }
    }
}
#endif
