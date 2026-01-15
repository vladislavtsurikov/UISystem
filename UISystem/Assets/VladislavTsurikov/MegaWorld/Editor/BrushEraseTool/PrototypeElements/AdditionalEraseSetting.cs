#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.PrototypeElements
{
    [Serializable]
    [Name("Additional Erase Settings")]
    public class AdditionalEraseSetting : Node
    {
        [Range(0, 100)]
        public float Success = 100f;
    }
}
#endif
