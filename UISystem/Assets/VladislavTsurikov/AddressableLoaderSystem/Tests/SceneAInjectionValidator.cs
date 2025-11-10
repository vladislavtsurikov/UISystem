#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using UnityEngine;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class SceneAInjectionValidator : MonoBehaviour
    {
        [Inject]
        private ConfigSceneAWithAssetReference _configSceneAWithAssetReference;

        [Inject]
        private ConfigSceneA _sceneAConfig;

        [Inject]
        private SceneAConfigLoader _sceneAConfigLoader;

        private void Start()
        {
            if (_sceneAConfig == null)
            {
                Debug.LogError("[SceneAInjectionValidator] Injected SceneAConfig is NULL!");
            }
            else
            {
                Debug.Log($"[SceneAInjectionValidator] SceneAConfig injected successfully: {_sceneAConfig.name}");
            }

            if (_configSceneAWithAssetReference == null)
            {
                Debug.LogError("[SceneAInjectionValidator] Injected ConfigSceneAWithAssetReference is NULL!");
            }
            else
            {
                if (_configSceneAWithAssetReference.PrefabRef == null)
                {
                    Debug.LogError("[SceneAInjectionValidator] Injected PrefabRef is NULL!");
                }
                else
                {
                    Debug.Log(
                        $"[SceneAInjectionValidator] Successfully injected ConfigSceneAWithAssetReference: {_sceneAConfig.name}, " +
                        $"PrefabRef: {_configSceneAWithAssetReference.PrefabRef.Asset.name}");
                }
            }

            if (_sceneAConfigLoader == null)
            {
                Debug.LogError("[SceneAInjectionValidator] Injected _sceneAConfigLoader is NULL!");
            }
            else
            {
                Debug.Log("[SceneAInjectionValidator] _sceneAConfigLoader injected successfully.");
            }
        }
    }
}
#endif
#endif
