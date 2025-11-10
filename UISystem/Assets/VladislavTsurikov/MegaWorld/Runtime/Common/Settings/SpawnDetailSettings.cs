using System;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Serializable]
    [Name("Spawn Detail Settings")]
    public class SpawnDetailSettings : Component
    {
        public bool UseRandomOpacity = true;
        public int Density = 5;
        public float FailureRate;
    }
}
