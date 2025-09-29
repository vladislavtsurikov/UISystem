using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "MonoBehaviourConfigA", menuName = "Test/MonoBehaviourConfigA")]
    public class MonoBehaviourConfigA : ScriptableObject
    {
        public AssetReferenceT<GeneralConfig> GeneralConfig;
    }
}
