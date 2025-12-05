#if ADDRESSABLE_LOADER_SYSTEM_VCONTAINER
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.VContainerIntergration
{
    public abstract class VContainerResourceLoader : ResourceLoader
    {
        private readonly LifetimeScope _parentScope;
        private LifetimeScope _resourceScope;
        private readonly List<Object> _loadedAssets = new List<Object>();

        protected VContainerResourceLoader(LifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }

        protected virtual UniTask OnResourceLoad(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnResourceUnload(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        protected sealed override async UniTask LoadResourceLoader(CancellationToken token)
        {
            _loadedAssets.Clear();

            await OnResourceLoad(token);

            _resourceScope = LifetimeScope.Create(_parentScope, builder =>
            {
                for (int i = 0; i < _loadedAssets.Count; i++)
                {
                    Object asset = _loadedAssets[i];
                    builder.RegisterInstance(asset);
                }
            });
        }

        protected sealed override async UniTask UnloadResourceLoader(CancellationToken token)
        {
            await OnResourceUnload(token);

            if (_resourceScope != null)
            {
                Object.Destroy(_resourceScope.gameObject);
                _resourceScope = null;
            }

            _loadedAssets.Clear();
        }

        protected async UniTask<T> LoadForScope<T>(string key, CancellationToken token)
            where T : Object
        {
            T asset = await LoadAndTrack<T>(key, token);

            if (asset != null)
            {
                _loadedAssets.Add(asset);
            }

            return asset;
        }
    }
}
#endif
