using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "ConfigSceneB_WithAssetReference", menuName = "Test/ConfigSceneB_Ref")]
    public class ConfigSceneBWithAssetReference : ScriptableObject
    {
        public AssetReferenceTexture2D TextureRef1;
        public AssetReferenceTexture2D TextureRef2;
        public AssetReferenceTexture2D TextureRef3;
    }
}
