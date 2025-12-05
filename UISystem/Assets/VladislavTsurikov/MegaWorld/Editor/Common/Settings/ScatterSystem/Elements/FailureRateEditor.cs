#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(FailureRate))]
    public class FailureRateEditor : ReorderableListComponentEditor
    {
        private FailureRate _failureRate;

        public override void OnEnable() => _failureRate = (FailureRate)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _failureRate.Value =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Value (%)"), _failureRate.Value, 0f, 100f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
