#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneUtility.Runtime;
using Caching =
    VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem.Caching;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GlobalSettings.StreamingRulesSystem
{
    [ElementEditor(typeof(Caching))]
    public class CachingEditor : ReorderableListComponentEditor
    {
        private readonly GUIContent _cacheMemoryThreshold = new("Cache Memory Threshold",
            "RAM threshold where scenes will no longer be cached, but will be unloaded.");

        private readonly GUIContent _keepScenes = new("Keep Scenes",
            "Cached scenes will be unloaded in seconds starting when the scene was cached");

        private readonly GUIContent _maxLoadingCachedScenePause = new("Max Loading Cached Scene Pause",
            "Pause in seconds until the next attempt to load the cached scene");

        private Caching _caching;

        public override void OnEnable() => _caching = (Caching)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _caching.MaxLoadingCachedScenePause = Mathf.Max(0,
                CustomEditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    _maxLoadingCachedScenePause, _caching.MaxLoadingCachedScenePause));
            rect.y += CustomEditorGUI.SingleLineHeight;

            _caching.KeepScenes = Mathf.Max(0,
                CustomEditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    _keepScenes, _caching.KeepScenes));
            rect.y += CustomEditorGUI.SingleLineHeight;

            StreamingUtilitySettings.Instance.CacheMemoryThreshold = (CacheMemoryThreshold)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                _cacheMemoryThreshold, StreamingUtilitySettings.Instance.CacheMemoryThreshold);
            rect.y += CustomEditorGUI.SingleLineHeight;

            if (StreamingUtilitySettings.Instance.CacheMemoryThreshold == CacheMemoryThreshold.Custom)
            {
                EditorGUI.indentLevel++;
                StreamingUtilitySettings.Instance.CustomCacheMemoryThreshold = CustomEditorGUI.FloatField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Threshold In Megabyte"),
                    StreamingUtilitySettings.Instance.CustomCacheMemoryThreshold);
                rect.y += CustomEditorGUI.SingleLineHeight;
                EditorGUI.indentLevel--;
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            if (StreamingUtilitySettings.Instance.CacheMemoryThreshold == CacheMemoryThreshold.Custom)
            {
                EditorGUI.indentLevel++;
                height += CustomEditorGUI.SingleLineHeight;
                EditorGUI.indentLevel--;
            }

            return height;
        }
    }
}
#endif
