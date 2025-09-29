using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "DictionarySpriteConfigSceneA", menuName = "Test/DictionarySpriteConfigSceneA")]
    public class DictionarySpriteConfigSceneA : BaseConfig
    {
        [SerializeField]
        public SerializedDictionary<string, AssetReferenceSprite> Sprites = new();
    }
}
