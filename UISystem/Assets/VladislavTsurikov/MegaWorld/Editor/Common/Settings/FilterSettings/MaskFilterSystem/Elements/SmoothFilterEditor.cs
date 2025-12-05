#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(SmoothFilter))]
    public class SmoothFilterEditor : MaskFilterEditor
    {
        public static readonly GUIContent Direction =
            EditorGUIUtility.TrTextContent("Verticality", "Blur only up (1.0), only down (-1.0) or both (0.0)");

        public static readonly GUIContent KernelSize =
            EditorGUIUtility.TrTextContent("Blur Radius", "Specifies the size of the blur kernel");

        private SmoothFilter _smoothFilter;

        public override void OnEnable() => _smoothFilter = (SmoothFilter)Target;

        public override void OnGUI(Rect rect, int index) =>
            _smoothFilter.SmoothBlurRadius =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), KernelSize,
                    _smoothFilter.SmoothBlurRadius, 0.0f, 10.0f);

        public override float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
    }
}
#endif
