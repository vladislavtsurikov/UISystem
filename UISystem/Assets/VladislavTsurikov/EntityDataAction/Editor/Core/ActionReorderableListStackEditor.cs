#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataAction.Editor.Core
{
    public sealed class ActionReorderableListStackEditor : ReorderableListStackEditor<Action, ActionReorderableListComponentEditor>
    {
        private readonly NodeStackOnlyDifferentTypes<ComponentData> _dataStack;
        private static GUIStyle s_missingStyle;

        public ActionReorderableListStackEditor(
            AdvancedNodeStack<Action> actionStack,
            NodeStackOnlyDifferentTypes<ComponentData> dataStack)
            : base(new GUIContent("Actions"), actionStack, true)
        {
            _dataStack = dataStack;
        }

        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (Type actionType in GetComponentTypes())
            {
                if (actionType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }

                if (actionType.GetAttribute<PersistentNodeAttribute>() != null ||
                    actionType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                {
                    continue;
                }

                NameAttribute nameAttribute = actionType.GetAttribute<NameAttribute>();

                string context = nameAttribute != null ? nameAttribute.Name : actionType.Name;

                if (!IsNameAllowed(context))
                {
                    continue;
                }

                bool requirementsMet = RequiresDataUtility.IsRequirementsMet(_dataStack, actionType);

                if (!requirementsMet)
                {
                    menu.AddDisabledItem(new GUIContent(context));
                    continue;
                }

                if (Stack is NodeStackSupportSameType<Action> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => componentStackWithSameTypes.CreateNode(actionType));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(context));
                }
            }

            menu.ShowAsContext();
        }

        protected override void DrawHeaderElement(Rect totalRect, int index, ActionReorderableListComponentEditor componentEditor)
        {
            Type actionType = Stack.ElementList[index].GetType();
            bool requirementsMet = RequiresDataUtility.IsRequirementsMet(_dataStack, actionType);

            if (requirementsMet)
            {
                base.DrawHeaderElement(totalRect, index, componentEditor);
                return;
            }

            EditorGUI.DrawRect(totalRect, new Color(0.35f, 0.05f, 0.05f, 0.35f));

            Rect headerRect = totalRect;
            headerRect.x += 15;
            headerRect.height = GetWarningHeaderHeight();

            System.Action menu = () => Menu(Stack.ElementList[index], index);

            CustomEditorGUI.WarningHeaderWithMenu(headerRect, componentEditor.Target.Name, menu);

            string missingText = BuildMissingRequiredMultilineText(actionType);
            if (string.IsNullOrEmpty(missingText))
            {
                return;
            }

            Rect missingRect = totalRect;
            missingRect.x += 15;
            missingRect.y += headerRect.height;
            missingRect.height = GetMissingBlockHeight(actionType);

            Rect labelRect = missingRect;
            labelRect.xMin += EditorGUIUtility.singleLineHeight * 2f;
            labelRect.yMin += 2f;

            EditorGUI.LabelField(labelRect, missingText, GetMissingStyle());
        }

        protected override void DrawElement(Rect totalRect, int index, float iconSize, Color prevColor, ActionReorderableListComponentEditor componentEditor)
        {
            Type actionType = Stack.ElementList[index].GetType();
            bool requirementsMet = RequiresDataUtility.IsRequirementsMet(_dataStack, actionType);

            if (requirementsMet)
            {
                base.DrawElement(totalRect, index, iconSize, prevColor, componentEditor);
            }
        }

        public override float ElementHeightCB(int index)
        {
            Type actionType = Stack.ElementList[index].GetType();
            bool requirementsMet = RequiresDataUtility.IsRequirementsMet(_dataStack, actionType);

            if (requirementsMet)
            {
                return base.ElementHeightCB(index);
            }

            float baseHeight = base.ElementHeightCB(index);

            float defaultHeaderHeight = GetDefaultHeaderHeight();
            float warningHeaderHeight = GetWarningHeaderHeight(actionType);

            float extra = warningHeaderHeight - defaultHeaderHeight;
            if (extra < 0f)
            {
                extra = 0f;
            }

            return baseHeight + extra;
        }

        private string BuildMissingRequiredMultilineText(Type actionType)
        {
            Type[] requiredTypes = RequiresDataUtility.GetRequiredDataTypes(actionType);
            if (requiredTypes == null || requiredTypes.Length == 0)
            {
                return string.Empty;
            }

            List<string> missing = null;

            for (int i = 0; i < requiredTypes.Length; i++)
            {
                Type requiredType = requiredTypes[i];

                if (!_dataStack.HasType(requiredType))
                {
                    if (missing == null)
                    {
                        missing = new List<string>(requiredTypes.Length);
                    }

                    missing.Add(requiredType.Name);
                }
            }

            if (missing == null || missing.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(64);

            for (int i = 0; i < missing.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append('\n');
                }

                sb.Append(missing[i]);
            }

            return sb.ToString();
        }


        private float GetWarningHeaderHeight(Type actionType)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            int missingCount = GetMissingRequiredCount(actionType);

            int lines = 1 + missingCount;
            float padding = 4f;

            return (lineHeight * lines) + padding;
        }

        private int GetMissingRequiredCount(Type actionType)
        {
            Type[] requiredTypes = RequiresDataUtility.GetRequiredDataTypes(actionType);
            if (requiredTypes == null || requiredTypes.Length == 0)
            {
                return 0;
            }

            int count = 0;

            for (int i = 0; i < requiredTypes.Length; i++)
            {
                if (!_dataStack.HasType(requiredTypes[i]))
                {
                    count++;
                }
            }

            return count;
        }

        private float GetDefaultHeaderHeight()
        {
            return EditorGUIUtility.singleLineHeight * 1.3f;
        }

        private float GetWarningHeaderHeight()
        {
            return EditorGUIUtility.singleLineHeight * 1.3f;
        }

        private float GetMissingBlockHeight(Type actionType)
        {
            int missingCount = GetMissingRequiredCount(actionType);
            if (missingCount == 0)
            {
                return 0f;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float padding = 4f;

            return (lineHeight * missingCount) + padding;
        }

        private static GUIStyle GetMissingStyle()
        {
            if (s_missingStyle != null)
            {
                return s_missingStyle;
            }

            s_missingStyle = new GUIStyle(EditorStyles.miniLabel);
            s_missingStyle.normal.textColor = new Color(1f, 0.35f, 0.35f, 1f);
            s_missingStyle.alignment = TextAnchor.UpperLeft;
            s_missingStyle.wordWrap = false;
            s_missingStyle.richText = false;

            return s_missingStyle;
        }
    }
}
#endif
