#if UNITY_EDITOR
using Plugins.VladislavTsurikov.EntiryDataAction.Runtime;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EntityDataActionFramework;
using VladislavTsurikov.EntityDataActionFramework.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace Plugins.VladislavTsurikov.EntiryDataAction.Editor
{
    [CustomEditor(typeof(Entity), true)]
    public sealed class EntityEditor : UnityEditor.Editor
    {
        private Entity _entity;

        private ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor> _dataEditor;
        private ActionReorderableListStackEditor _actionsEditor;

        private void OnEnable()
        {
            _entity = (Entity)target;

            _dataEditor = new ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor>(
                new GUIContent("Data"), _entity.Data, true);
            _dataEditor.ShowActiveToggle = false;

            _actionsEditor = new ActionReorderableListStackEditor(_entity.Actions, _entity.Data);
        }

        public override void OnInspectorGUI()
        {
            DrawDirtyRunnerButton();

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(3);
            bool isDerived = _entity.GetType() != typeof(Entity);
            if (isDerived)
            {
                _dataEditor.DisplayPlusButton = false;
                _dataEditor.DuplicateSupport = false;
                _dataEditor.RemoveSupport = false;
                _dataEditor.ReorderSupport = false;

                _actionsEditor.DisplayPlusButton = false;
                _actionsEditor.DuplicateSupport = false;
                _actionsEditor.RemoveSupport = false;
                _actionsEditor.ReorderSupport = false;

                EditorGUILayout.HelpBox("Component lists are locked for Entity subclasses.", MessageType.Info);
            }
            else
            {
                _dataEditor.DisplayPlusButton = true;
                _dataEditor.DuplicateSupport = true;
                _dataEditor.RemoveSupport = true;
                _dataEditor.ReorderSupport = true;

                _actionsEditor.DisplayPlusButton = true;
                _actionsEditor.DuplicateSupport = true;
                _actionsEditor.RemoveSupport = true;
                _actionsEditor.ReorderSupport = true;
            }

            _dataEditor.OnGUI();

            GUILayout.Space(3);
            _actionsEditor.OnGUI();

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawDirtyRunnerButton()
        {
            DirtyActionRunner runner = _entity.DirtyRunner;
            if (runner == null)
            {
                return;
            }

            bool enabled = _entity.Active;

            Color prev = GUI.color;
            GUI.color = enabled ? new Color(0.2f, 0.8f, 0.2f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f);

            string label = enabled ? "Dirty Runner Enabled" : "Dirty Runner Disabled";
            if (GUILayout.Button(label))
            {
                _entity.Active = !enabled;
            }

            GUI.color = prev;
        }
    }
}
#endif
