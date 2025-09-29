using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public abstract class PrefabResourceLoader : ResourceLoader
    {
        private bool _prefabLoaded;

        public abstract string PrefabAddress { get; }

        public virtual bool LoadOnStartup => true;

        public GameObject LoadedPrefab { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            if (LoadOnStartup)
            {
                await LoadPrefabIfNotLoaded(token);
            }
        }

        public async UniTask<GameObject> LoadPrefabIfNotLoaded(CancellationToken cancellationToken)
        {
            if (_prefabLoaded)
            {
                return LoadedPrefab;
            }

            LoadedPrefab = await LoadAndTrack<GameObject>(PrefabAddress, cancellationToken);
            _prefabLoaded = true;

            return LoadedPrefab;
        }

        protected override UniTask UnloadResourceLoader(CancellationToken cancellationToken)
        {
            _prefabLoaded = false;
            LoadedPrefab = null;
            return UniTask.CompletedTask;
        }
    }
}
