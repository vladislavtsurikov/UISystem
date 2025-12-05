#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [Serializable]
    [ElementEditor(typeof(AdditionalRotation))]
    public class AdditionalRotationEditor : ReorderableListComponentEditor
    {
        private AdditionalRotation _additionalRotation;

        public override void OnEnable() => _additionalRotation = (AdditionalRotation)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _additionalRotation.Rotation = EditorGUI.Vector3Field(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Additional Rotation"), _additionalRotation.Rotation);
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 2;

            return height;
        }
    }
}
#endif
