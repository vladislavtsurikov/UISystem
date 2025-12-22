#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Behavior;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [Behavior(typeof(SceneBehavior), "TestScene_A")]
    public class SceneAConfigLoader : ZenjectResourceLoader
    {
        public SceneAConfigLoader(DiContainer container) : base(container)
        {
        }

        public ConfigSceneA ConfigSceneA { get; private set; }

        public ConfigSceneAWithAssetReference ConfigSceneA_WithAssetReference { get; private set; }

        public DictionarySpriteConfigSceneA DictionarySpriteConfigSceneA { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            ConfigSceneA = await LoadAndBind<ConfigSceneA>(token, "ConfigSceneA");
            ConfigSceneA_WithAssetReference =
                await LoadAndBind<ConfigSceneAWithAssetReference>(token, "ConfigSceneA_WithAssetReference");
            DictionarySpriteConfigSceneA =
                await LoadAndBind<DictionarySpriteConfigSceneA>(token, "DictionarySpriteConfigSceneA");

            ValidateSpriteReferences();
        }

        private void ValidateSpriteReferences()
        {
            if (DictionarySpriteConfigSceneA == null || DictionarySpriteConfigSceneA.Sprites == null ||
                !DictionarySpriteConfigSceneA.Sprites.Any())
            {
                Debug.LogError(
                    "[SceneAConfigLoader] DictionarySpriteConfig or its ChapterImages dictionary is null or empty!");
                return;
            }

            foreach (KeyValuePair<string, AssetReferenceSprite> entry in DictionarySpriteConfigSceneA.Sprites)
            {
                if (entry.Value.Asset == null)
                {
                    Debug.LogError($"[SceneAConfigLoader] Sprite for key '{entry.Key}' is not valid or not loaded!");
                }
                else
                {
                    Debug.Log(
                        $"[SceneAConfigLoader] Sprite for key '{entry.Key}' loaded successfully. Type: {entry.Value.Asset.GetType()}");
                }
            }
        }
    }
}
#endif
#endif
