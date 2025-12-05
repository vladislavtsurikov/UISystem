#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem
{
    [ElementEditor(typeof(Single))]
    public class SingleSceneEditor : SceneTypeEditor
    {
        private Single _single;

        public override void OnEnable()
        {
            base.OnEnable();

            _single = (Single)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            _single.SceneReference.SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), null,
                _single.SceneReference.SceneAsset, typeof(SceneAsset));
            rect.y += CustomEditorGUI.SingleLineHeight;

            base.OnGUI(rect, index);
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += base.GetElementHeight(index);

            return height;
        }
    }
}
#endif
