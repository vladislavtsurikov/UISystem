#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.EntityDataAction.Editor;
using VladislavTsurikov.EntityDataAction.Editor.Core;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Style;

namespace VladislavTsurikov.EntityDataAction.Shared.Editor
{
    [ElementEditor(typeof(StyleActionGroup))]
    public sealed class StyleActionGroupEditor : ActionReorderableListComponentEditor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new IMGUIInspectorFieldsDrawer();
        private ActionReorderableListStackEditor _stackEditor;
        private StyleActionGroup _group;

        public override void OnGUI(Rect rect, int index)
        {
            if (Target == null)
            {
                return;
            }

            _group ??= Target as StyleActionGroup;
            if (_group == null)
            {
                return;
            }

            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_group, index);
            Rect fieldsRect = new Rect(rect.x, rect.y, rect.width, fieldsHeight);
            _fieldsDrawer.DrawFields(_group, fieldsRect, index);

            float warningHeight = 0f;
            if (string.IsNullOrWhiteSpace(_group.StyleName))
            {
                warningHeight = EditorGUIUtility.singleLineHeight * 2f;
                Rect warningRect = new Rect(rect.x, fieldsRect.yMax + 2f, rect.width, warningHeight);
                EditorGUI.HelpBox(warningRect, "Style name is empty.", MessageType.Warning);
            }

            if (Entity == null)
            {
                Rect labelRect = new Rect(rect.x, fieldsRect.yMax + warningHeight + 4f, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, "Entity not found");
                return;
            }

            EnsureStackEditor(Entity);

            Rect listRect = new Rect(
                rect.x,
                fieldsRect.yMax + warningHeight + 4f,
                rect.width,
                rect.height - fieldsHeight - warningHeight - 4f);
            _stackEditor.OnGUI(listRect);
        }

        public override float GetElementHeight(int index)
        {
            if (Target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            _group ??= Target as StyleActionGroup;
            if (_group == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float height = _fieldsDrawer.GetFieldsHeight(_group, index);
            float warningHeight = string.IsNullOrWhiteSpace(_group.StyleName)
                ? EditorGUIUtility.singleLineHeight * 2f
                : 0f;

            if (Entity == null)
            {
                return height + warningHeight + EditorGUIUtility.singleLineHeight + 4f;
            }

            EnsureStackEditor(Entity);

            return height + warningHeight + _stackEditor.GetElementStackHeight() + 4f;
        }

        private void EnsureStackEditor(Entity entity)
        {
            if (_group == null || entity == null)
            {
                return;
            }

            if (_stackEditor != null && ReferenceEquals(_stackEditor.Stack, _group.Actions))
            {
                return;
            }

            _stackEditor = new ActionReorderableListStackEditor(_group.Actions, entity.Data)
            {
                DisplayHeaderText = false,
                AllowedNamePrefixes = new[] { "UI/Common/Style/" }
            };
        }
    }
}
#endif
