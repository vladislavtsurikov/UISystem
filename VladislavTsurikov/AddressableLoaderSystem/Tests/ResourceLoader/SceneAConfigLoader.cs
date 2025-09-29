using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [SceneFilter("TestScene_A")]
    public class SceneAConfigLoader : BindableResourceLoader
    {
        public SceneAConfigLoader(DiContainer container) : base(container)
        {
        }

        public ConfigSceneA Config { get; private set; }
        public ConfigSceneAWithAssetReference ConfigWithReference { get; private set; }
        public DictionarySpriteConfigSceneA DictionarySpriteConfig { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            Config = await LoadAndBind<ConfigSceneA>(token, "ConfigSceneA");
            ConfigWithReference =
                await LoadAndBind<ConfigSceneAWithAssetReference>(token, "ConfigSceneA_WithAssetReference");
            DictionarySpriteConfig =
                await LoadAndBind<DictionarySpriteConfigSceneA>(token, "DictionarySpriteConfigSceneA");

            ValidateSpriteReferences();
        }

        private void ValidateSpriteReferences()
        {
            if (DictionarySpriteConfig == null || DictionarySpriteConfig.Sprites == null ||
                !DictionarySpriteConfig.Sprites.Any())
            {
                Debug.LogError(
                    "[SceneAConfigLoader] DictionarySpriteConfig or its ChapterImages dictionary is null or empty!");
                return;
            }

            foreach (KeyValuePair<string, AssetReferenceSprite> entry in DictionarySpriteConfig.Sprites)
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
