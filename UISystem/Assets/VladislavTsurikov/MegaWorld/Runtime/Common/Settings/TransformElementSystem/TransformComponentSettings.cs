using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Name("Transform Settings")]
    public class TransformComponentSettings : Component
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
