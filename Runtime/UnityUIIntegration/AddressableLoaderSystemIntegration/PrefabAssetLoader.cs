#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public abstract class PrefabAssetLoader : AddressableAssetLoader<GameObject>
    {
        protected override string AssetAddress => PrefabAddress;

        public abstract string PrefabAddress { get; }

        public GameObject LoadedPrefab => LoadedAsset;

        public UniTask<GameObject> LoadPrefabIfNotLoaded(CancellationToken cancellationToken)
        {
            return LoadAssetIfNotLoaded(cancellationToken);
        }
    }
}
#endif
