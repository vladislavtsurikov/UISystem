using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class PrefabSpawner : MonoBehaviour
    {
        [Inject]
        private ConfigSceneAWithAssetReference _config;

        private async void Start() => await SpawnPrefab();

        private async UniTask SpawnPrefab() => await _config.PrefabRef.InstantiateWithAutoLoad();
    }
}
