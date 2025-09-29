using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "MonoBehaviourConfigB", menuName = "Test/MonoBehaviourConfigB")]
    public class MonoBehaviourConfigB : ScriptableObject
    {
        public AssetReferenceT<GeneralConfig> GeneralConfig;
    }
}
