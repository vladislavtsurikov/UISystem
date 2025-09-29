using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "ConfigSceneB", menuName = "Test/ConfigSceneB")]
    public class ConfigSceneB : ScriptableObject
    {
        public string Message = "Scene B";
    }
}
