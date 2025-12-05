#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GlobalSettings.StreamingRulesSystem
{
    [ElementEditor(typeof(AsynchronousLoading))]
    public class AsynchronousLoadingEditor : ReorderableListComponentEditor
    {
        private readonly GUIContent _maxDistance = new("Max Distance",
            "Loads scenes not in one frame, first the scene is loaded, then a pause for a few seconds, then the next scene is loaded. " +
            "This parameter is necessary to make loading scenes as invisible to the player as possible.");

        private readonly GUIContent _maxLoadingScenePause = new("Max Loading Scene Pause",
            "Pause in seconds until the next attempt to load the scene");

        private AsynchronousLoading _asynchronousLoading;

        public override void OnEnable() => _asynchronousLoading = (AsynchronousLoading)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _asynchronousLoading.MaxDistance = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), _maxDistance,
                _asynchronousLoading.MaxDistance);
            rect.y += CustomEditorGUI.SingleLineHeight;
            _asynchronousLoading.MaxLoadingScenePause =
                Mathf.Max(0,
                    CustomEditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        _maxLoadingScenePause, _asynchronousLoading.MaxLoadingScenePause));
            rect.y += CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
