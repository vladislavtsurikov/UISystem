#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class ChildSpawningUIHandler : ComponentBindingUIHandler
    {
        private readonly List<(GameObject instance, PrefabResourceLoader loader)> _spawnedChildren = new();

        protected ChildSpawningUIHandler(DiContainer container)
            : base(container)
        {
        }

        protected async UniTask<GameObject> SpawnChildPrefab(PrefabResourceLoader prefabLoader, Transform parent,
            bool enable, CancellationToken cancellationToken)
        {
            GameObject instance = await this.Spawn()
                .WithParent(parent)
                .Enable(enable)
                .Execute(prefabLoader, ComponentBinder, cancellationToken);

            _spawnedChildren.Add((instance, prefabLoader));

            return instance;
        }

        protected override async UniTask DestroyUIHandler(bool unload, CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            if (unload)
            {
                foreach ((GameObject _, PrefabResourceLoader loader) in _spawnedChildren)
                {
                    await loader.Unload(cancellationToken);
                }
            }

            _spawnedChildren.Clear();

            await DestroyChildSpawningUIHandler(unload, cancellationToken);
        }

        protected virtual UniTask DestroyChildSpawningUIHandler(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        protected virtual void DisposeChildSpawningUIHandler()
        {
        }

        protected override void DisposeComponentBindingUIHandler()
        {
            _spawnedChildren.Clear();

            DisposeChildSpawningUIHandler();
        }
    }
}
#endif
