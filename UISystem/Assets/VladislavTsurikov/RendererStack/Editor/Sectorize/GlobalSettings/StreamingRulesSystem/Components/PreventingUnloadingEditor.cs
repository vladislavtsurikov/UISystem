#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GlobalSettings.StreamingRulesSystem
{
    [ElementEditor(typeof(PreventingUnloading))]
    public class PreventingUnloadingEditor : ReorderableListComponentEditor
    {
        private readonly GUIContent _maxDistance = new("Max Distance");
        private PreventingUnloading _preventingUnloading;

        public override void OnEnable() => _preventingUnloading = (PreventingUnloading)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _preventingUnloading.MaxDistance = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), _maxDistance,
                _preventingUnloading.MaxDistance);
            rect.y += CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
