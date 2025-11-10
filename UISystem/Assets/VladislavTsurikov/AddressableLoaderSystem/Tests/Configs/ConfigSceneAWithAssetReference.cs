#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "ConfigSceneA_WithAssetReference", menuName = "Test/ConfigSceneA_Ref")]
    public class ConfigSceneAWithAssetReference : ScriptableObject
    {
        public AssetReferenceGameObject PrefabRef;
    }
}
#endif
