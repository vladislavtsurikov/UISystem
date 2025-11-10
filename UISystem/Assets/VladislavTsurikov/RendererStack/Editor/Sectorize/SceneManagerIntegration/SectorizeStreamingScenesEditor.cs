#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.SceneManagerIntegration;
using VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.SceneManagerIntegration
{
    [DontDrawForAddButton]
    [ElementEditor(typeof(SectorizeStreamingScenes))]
    public class SectorizeStreamingScenesEditor : SceneTypeEditor
    {
        private ReorderableList _reorderableList;
        private SectorizeStreamingScenes _sectorizeStreamingScenes;

        public override void OnEnable()
        {
            base.OnEnable();

            _sectorizeStreamingScenes = (SectorizeStreamingScenes)Target;

            _reorderableList = new ReorderableList(_sectorizeStreamingScenes.SubScenes, typeof(SceneReference), false,
                true, false, false) { drawHeaderCallback = DrawHeader, drawElementCallback = DrawElement };
        }

        public override void OnGUI(Rect rect, int index)
        {
            _reorderableList.DoList(rect);

            rect.y += CustomEditorGUI.SingleLineHeight * 2;
            rect.y += _sectorizeStreamingScenes.SubScenes.Count * CustomEditorGUI.SingleLineHeight;

            base.OnGUI(rect, index);
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight * 2;
            height += _sectorizeStreamingScenes.SubScenes.Count * CustomEditorGUI.SingleLineHeight;

            height += base.GetElementHeight(index);
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }

        private void DrawElement(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                Rect rectField = totalRect;
                rectField.width -= 14;

                _sectorizeStreamingScenes.SubScenes[index].SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                    new Rect(rectField.x, rectField.y, rectField.width, EditorGUIUtility.singleLineHeight),
                    null, _sectorizeStreamingScenes.SubScenes[index].SceneAsset, typeof(SceneAsset));
            }
        }

        private void DrawHeader(Rect rect) =>
            CustomEditorGUI.Label(rect, "Scenes", CustomEditorGUI.GetStyle(StyleName.LabelFoldout));
    }
}
#endif
