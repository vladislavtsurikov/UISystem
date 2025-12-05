#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.ActionFlow.Editor
{
    [CustomEditor(typeof(Actions))]
    public class ActionsEditor : UnityEditor.Editor
    {
        private ReorderableListStackEditor<Action, ReorderableListComponentEditor> _actionsCollectionEditor;
        private Actions Actions => (Actions)target;

        public void OnEnable() =>
            _actionsCollectionEditor = new ReorderableListStackEditor<Action, ReorderableListComponentEditor>(
                new GUIContent("Actions"), Actions.ActionCollection, true);

        public override void OnInspectorGUI() => _actionsCollectionEditor.OnGUI();
    }
}
#endif
