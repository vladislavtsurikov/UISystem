using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings
{
    [Name("Physics Transform Settings")]
    public class PhysicsTransformComponentSettings : Component
    {
        public TransformComponentStack TransformComponentStack = new();

        protected override void OnCreate()
        {
            TransformComponentStack.CreateIfMissingType(typeof(PositionOffset));
            TransformComponentStack.CreateIfMissingType(typeof(Rotation));
            TransformComponentStack.CreateIfMissingType(typeof(Scale));
        }
    }
}
