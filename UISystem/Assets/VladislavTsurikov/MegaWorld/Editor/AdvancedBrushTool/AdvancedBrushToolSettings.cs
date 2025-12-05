#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [Serializable]
    [Name("Advanced Brush Tool Settings")]
    public class AdvancedBrushToolSettings : Component
    {
        public float TextureTargetStrength = 1.0f;

        public bool EnableFailureRateOnMouseDrag = true;
        public float FailureRateOnMouseDrag = 50f;
    }
}
#endif
