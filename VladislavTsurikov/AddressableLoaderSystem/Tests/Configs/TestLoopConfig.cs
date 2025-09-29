using System;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "TestLoopConfig", menuName = "Tests/TestLoopConfig")]
    public class TestLoopConfig : ScriptableObject
    {
        public TestLoopData data;
    }

    [Serializable]
    public class TestLoopData
    {
        public TestLoopConfig backReference;
    }
}
