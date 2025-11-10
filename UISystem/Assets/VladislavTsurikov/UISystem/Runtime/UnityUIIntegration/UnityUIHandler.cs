#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using Zenject;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class UnityUIHandler : ChildSpawningUIHandler
    {
        protected UnityUIHandler(DiContainer container, PrefabResourceLoader loader) : base(container) =>
            Loader = loader;

        public PrefabResourceLoader Loader { get; }

        public GameObject SpawnedParentPrefab { get; private set; }

        public event Action<GameObject, UnityUIHandler> OnAnyChildAdded;

        protected override async UniTask BeforeShowUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnMainPrefab(true, cancellationToken);

        protected virtual Transform GetSpawnParentTransform()
        {
            if (Parent == null)
            {
                return null;
            }

            if (Parent is UnityUIHandler unityUIUnit)
            {
                return unityUIUnit.SpawnedParentPrefab.transform;
            }

            throw new InvalidOperationException(
                $"Invalid parent type: {Parent.GetType().Name}. Expected UnityUIHandler.");
        }

        protected virtual string GetParentName() => null;

        protected override UniTask OnShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            SpawnedParentPrefab.SetActive(true);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            SpawnedParentPrefab.SetActive(false);
            return UniTask.CompletedTask;
        }

        protected override async UniTask DestroyChildSpawningUIHandler(bool unload, CancellationToken cancellationToken)
        {
            if (SpawnedParentPrefab != null)
            {
                Object.Destroy(SpawnedParentPrefab);
                SpawnedParentPrefab = null;
            }

            if (unload)
            {
                await Loader.Unload(cancellationToken);
            }
        }

        protected override void DisposeChildSpawningUIHandler() => SpawnedParentPrefab = null;

        private async UniTask<GameObject> SpawnMainPrefab(bool enable, CancellationToken cancellationToken)
        {
            if (SpawnedParentPrefab == null)
            {
                SpawnedParentPrefab = await this.Spawn()
                    .WithParent(GetSpawnParentTransform())
                    .Enable(enable)
                    .WithName(GetParentName())
                    .Execute(Loader, ComponentBinder, cancellationToken);

                if (Parent != null)
                {
                    var parentUnityUIHandler = (UnityUIHandler)Parent;
                    parentUnityUIHandler.OnAnyChildAdded?.Invoke(SpawnedParentPrefab, this);
                }
            }

            return SpawnedParentPrefab;
        }
    }
}

#endif

#endif
