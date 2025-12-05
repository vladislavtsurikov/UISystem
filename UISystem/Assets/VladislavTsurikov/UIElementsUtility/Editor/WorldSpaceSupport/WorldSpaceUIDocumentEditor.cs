#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.WorldSpaceSupport;

namespace VladislavTsurikov.UIElementsUtility.Editor.WorldSpaceSupport
{
    [CustomEditor(typeof(WorldSpaceUIDocument))]
    public class WorldSpaceUIDocumentEditor : UnityEditor.Editor
    {
        private WorldSpaceUIDocument _target => (WorldSpaceUIDocument)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _target.EditorWorldSpaceUIDocumentSupport.Resolution =
                EditorGUILayout.Vector2IntField(new GUIContent("Resolution"),
                    _target.EditorWorldSpaceUIDocumentSupport.Resolution);

            if (GUILayout.Button(new GUIContent("Set Resolution From Scale Transform")))
            {
                Vector3 localScale = _target.transform.localScale;
                _target.EditorWorldSpaceUIDocumentSupport.Resolution =
                    new Vector2Int((int)(localScale.x * 100), (int)(localScale.y * 100));
            }
        }
    }
}
#endif
