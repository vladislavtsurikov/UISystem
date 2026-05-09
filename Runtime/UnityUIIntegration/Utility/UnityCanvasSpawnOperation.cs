#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public class UnityCanvasSpawnOperation
        : UISpawnOperation<UnityCanvasSpawnOperation, Transform, GameObject, PrefabAssetLoader, UnityUIComponentBinder>
    {
        public UnityCanvasSpawnOperation Enable(bool enable)
        {
            return SetVisibleState(enable);
        }

        protected override UniTask<GameObject> CreateInstance(
            PrefabAssetLoader prefabLoader,
            Transform parent,
            CancellationToken cancellationToken)
        {
            return prefabLoader.LoadAndSpawnPrefab(parent, cancellationToken);
        }

        protected override void ApplyName(GameObject instance, string name)
        {
            instance.name = name;
        }

        protected override void ApplyVisibility(GameObject instance, bool visible)
        {
            instance.SetActive(visible);
        }

        protected override void AttachToParent(GameObject instance, Transform parent)
        {
        }

        protected override void Bind(GameObject instance, UnityUIComponentBinder componentBinder)
        {
            componentBinder.BindUIComponentsFrom(instance);
        }
    }
}
#endif
