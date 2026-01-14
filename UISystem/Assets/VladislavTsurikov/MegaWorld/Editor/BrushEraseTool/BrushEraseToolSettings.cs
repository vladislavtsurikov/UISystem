#if UNITY_EDITOR
using System;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool
{
    [Serializable]
    [Name("Brush Erase Tool Settings")]
    public class BrushEraseToolSettings : Node
    {
        public float EraseStrength = 1.0f;
    }
}
#endif
