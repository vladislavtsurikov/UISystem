#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class MonoBehaviourLoaderTest : MonoBehaviour
    {
        public AssetReferenceT<MonoBehaviourConfigA> ConfigAReference;
        public AssetReferenceT<MonoBehaviourConfigB> ConfigBReference;

        private void Start()
        {
            CheckAssetReference(ConfigAReference);
            CheckAssetReference(ConfigBReference);
        }

        public void CheckAssetReference<T>(AssetReferenceT<T> assetReference) where T : ScriptableObject
        {
            if (assetReference != null && assetReference.IsValid())
            {
                if (assetReference is AssetReferenceT<MonoBehaviourConfigA> configAReference)
                {
                    var configA = configAReference.Asset as MonoBehaviourConfigA;
                    if (configA != null)
                    {
                        Debug.Log($"Assigned MonoBehaviourConfigA: {configA.name}");
                        CheckGeneralConfig(configA.GeneralConfig);
                    }
                    else
                    {
                        Debug.LogError("Failed to load MonoBehaviourConfigA.");
                    }
                }
                else if (assetReference is AssetReferenceT<MonoBehaviourConfigB> configBReference)
                {
                    var configB = configBReference.Asset as MonoBehaviourConfigB;
                    if (configB != null)
                    {
                        Debug.Log($"Assigned MonoBehaviourConfigB: {configB.name}");
                        CheckGeneralConfig(configB.GeneralConfig);
                    }
                    else
                    {
                        Debug.LogError("Failed to load MonoBehaviourConfigB.");
                    }
                }
            }
            else
            {
                Debug.LogError($"Asset {assetReference.AssetGUID} is not valid.");
            }
        }

        private void CheckGeneralConfig(AssetReferenceT<GeneralConfig> generalConfigReference)
        {
            if (generalConfigReference != null && generalConfigReference.IsValid())
            {
                Debug.Log("GeneralConfig is valid and loaded.");
                Debug.Log($"GeneralConfig loaded with name: {generalConfigReference.AssetGUID}");
            }
            else
            {
                Debug.LogError("GeneralConfig is not valid.");
            }
        }
    }
}
#endif
