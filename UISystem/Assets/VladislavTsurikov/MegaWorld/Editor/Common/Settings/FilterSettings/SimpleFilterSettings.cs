#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
    [Serializable]
    public class SimpleFilterSettings
    {
        public bool FilterSettingsFoldout = true;

        private readonly GUIContent _checkHeight = new("Check Height");
        private readonly GUIContent _checkSlope = new("Check Slope");
        private readonly GUIContent _slope = new("Slope");

        public void OnGUI(SimpleFilter element, string text)
        {
            FilterSettingsFoldout = CustomEditorGUILayout.Foldout(FilterSettingsFoldout, text);

            if (FilterSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                DrawCheckHeight(element);
                DrawCheckSlope(element);

                EditorGUI.indentLevel--;
            }
        }

        public void DrawCheckHeight(SimpleFilter filter)
        {
            filter.CheckHeight = CustomEditorGUILayout.Toggle(_checkHeight, filter.CheckHeight);

            EditorGUI.indentLevel++;

            if (filter.CheckHeight)
            {
                filter.MinHeight = CustomEditorGUILayout.FloatField(new GUIContent("Min Height"), filter.MinHeight);
                filter.MaxHeight = CustomEditorGUILayout.FloatField(new GUIContent("Max Height"), filter.MaxHeight);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawCheckSlope(SimpleFilter filter)
        {
            filter.CheckSlope = CustomEditorGUILayout.Toggle(_checkSlope, filter.CheckSlope);

            EditorGUI.indentLevel++;

            if (filter.CheckSlope)
            {
                CustomEditorGUILayout.MinMaxSlider(_slope, ref filter.MinSlope, ref filter.MaxSlope, 0f, 90);
            }

            EditorGUI.indentLevel--;
        }
    }
}
#endif
