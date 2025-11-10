#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public class UISpawnOperation
    {
        private bool _enable;
        private string _name;
        private Transform _parentTransform;

        public UISpawnOperation Enable(bool enable)
        {
            _enable = enable;
            return this;
        }

        public UISpawnOperation WithParent(Transform parent)
        {
            _parentTransform = parent;
            return this;
        }

        public UISpawnOperation WithName(string name)
        {
            _name = name;
            return this;
        }

        public async UniTask<GameObject> Execute(PrefabResourceLoader prefabLoader, UIComponentBinder componentBinder,
            CancellationToken cancellationToken)
        {
            GameObject instance = await prefabLoader.LoadAndSpawnPrefab(_parentTransform, cancellationToken);

            if (!string.IsNullOrEmpty(_name))
            {
                instance.name = _name;
            }

            instance.SetActive(_enable);

            componentBinder.BindUIComponentsFrom(instance);

            return instance;
        }
    }
}
#endif
