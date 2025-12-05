#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.OperationSystem.Components
{
    [ElementEditor(typeof(SpawnPrefabs))]
    public class SpawnPrefabsEditor : ReorderableListComponentEditor
    {
        private ReorderableList _reorderableList;
        private SpawnPrefabs _spawnPrefabs;

        public override void OnEnable()
        {
            _spawnPrefabs = (SpawnPrefabs)Target;
            _reorderableList =
                new ReorderableList(_spawnPrefabs.GameObjects, typeof(GameObject), true, false, true, true);
            _reorderableList.drawElementCallback = DrawElementCB;
        }

        public override void OnGUI(Rect rect, int index)
        {
            _reorderableList.DoList(rect);

            rect.y += CustomEditorGUI.SingleLineHeight * 2;
            rect.y += _spawnPrefabs.GameObjects.Count * CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight * 2;
            height += _spawnPrefabs.GameObjects.Count * CustomEditorGUI.SingleLineHeight;

            return height;
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused) =>
            _spawnPrefabs.GameObjects[index] = (GameObject)CustomEditorGUI.ObjectField(
                new Rect(totalRect.x, totalRect.y, totalRect.width, EditorGUIUtility.singleLineHeight),
                null, _spawnPrefabs.GameObjects[index], typeof(GameObject));
    }
}
#endif
