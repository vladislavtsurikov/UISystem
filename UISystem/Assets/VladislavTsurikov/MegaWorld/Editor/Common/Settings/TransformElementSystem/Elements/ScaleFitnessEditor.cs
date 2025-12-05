#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(ScaleFitness))]
    public class ScaleFitnessEditor : ReorderableListComponentEditor
    {
        private ScaleFitness _scaleFitness;

        public override void OnEnable() => _scaleFitness = (ScaleFitness)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _scaleFitness.OffsetScale = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Offset Scale"), _scaleFitness.OffsetScale);
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
