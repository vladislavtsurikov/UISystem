using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    [Name("Brush Physics Tool Settings")]
    public class BrushPhysicsToolSettings : Component
    {
        public PhysicsEffects PhysicsEffects = new();
        public float PositionOffsetY = 30;
    }
}
