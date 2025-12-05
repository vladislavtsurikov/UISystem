#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GlobalSettings.StreamingRulesSystem
{
    [ElementEditor(typeof(ImmediatelyLoading))]
    public class ImmediatelyLoadingEditor : ReorderableListComponentEditor
    {
        private readonly GUIContent _maxDistance = new("Max Distance",
            "Sets the distance from the camera where scenes will be loaded immediately in one frame. " +
            "It's better not to make this value too big, otherwise when the camera moves you will have to load too many scenes in one frame, which will cause a lot of delay, " +
            "I advise you to use the Offset Max Loading Distance With Pause parameter, this will not load scenes immediately in one frame");

        private ImmediatelyLoading _immediatelyLoading;

        public override void OnEnable() => _immediatelyLoading = (ImmediatelyLoading)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _immediatelyLoading.MaxDistance = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), _maxDistance,
                _immediatelyLoading.MaxDistance);
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
