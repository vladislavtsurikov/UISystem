using System;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Serializable]
    [Name("Spawn Detail Settings")]
    public class SpawnDetailSettings : Node
    {
        public bool UseRandomOpacity = true;
        public int Density = 5;
        public float FailureRate;
    }
}
