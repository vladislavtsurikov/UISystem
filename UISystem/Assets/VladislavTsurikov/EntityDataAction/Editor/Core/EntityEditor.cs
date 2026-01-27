#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.EntityDataAction.Editor.Core
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

            if (!_entity.IsSetup && !Application.isPlaying)
            {
                _entity.Setup();
            }

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
                SetDirtyTarget();
            }
        }

        private void DrawDirtyRunnerButton()
        {
            DirtyActionRunner runner = _entity.DirtyRunner;
            if (runner == null)
            {
                return;
            }

            bool enabled = _entity.LocalActive;

            Color prev = GUI.color;
            GUI.color = enabled ? new Color(0.2f, 0.8f, 0.2f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f);

            string label = enabled ? "Active" : "Inactive";
            if (GUILayout.Button(label))
            {
                _entity.LocalActive = !enabled;
                SetDirtyTarget();
            }

            GUI.color = prev;

#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                EditorGUILayout.HelpBox("Inactive in Prefab Mode.", MessageType.Info);
            }
#endif
        }

        private void SetDirtyTarget()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif
