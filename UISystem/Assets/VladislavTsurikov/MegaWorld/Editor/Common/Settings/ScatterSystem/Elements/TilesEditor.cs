#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(Tiles))]
    public class TilesEditor : ReorderableListComponentEditor
    {
        private Tiles _scatter;

        public override void OnEnable() => _scatter = (Tiles)Target;

        public override void OnGUI(Rect rect, int index) =>
            _scatter.Size = Vector2.Max(new Vector2(1f, 1f),
                EditorGUI.Vector2Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Size"), _scatter.Size));

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
