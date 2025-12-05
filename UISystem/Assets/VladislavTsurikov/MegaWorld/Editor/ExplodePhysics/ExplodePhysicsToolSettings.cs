#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    [Name("Explode Physics Tool Settings")]
    public class ExplodePhysicsToolSettings : Component
    {
        public float Force = 20f;
        public int InstancesMax = 50;
        public int InstancesMin = 25;
        public float PositionOffsetY = 15;
        public float Size = 10;
        public float Spacing = 20;
        public bool SpawnFromOnePoint;
    }
}
#endif
