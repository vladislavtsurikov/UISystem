#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(ExpandDownFilter))]
    public class ExpandDownFilterEditor : MaskFilterEditor
    {
        public static readonly GUIContent KernelSize = EditorGUIUtility.TrTextContent("Kernel Size");
        private ExpandDownFilter _filter;

        public override void OnEnable() => _filter = (ExpandDownFilter)Target;

        public override void OnGUI(Rect rect, int index) =>
            _filter.KernelSize = Mathf.Max(0.1f,
                EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    KernelSize, _filter.KernelSize));

        public override float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
    }
}
#endif
