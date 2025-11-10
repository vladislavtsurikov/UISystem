#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class AssetReferenceGameObjectExtensions
    {
        public static async UniTask<GameObject> InstantiateWithAutoLoad(this AssetReferenceGameObject prefabRef,
            Transform parent = null)
        {
            if (prefabRef == null)
            {
                Debug.LogError("[AssetReferenceGameObjectExtensions] PrefabRef is null.");
                return null;
            }

            ResourceLoader owner = AddressableAssetTracker.TryGetSingleOwner(prefabRef);

            if (owner != null)
            {
                return await owner.InstantiateWithAutoLoad(prefabRef, parent);
            }

            Debug.LogError(
                $"[AssetReferenceGameObjectExtensions] PrefabRef '{prefabRef.AssetGUID}' has no unique owner. Cannot resolve loader.");
            return null;
        }
    }
}
#endif
