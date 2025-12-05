#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.PrototypeElements
{
    [Serializable]
    [Name("Additional Erase Settings")]
    public class AdditionalEraseSetting : Component
    {
        [Range(0, 100)]
        public float Success = 100f;
    }
}
#endif
