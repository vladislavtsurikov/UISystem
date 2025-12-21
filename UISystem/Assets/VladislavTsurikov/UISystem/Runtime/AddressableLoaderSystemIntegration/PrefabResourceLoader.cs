#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public abstract class PrefabResourceLoader : ResourceLoader
    {
        private bool _prefabLoaded;
        private string _prefabAddress;
        private bool _prefabAddressResolved;

        public virtual bool LoadOnStartup => true;

        public GameObject LoadedPrefab { get; private set; }

        public string PrefabAddress => _prefabAddress;

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

            LoadedPrefab = await LoadAndTrack<GameObject>(GetPrefabAddress(), cancellationToken);
            _prefabLoaded = true;

            return LoadedPrefab;
        }

        protected override UniTask UnloadResourceLoader(CancellationToken cancellationToken)
        {
            _prefabLoaded = false;
            LoadedPrefab = null;
            return UniTask.CompletedTask;
        }

        private string GetPrefabAddress()
        {
            if (_prefabAddressResolved)
            {
                return _prefabAddress;
            }

            _prefabAddressResolved = true;

            var attr = GetType().GetAttribute<PrefabAddressAttribute>();
            if (attr == null || string.IsNullOrEmpty(attr.Address))
            {
                Debug.LogError($"[PrefabResourceLoader] Missing [PrefabAddress] on {GetType().FullName}");
                _prefabAddress = string.Empty;
                return _prefabAddress;
            }

            _prefabAddress = attr.Address;
            return _prefabAddress;
        }
    }
}

#endif
