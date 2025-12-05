#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    public class MaskFilterStackEditor : ReorderableListStackEditor<MaskFilter, MaskFilterEditor>
    {
        public MaskFilterStackEditor(GUIContent label, MaskFilterStack filterStack) : base(label, filterStack, true) =>
            DisplayHeaderText = false;
    }
}
#endif
