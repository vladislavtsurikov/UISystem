using UnityEngine;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class SceneBInjectionValidator : MonoBehaviour
    {
        [Inject]
        private ConfigSceneBWithAssetReference _configSceneBWithAssetReference;

        [Inject]
        private ConfigSceneB _sceneBConfig;

        [Inject]
        private SceneBConfigLoader _sceneBConfigLoader;

        private void Start()
        {
            if (_sceneBConfig == null)
            {
                Debug.LogError("[SceneBInjectionValidator] Injected SceneBConfig is NULL!");
            }
            else
            {
                Debug.Log($"[SceneBInjectionValidator] SceneBConfig injected successfully: {_sceneBConfig.name}");
            }

            if (_configSceneBWithAssetReference == null)
            {
                Debug.LogError("[SceneBInjectionValidator] Injected ConfigSceneBWithAssetReference is NULL!");
            }
            else
            {
                if (_configSceneBWithAssetReference.TextureRef1 == null ||
                    _configSceneBWithAssetReference.TextureRef2 == null ||
                    _configSceneBWithAssetReference.TextureRef3 == null)
                {
                    Debug.LogError("[SceneBInjectionValidator] One or more AssetReferences are NULL!");
                }
                else
                {
                    Debug.Log(
                        $"[SceneBInjectionValidator] Successfully injected ConfigSceneBWithAssetReference: {_sceneBConfig.name}");
                }
            }

            if (_sceneBConfigLoader == null)
            {
                Debug.LogError("[SceneAInjectionValidator] Injected SceneBConfigLoader is NULL!");
            }
            else
            {
                Debug.Log("[SceneAInjectionValidator] SceneBConfigLoader injected successfully.");
            }
        }
    }
}
