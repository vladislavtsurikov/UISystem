#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Editor.Core;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.EntityDataAction.Editor
{
    [CustomEditor(typeof(FilteredEntity), true)]
    public class FilteredEntityEditor : UnityEditor.Editor
    {
        private FilteredEntity _entity;

        private ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor> _dataEditor;
        private ActionReorderableListStackEditor _actionsEditor;

        private void OnEnable()
        {
            _entity = (FilteredEntity)target;

            if (!_entity.IsSetup && !Application.isPlaying)
            {
                _entity.Setup();
            }

            _dataEditor = new ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor>(
                new GUIContent("Data"), _entity.Data, true);
            _dataEditor.ShowActiveToggle = false;
            _dataEditor.AllowedNamePrefixes = _entity.GetAllowedDataNamePrefixes();

            _actionsEditor = new ActionReorderableListStackEditor(_entity.Actions, _entity.Data);
            _actionsEditor.AllowedNamePrefixes = _entity.GetAllowedActionNamePrefixes();
        }

        public override void OnInspectorGUI()
        {
            DrawDirtyRunnerButton();

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(3);

            _dataEditor.OnGUI();

            GUILayout.Space(3);
            _actionsEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
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

            bool enabled = _entity.LocalActive;

            Color prev = GUI.color;
            GUI.color = enabled ? new Color(0.2f, 0.8f, 0.2f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f);

            string label = enabled ? "Active" : "Inactive";
            if (GUILayout.Button(label))
            {
                _entity.LocalActive = !enabled;
                EditorUtility.SetDirty(target);
            }

            GUI.color = prev;

#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                EditorGUILayout.HelpBox("Inactive in Prefab Mode.", MessageType.Info);
            }
#endif
        }
    }
}
#endif
