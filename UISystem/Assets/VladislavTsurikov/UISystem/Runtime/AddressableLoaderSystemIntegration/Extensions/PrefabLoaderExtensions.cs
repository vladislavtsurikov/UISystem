#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public static class PrefabLoaderExtensions
    {
        public static async UniTask<GameObject> LoadAndSpawnPrefab(this PrefabResourceLoader prefabLoader,
            Transform parent, CancellationToken cancellationToken)
        {
            GameObject prefab = await prefabLoader.LoadPrefabIfNotLoaded(cancellationToken);

            if (prefab == null)
            {
                Debug.LogError(
                    $"[PrefabLoader] Prefab is null for loader: {prefabLoader.GetType().Name} (Address: {prefabLoader.PrefabAddress})");
                return null;
            }

            GameObject instance = Object.Instantiate(prefab, parent);
            return instance;
        }
    }
}
#endif
