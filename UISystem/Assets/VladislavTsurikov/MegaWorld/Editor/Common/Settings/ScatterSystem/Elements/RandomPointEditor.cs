#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(RandomPoint))]
    public class RandomPointEditor : ReorderableListComponentEditor
    {
        private RandomPoint _randomPoint;

        public override void OnEnable() => _randomPoint = (RandomPoint)Target;

        public override void OnGUI(Rect rect, int index)
        {
            var alignmentStyleRight = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight, stretchWidth = true
            };
            var alignmentStyleLeft = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft, stretchWidth = true
            };

            float minimumTmp = _randomPoint.MinChecks;
            float maximumTmp = _randomPoint.MaxChecks;

            EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Checks"), ref minimumTmp, ref maximumTmp, 1,
                PreferenceElementSingleton<ScatterPreferenceSettings>.Instance.MaxChecks);

            _randomPoint.MinChecks = (int)minimumTmp;
            _randomPoint.MaxChecks = (int)maximumTmp;

            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                new GUIContent(""));
            var numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y,
                (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            var minContent = new GUIContent("");

            EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            _randomPoint.MinChecks = EditorGUI.IntField(numFieldRect, _randomPoint.MinChecks);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            _randomPoint.MaxChecks = EditorGUI.IntField(numFieldRect, _randomPoint.MaxChecks);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            var maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
