using System;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool
{
    [Serializable]
    [Name("Precise Physics Tool")]
    public class PrecisePhysicsToolSettings : Component
    {
        public float Spacing = 5;
        public float PositionOffsetY = 30;
    }
}
